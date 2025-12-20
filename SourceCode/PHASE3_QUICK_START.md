# 🚀 Phase 3 Quick Start Guide

## Prerequisites
- Phase 1 and Phase 2 completed
- PostgreSQL running
- .NET 7 SDK installed

## Step 1: Apply Database Migration

```powershell
cd "D:\Task\BackEnd\SmartPrice\Source\SmartPrice\SourceCode\src\SmartPrice.API"
dotnet ef database update --project ..\SmartPrice.Infrastructure\SmartPrice.Infrastructure.csproj
```

Expected output:
```
Applying migration '20251221010000_AddBackgroundJobSupport'.
Done.
```

## Step 2: Start the Application

```powershell
cd "D:\Task\BackEnd\SmartPrice\Source\SmartPrice\SourceCode\src\SmartPrice.API"
dotnet run
```

Look for this log message:
```
[INFO] Scraper Background Service started. Check interval: 00:01:00
```

## Step 3: Create Your First Job

Open Swagger UI at `http://localhost:5000` or use PowerShell:

```powershell
$body = @{
    name = "Test Digikala Scraper"
    urls = @(
        "https://www.digikala.com/product/dkp-12345678"
    )
    frequency = 0  # Manual (won't auto-run)
    priority = 2   # High
    isActive = $true
    maxRetries = 3
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/jobs" `
    -Method POST `
    -Body $body `
    -ContentType "application/json"
```

**Response:**
```json
{
  "jobId": "guid-here",
  "name": "Test Digikala Scraper",
  "frequency": "Manual",
  "priority": "High",
  "isActive": true,
  "nextRunAt": null,
  "urlCount": 1
}
```

## Step 4: Execute the Job Manually

```powershell
$jobId = "paste-job-id-here"

Invoke-RestMethod -Uri "http://localhost:5000/api/jobs/$jobId/execute" `
    -Method POST `
    -ContentType "application/json"
```

## Step 5: Check Job Status

```powershell
$jobId = "paste-job-id-here"

Invoke-RestMethod -Uri "http://localhost:5000/api/jobs/$jobId" `
    -Method GET
```

**Response:**
```json
{
  "jobId": "guid",
  "name": "Test Digikala Scraper",
  "status": "Completed",
  "nextRunAt": null,
  "lastRunAt": "2024-12-21T12:34:56Z",
  "queueLength": 0,
  "totalRuns": 1,
  "successCount": 1,
  "failureCount": 0,
  "frequency": "Manual",
  "priority": "High",
  "isActive": true
}
```

## Step 6: Create an Automatic Job

**Example: Hourly Price Monitor**

```powershell
$body = @{
    name = "Hourly Digikala Monitor"
    urls = @(
        "https://www.digikala.com/product/dkp-12345678",
        "https://www.digikala.com/product/dkp-87654321"
    )
    frequency = 1  # Hourly
    priority = 1   # Normal
    isActive = $true
    maxRetries = 3
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/jobs" `
    -Method POST `
    -Body $body `
    -ContentType "application/json"
```

This job will now run automatically every hour!

## Step 7: Monitor the Background Service

Watch the logs for:

```
[INFO] Checking 1 active jobs for due execution
[INFO] Job Hourly Digikala Monitor (guid) is due for execution
[INFO] Executing scheduled job: Hourly Digikala Monitor (guid)
[INFO] Starting job execution: Hourly Digikala Monitor (guid)
[INFO] Processing 2 URLs for job Hourly Digikala Monitor
[INFO] Successfully scraped and saved product from: url1
[INFO] Successfully scraped and saved product from: url2
[INFO] Job completed: Hourly Digikala Monitor. Processed: 2, Failed: 0, Duration: 4567ms
[INFO] Job schedule updated: Hourly Digikala Monitor (guid). Next run: 2024-12-21T13:34:56Z
```

## Step 8: Verify Products in Database

```powershell
# Connect to PostgreSQL
psql -U postgres -d smartprice

# Check products
SELECT "Id", "Name", "CurrentPrice", "IsAvailable", "CreatedAt" 
FROM "Products" 
ORDER BY "CreatedAt" DESC 
LIMIT 5;

# Check price history
SELECT p."Name", ph."Price", ph."RecordedAt"
FROM "PriceHistories" ph
JOIN "Products" p ON ph."ProductId" = p."Id"
ORDER BY ph."RecordedAt" DESC
LIMIT 10;

# Check jobs
SELECT "Id", "Name", "Status", "RunCount", "SuccessCount", "FailureCount", "NextRunAt"
FROM "ScrapingJobs"
ORDER BY "CreatedAt" DESC;

# Check queue
SELECT "Id", "Url", "Status", "Priority", "ScheduledAt"
FROM "ScrapingQueues"
ORDER BY "ScheduledAt" DESC
LIMIT 10;
```

## Job Frequency Reference

```powershell
# Manual (0) - Run only when manually triggered
frequency = 0

# Hourly (1) - Every hour
frequency = 1

# Daily (2) - Once per day
frequency = 2

# Weekly (3) - Once per week
frequency = 3

# Custom (4) - Use cron expression
frequency = 4
cronExpression = "0 */6 * * *"  # Every 6 hours
```

## Priority Levels

```powershell
# Low (0)
priority = 0

# Normal (1)
priority = 1

# High (2)
priority = 2

# Critical (3)
priority = 3
```

## Common Cron Expressions

```
"*/30 * * * *"   # Every 30 minutes
"0 */6 * * *"    # Every 6 hours
"0 0 * * *"      # Daily at midnight UTC
"0 9 * * *"      # Daily at 9 AM UTC
"0 0 * * 0"      # Every Sunday at midnight
"0 0 1 * *"      # First day of every month
```

## API Endpoints Summary

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/jobs | Create new job |
| GET | /api/jobs | Get all jobs |
| GET | /api/jobs/{id} | Get job details |
| POST | /api/jobs/{id}/execute | Execute job manually |
| PATCH | /api/jobs/{id}/active | Enable/disable job |
| DELETE | /api/jobs/{id} | Delete job |

## Troubleshooting

### Issue: Background service not starting
**Check logs for:**
```
[INFO] Scraper Background Service started
```

**Solution:** Make sure the application started successfully and check for any startup errors.

### Issue: Jobs not running automatically
**Possible causes:**
1. Job `IsActive` is set to false
2. Job `Frequency` is Manual (0)
3. `NextRunAt` is in the future

**Check:**
```powershell
$jobId = "paste-job-id-here"
Invoke-RestMethod -Uri "http://localhost:5000/api/jobs/$jobId"
```

### Issue: Job execution failed
**Check job error message:**
```powershell
$jobId = "paste-job-id-here"
$job = Invoke-RestMethod -Uri "http://localhost:5000/api/jobs/$jobId"
$job.errorMessage  # If job failed
```

**Common errors:**
- Invalid URL format
- Website blocking requests
- Network timeout
- Product selectors changed

### Issue: Products not being saved
**Check logs for:**
```
[INFO] Successfully scraped and saved product from: url
[INFO] Created new product: ProductName from Digikala
```

**Or:**
```
[INFO] Updated existing product: ProductName
```

## Testing Different Job Types

### 1. Manual Job (Run on Demand)
```powershell
$body = @{
    name = "Manual Test"
    urls = @("https://www.digikala.com/product/dkp-12345678")
    frequency = 0
    priority = 1
    isActive = $true
} | ConvertTo-Json

$job = Invoke-RestMethod -Uri "http://localhost:5000/api/jobs" -Method POST -Body $body -ContentType "application/json"

# Execute it
Invoke-RestMethod -Uri "http://localhost:5000/api/jobs/$($job.jobId)/execute" -Method POST
```

### 2. Hourly Job (Automatic)
```powershell
$body = @{
    name = "Hourly Monitor"
    urls = @("https://www.digikala.com/product/dkp-12345678")
    frequency = 1
    priority = 1
    isActive = $true
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/jobs" -Method POST -Body $body -ContentType "application/json"

# Will run automatically every hour
```

### 3. Daily Job
```powershell
$body = @{
    name = "Daily Price Check"
    urls = @("https://www.digikala.com/product/dkp-12345678")
    frequency = 2
    priority = 1
    isActive = $true
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/jobs" -Method POST -Body $body -ContentType "application/json"
```

### 4. Custom Cron Job
```powershell
$body = @{
    name = "Every 6 Hours"
    urls = @("https://www.digikala.com/product/dkp-12345678")
    frequency = 4
    cronExpression = "0 */6 * * *"
    priority = 1
    isActive = $true
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/jobs" -Method POST -Body $body -ContentType "application/json"
```

## Next Steps

1. ✅ **Create test jobs** with real Digikala URLs
2. ✅ **Monitor logs** to see automatic execution
3. ✅ **Check database** to verify products are saved
4. ✅ **Test manual execution** for immediate results
5. ✅ **Set up hourly/daily jobs** for ongoing monitoring
6. ✅ **Track price history** over multiple executions

## Success Indicators

You know Phase 3 is working when you see:

1. ✅ Background service starts with application
2. ✅ Jobs created via API appear in database
3. ✅ Manual execution works immediately
4. ✅ Scheduled jobs run automatically
5. ✅ Products saved to database
6. ✅ Price history records created
7. ✅ Job statistics updated (RunCount, SuccessCount)
8. ✅ NextRunAt calculated automatically
9. ✅ Logs show successful scraping
10. ✅ No error messages in logs

---

**Phase 3 is now fully operational! 🎉**

The SmartPrice application can now automatically monitor prices across multiple marketplaces with flexible scheduling options!
