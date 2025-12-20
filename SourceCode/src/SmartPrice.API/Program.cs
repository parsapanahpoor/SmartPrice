using Microsoft.EntityFrameworkCore;
using Serilog;
using SmartPrice.Application.Interfaces;
using SmartPrice.Infrastructure.Data;
using SmartPrice.Infrastructure.Repositories;
using SmartPrice.Infrastructure.Scraping;
using SmartPrice.Infrastructure.Scraping.Scrapers;

var builder = WebApplication.CreateBuilder(args);

// Serilog Configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting SmartPrice API");

    // Database Configuration
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly("SmartPrice.Infrastructure")
        ));

    // Redis Cache Configuration
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration["Redis:ConnectionString"];
        options.InstanceName = "SmartPrice_";
    });

    // Health Checks
    builder.Services.AddHealthChecks()
        .AddNpgSql(
            builder.Configuration.GetConnectionString("DefaultConnection")!,
            name: "postgresql",
            tags: new[] { "db", "sql", "postgresql" })
        .AddRedis(
            builder.Configuration["Redis:ConnectionString"]!,
            name: "redis",
            tags: new[] { "cache", "redis" });

    // Repository Registration
    builder.Services.AddScoped<IProductRepository, ProductRepository>();

    // Scraper Configuration
    builder.Services.Configure<ScraperOptions>(builder.Configuration.GetSection("Scraper"));

    // HttpClient for Scraping
    builder.Services.AddHttpClient("ScraperClient")
        .ConfigureHttpClient(client =>
        {
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            client.DefaultRequestHeaders.Add("Accept-Language", "fa-IR,fa;q=0.9,en-US;q=0.8,en;q=0.7");
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
        })
        .SetHandlerLifetime(TimeSpan.FromMinutes(5));

    // Scraper Services
    builder.Services.AddSingleton<IProxyManager, ProxyManager>();
    builder.Services.AddScoped<IScraperService, ScraperService>();
    
    // Marketplace Scrapers
    builder.Services.AddScoped<IMarketplaceScraper, DigikalaScraper>();
    // Add more scrapers here: Torob, Snapfood, Emalls, etc.

    // API Services
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "SmartPrice API",
            Version = "v1",
            Description = "A professional price tracking and comparison API for Iranian markets",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "SmartPrice Team",
                Email = "support@smartprice.ir"
            }
        });

        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
    });

    // CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    var app = builder.Build();

    // Middleware Pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartPrice API V1");
            c.RoutePrefix = string.Empty;
        });
    }

    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();

    app.UseCors("AllowAll");

    app.UseAuthorization();

    app.MapControllers();

    app.MapHealthChecks("/health");

    app.MapGet("/", () => Results.Redirect("/swagger"))
        .ExcludeFromDescription();

    Log.Information("SmartPrice API started successfully");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
