# 🚀 Quick Start Guide - SmartPrice Web Scraper

## Prerequisites
- .NET 7 SDK installed
- PostgreSQL running (connection string in appsettings.json)
- (Optional) Redis running for caching
- (Optional) Seq running for log aggregation

## Step-by-Step Setup

### 1. Apply Database Migration
```powershell
cd "D:\Task\BackEnd\SmartPrice\Source\SmartPrice\SourceCode\src\SmartPrice.API"
dotnet ef database update --project ..\SmartPrice.Infrastructure\SmartPrice.Infrastructure.csproj
```

### 2. Run the Application
```powershell
cd "D:\Task\BackEnd\SmartPrice\Source\SmartPrice\SourceCode\src\SmartPrice.API"
dotnet run
```

The API will start at: `https://localhost:5001` or `http://localhost:5000`

### 3. Open Swagger UI
Navigate to: `http://localhost:5000` (or your configured port)

You should see the Swagger documentation with three new endpoints under **Scraper**:
- POST /api/scraper/test
- POST /api/scraper/batch
- POST /api/scraper/validate

## Testing the Scraper

### Option 1: Using Swagger UI

1. Click on **POST /api/scraper/test**
2. Click "Try it out"
3. Replace the request body with:
```json
{
  "url": "https://www.digikala.com/product/dkp-12345678"
}
```
4. Click "Execute"

### Option 2: Using PowerShell
```powershell
# Test single URL
$body = @{
    url = "https://www.digikala.com/product/dkp-12345678"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/scraper/test" `
    -Method POST `
    -Body $body `
    -ContentType "application/json"

# Validate URL
$body = @{
    url = "https://www.digikala.com/product/dkp-12345678"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/scraper/validate" `
    -Method POST `
    -Body $body `
    -ContentType "application/json"

# Batch scraping
$body = @{
    urls = @(
        "https://www.digikala.com/product/dkp-12345678",
        "https://www.digikala.com/product/dkp-87654321"
    )
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/scraper/batch" `
    -Method POST `
    -Body $body `
    -ContentType "application/json"
```

### Option 3: Using cURL (Git Bash or WSL)
```bash
# Test single URL
curl -X POST http://localhost:5000/api/scraper/test \
  -H "Content-Type: application/json" \
  -d '{"url":"https://www.digikala.com/product/dkp-12345678"}'

# Validate URL
curl -X POST http://localhost:5000/api/scraper/validate \
  -H "Content-Type: application/json" \
  -d '{"url":"https://www.digikala.com/product/dkp-12345678"}'

# Batch scraping
curl -X POST http://localhost:5000/api/scraper/batch \
  -H "Content-Type: application/json" \
  -d '{"urls":["https://www.digikala.com/product/dkp-12345678","https://www.digikala.com/product/dkp-87654321"]}'
```

## Expected Responses

### Successful Scrape
```json
{
  "success": true,
  "url": "https://www.digikala.com/product/dkp-12345678",
  "product": {
    "title": "Product Name",
    "price": 1250000,
    "imageUrl": "https://dkstatics-public.digikala.com/...",
    "isAvailable": true,
    "sku": "12345678",
    "metadata": {
      "Source": "Digikala",
      "ScrapedAt": "2024-12-21T12:34:56.789Z"
    }
  },
  "errorMessage": null,
  "duration": "00:00:02.5",
  "marketplace": 0
}
```

### Failed Scrape
```json
{
  "success": false,
  "url": "https://www.digikala.com/product/dkp-invalid",
  "product": null,
  "errorMessage": "Response status code does not indicate success: 404 (Not Found).",
  "duration": "00:00:01.2",
  "marketplace": 0
}
```

### Validation Response
```json
{
  "valid": true,
  "marketplace": "Digikala",
  "message": "URL is valid"
}
```

## Troubleshooting

### Issue: Database connection error
**Solution**: Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=smartprice;Username=postgres;Password=your_actual_password"
  }
}
```

### Issue: Redis connection error
**Solution**: Either:
1. Start Redis service, OR
2. Comment out Redis health check in `Program.cs` (lines 44-47)

### Issue: Seq connection error
**Solution**: Either:
1. Start Seq service, OR
2. Remove `.WriteTo.Seq()` from Program.cs (line 15)

### Issue: Scraping returns empty data
**Possible causes**:
1. **Website structure changed** - Update selectors in `DigikalaScraper.cs`
2. **Website blocking requests** - Enable proxy support in `appsettings.json`
3. **Rate limiting** - Increase `RequestDelayMs` in configuration
4. **Invalid URL** - Make sure URL is a valid Digikala product page

### Issue: Timeout errors
**Solution**: Increase timeout in `appsettings.json`:
```json
{
  "Scraper": {
    "TimeoutSeconds": 60
  }
}
```

## Configuration Tips

### For Development
```json
{
  "Scraper": {
    "MaxConcurrentRequests": 3,
    "RequestDelayMs": 3000,
    "TimeoutSeconds": 30,
    "MaxRetries": 3,
    "UseProxy": false
  }
}
```

### For Production
```json
{
  "Scraper": {
    "MaxConcurrentRequests": 10,
    "RequestDelayMs": 1000,
    "TimeoutSeconds": 45,
    "MaxRetries": 5,
    "UseProxy": true,
    "Proxies": [
      {
        "Host": "your-proxy.com",
        "Port": 8080,
        "Username": "user",
        "Password": "pass"
      }
    ]
  }
}
```

## Monitoring

### Check Logs
Logs are written to:
1. **Console** - See terminal output
2. **Seq** (if running) - Navigate to `http://localhost:5341`

### Sample Log Messages
```
[12:34:56 INF] Starting scrape for https://www.digikala.com/product/dkp-12345678 using Digikala
[12:34:58 INF] Successfully scraped https://www.digikala.com/product/dkp-12345678 in 2345ms
[12:34:59 INF] Batch scrape completed: 2/2 successful
```

## Next Steps

1. **Test with real Digikala URLs** to verify selectors still work
2. **Implement additional scrapers** for Torob, Snapfood, etc.
3. **Enable proxy rotation** if scraping at scale
4. **Set up monitoring** with Seq for production use
5. **Create scheduled jobs** for automatic price updates

## Support

For detailed documentation, see:
- `SCRAPER_IMPLEMENTATION.md` - Full implementation details
- `IMPLEMENTATION_SUMMARY.md` - Feature summary
- Swagger UI at `http://localhost:5000` - API documentation

---

**Happy Scraping! 🚀**
