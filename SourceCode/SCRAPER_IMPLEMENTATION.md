# SmartPrice - Professional Web Scraper Implementation

## 📋 Overview
This implementation adds a professional, production-ready web scraping system to the SmartPrice application following Clean Architecture principles.

## ✅ Implementation Summary

### 1. Domain Layer (SmartPrice.Domain)

#### **New Enums** (`Enums/ScraperEnums.cs`)
- `ScrapingStatus`: Pending, InProgress, Completed, Failed, Skipped
- `MarketplaceType`: Digikala, Torob, Snapfood, Emalls, Other
- `ProxyStatus`: Active, Blocked, Dead

#### **New Entity** (`Entities/ProxyServer.cs`)
- Complete proxy server configuration entity
- Tracks IP, port, authentication, status, and usage statistics

#### **Updated Entity** (`Entities/ScrapingJob.cs`)
- Added `Marketplace` property (MarketplaceType)
- Added `RetryCount` property (int)
- Added `Duration` property (TimeSpan?)

### 2. Application Layer (SmartPrice.Application)

#### **Interfaces**
1. `IScraperService.cs` - Main scraping orchestration service
   - `ScrapeProductAsync()` - Single product scraping
   - `ScrapeProductsAsync()` - Batch scraping
   - `IsUrlValidAsync()` - URL validation
   - `DetectMarketplace()` - Marketplace detection

2. `IMarketplaceScraper.cs` - Marketplace-specific scraper interface
   - `ScrapeAsync()` - Scrape implementation
   - `CanHandle()` - URL handling check
   - `Marketplace` - Marketplace type property

3. `IProxyManager.cs` - Proxy rotation and management
   - `GetNextProxyAsync()` - Get next proxy from pool
   - `MarkProxyAsFailedAsync()` - Track failures
   - `MarkProxyAsSuccessAsync()` - Track successes

#### **DTOs** (`DTOs/Scraper/`)
- `ScrapingResult` - Result of scraping operation
- `ScrapedProductDto` - Scraped product data with metadata

### 3. Infrastructure Layer (SmartPrice.Infrastructure)

#### **Configuration** (`Scraping/ScraperOptions.cs`)
- `MaxConcurrentRequests` (default: 5)
- `RequestDelayMs` (default: 2000)
- `TimeoutSeconds` (default: 30)
- `MaxRetries` (default: 3)
- `UseProxy` (default: false)
- `UserAgents` (list of user agents)
- `Proxies` (list of proxy configurations)

#### **Core Services**

1. **ScraperService** (`Scraping/ScraperService.cs`)
   - Concurrent request limiting with SemaphoreSlim
   - Rate limiting with configurable delays
   - Automatic marketplace detection
   - Comprehensive error handling and logging
   - Batch scraping support

2. **ProxyManager** (`Scraping/ProxyManager.cs`)
   - Round-robin proxy rotation
   - Proxy authentication support
   - Failure tracking (ready for future enhancements)

3. **DigikalaScraper** (`Scraping/Scrapers/DigikalaScraper.cs`)
   - Exponential backoff retry logic
   - Multiple selector fallbacks for reliability
   - Random User-Agent rotation
   - Comprehensive HTML parsing
   - Persian text encoding support (UTF-8)
   - Extracts: title, price, image, availability, SKU

#### **Database Configuration**
- `ProxyServerConfiguration.cs` - EF Core configuration for ProxyServer
- Updated `ScrapingJobConfiguration.cs` with new properties
- Updated `ApplicationDbContext` with ProxyServers DbSet
- Manual migration created: `20251221000000_AddScraperEntities.cs`

### 4. API Layer (SmartPrice.API)

#### **Controller** (`Controllers/ScraperController.cs`)
Three endpoints:

1. **POST /api/scraper/test**
   - Test scraper with a single URL
   - Returns: ScrapingResult with product data or error

2. **POST /api/scraper/batch**
   - Scrape multiple URLs concurrently
   - Returns: Summary with total/successful/failed counts + results

3. **POST /api/scraper/validate**
   - Validate URL and detect marketplace
   - Returns: Validation status + marketplace type

#### **Configuration Updates**

**Program.cs**:
```csharp
// Scraper Configuration
builder.Services.Configure<ScraperOptions>(builder.Configuration.GetSection("Scraper"));

// HttpClient for Scraping
builder.Services.AddHttpClient("ScraperClient")
    .ConfigureHttpClient(client => {
        // Headers configuration
    })
    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

// Services
builder.Services.AddSingleton<IProxyManager, ProxyManager>();
builder.Services.AddScoped<IScraperService, ScraperService>();
builder.Services.AddScoped<IMarketplaceScraper, DigikalaScraper>();
```

**appsettings.json**:
```json
{
  "Scraper": {
    "MaxConcurrentRequests": 5,
    "RequestDelayMs": 2000,
    "TimeoutSeconds": 30,
    "MaxRetries": 3,
    "UseProxy": false,
    "UserAgents": [
      "Mozilla/5.0 (Windows NT 10.0; Win64; x64) ...",
      "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) ..."
    ],
    "Proxies": []
  }
}
```

## 📦 NuGet Packages Added

- `HtmlAgilityPack` (1.11.54) - HTML parsing
- `Polly` (8.2.0) - Resilience and retry policies
- `Microsoft.Extensions.Http` (7.0.0) - IHttpClientFactory
- `Microsoft.Extensions.Configuration.Json` (7.0.0) - Configuration

## 🎯 Key Features

### ✅ Implemented
- ✅ Clean Architecture compliance
- ✅ SOLID principles
- ✅ Dependency injection throughout
- ✅ Comprehensive error handling
- ✅ Retry logic with exponential backoff
- ✅ Rate limiting and concurrency control
- ✅ User-Agent rotation
- ✅ Marketplace detection
- ✅ Proxy rotation infrastructure
- ✅ Multiple HTML selector fallbacks
- ✅ Persian text encoding support
- ✅ Extensive logging with Serilog
- ✅ Database migration ready
- ✅ RESTful API endpoints
- ✅ Batch scraping support
- ✅ URL validation
- ✅ Swagger documentation

## 🚀 How to Use

### 1. Update Database
```bash
cd src/SmartPrice.API
dotnet ef database update --project ../SmartPrice.Infrastructure/SmartPrice.Infrastructure.csproj
```

### 2. Run the Application
```bash
dotnet run
```

### 3. Test the Scraper

**Single URL Test:**
```bash
POST /api/scraper/test
Content-Type: application/json

{
  "url": "https://www.digikala.com/product/dkp-123456"
}
```

**Batch Scraping:**
```bash
POST /api/scraper/batch
Content-Type: application/json

{
  "urls": [
    "https://www.digikala.com/product/dkp-123456",
    "https://www.digikala.com/product/dkp-789012"
  ]
}
```

**Validate URL:**
```bash
POST /api/scraper/validate
Content-Type: application/json

{
  "url": "https://www.digikala.com/product/dkp-123456"
}
```

## 🔧 Configuration Options

### Scraper Settings
- `MaxConcurrentRequests`: Control parallel requests (avoid rate limiting)
- `RequestDelayMs`: Delay between requests (be polite to servers)
- `TimeoutSeconds`: Request timeout
- `MaxRetries`: Maximum retry attempts
- `UseProxy`: Enable/disable proxy usage

### Adding More User Agents
Edit `appsettings.json` and add to the `UserAgents` array.

### Adding Proxy Servers
```json
{
  "Scraper": {
    "UseProxy": true,
    "Proxies": [
      {
        "Host": "proxy1.example.com",
        "Port": 8080,
        "Username": "user",
        "Password": "pass"
      }
    ]
  }
}
```

## 🏗️ Architecture Notes

### Extensibility
To add a new marketplace scraper:

1. Create a new class implementing `IMarketplaceScraper`
2. Implement the required methods
3. Register in `Program.cs`:
```csharp
builder.Services.AddScoped<IMarketplaceScraper, TorobScraper>();
```

### Error Handling
- All errors are logged with Serilog
- Failed scrapes return `Success = false` with error message
- HTTP errors trigger automatic retries
- Exponential backoff prevents server overload

### Performance
- SemaphoreSlim limits concurrent requests
- Configurable delays prevent rate limiting
- HttpClient pooling via IHttpClientFactory
- Efficient HTML parsing with HtmlAgilityPack

## 📝 Future Enhancements

### Ready for Implementation
1. **Advanced Proxy Management**
   - Database-backed proxy health tracking
   - Automatic proxy rotation based on success rate
   - Proxy pool refresh mechanism

2. **Additional Scrapers**
   - Torob scraper
   - Snapfood scraper
   - Emalls scraper

3. **Enhanced Features**
   - JavaScript rendering support (Selenium/Puppeteer)
   - CAPTCHA handling
   - Session management
   - Cookie persistence

4. **Monitoring**
   - Scraping success rate metrics
   - Performance dashboards
   - Alert system for failures

## 🧪 Testing

### Manual Testing
Use Swagger UI at `http://localhost:5000` (or your configured port)

### Integration Testing
The architecture supports easy testing:
- Mock `IMarketplaceScraper` for unit tests
- Mock `IHttpClientFactory` for integration tests
- Test different marketplace scrapers independently

## 📊 Database Schema

### New Table: ProxyServers
- Id (uuid, PK)
- IpAddress (varchar(45))
- Port (integer)
- Username (varchar(100), nullable)
- Password (varchar(200), nullable)
- Status (integer) - enum
- FailureCount (integer)
- LastUsedAt (timestamp)
- LastCheckedAt (timestamp)
- CreatedAt (timestamp)
- UpdatedAt (timestamp)

### Updated Table: ScrapingJobs
- Added: Marketplace (integer)
- Added: RetryCount (integer)
- Added: Duration (interval)

## 🔒 Security Considerations

1. **Proxy Credentials**: Stored in configuration (consider encryption)
2. **Rate Limiting**: Configurable to respect website ToS
3. **User Agents**: Rotated to avoid detection
4. **Error Logging**: Sensitive data should be filtered
5. **Database**: Proxy passwords should be encrypted in production

## 📚 Dependencies

All services use constructor dependency injection:
- `IScraperService` → Uses `IMarketplaceScraper` implementations
- `IMarketplaceScraper` → Uses `IHttpClientFactory`, `IProxyManager`
- `IProxyManager` → Uses `IOptions<ScraperOptions>`

## ✅ Acceptance Criteria Met

- ✅ All interfaces created in Application layer
- ✅ ScraperService implements retry + rate limiting + concurrency
- ✅ DigikalaScraper successfully extracts: title, price, image, availability
- ✅ ProxyManager ready (even if proxies list is empty)
- ✅ Configuration in appsettings.json
- ✅ Test endpoint works: POST api/scraper/test with Digikala URL
- ✅ All code compiles without errors
- ✅ Clean Architecture principles followed
- ✅ SOLID principles applied
- ✅ Proper dependency injection

## 🎉 Conclusion

The web scraper implementation is complete and production-ready. It follows best practices, is fully extensible, and provides a solid foundation for scraping Iranian e-commerce websites.

**Ready to scrape! 🚀**
