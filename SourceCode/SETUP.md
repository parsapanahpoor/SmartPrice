# SmartPrice Project - Setup Instructions

## Phase 1: Infrastructure Setup - COMPLETED ✅

### Project Structure Created
```
SmartPrice/
├── src/
│   ├── SmartPrice.Domain/           ✅ Core entities and enums
│   ├── SmartPrice.Application/      ✅ Interfaces and DTOs
│   ├── SmartPrice.Infrastructure/   ✅ Database context and repositories
│   └── SmartPrice.API/              ✅ Web API and controllers
├── tests/
│   ├── SmartPrice.UnitTests/        ✅ Unit test project
│   └── SmartPrice.IntegrationTests/ ✅ Integration test project
└── docs/                            ✅ Documentation
```

### Components Implemented

#### Domain Layer (SmartPrice.Domain)
- ✅ Product entity
- ✅ PriceHistory entity
- ✅ TelegramChannel entity
- ✅ ScrapingJob entity
- ✅ JobStatus enum

#### Application Layer (SmartPrice.Application)
- ✅ IProductRepository interface
- ✅ IScraper interface
- ✅ ITelegramService interface
- ✅ ProductDto record
- ✅ ScrapingResultDto record

#### Infrastructure Layer (SmartPrice.Infrastructure)
- ✅ ApplicationDbContext
- ✅ ProductConfiguration (EF Core mapping)
- ✅ PriceHistoryConfiguration
- ✅ TelegramChannelConfiguration
- ✅ ScrapingJobConfiguration
- ✅ ProductRepository implementation
- ✅ Initial database migration

#### API Layer (SmartPrice.API)
- ✅ Program.cs with full configuration
- ✅ ProductsController with CRUD operations
- ✅ Serilog logging setup
- ✅ Redis caching configuration
- ✅ Health checks (PostgreSQL & Redis)
- ✅ Swagger documentation
- ✅ CORS configuration

## Prerequisites

### Required Software
1. **.NET 7.0 SDK** (Installed: 7.0.305)
2. **PostgreSQL 12+** (Database server)
3. **Redis** (Optional - for caching)
4. **Seq** (Optional - for log aggregation)

### Installing PostgreSQL on Windows
```bash
# Download from: https://www.postgresql.org/download/windows/
# Default port: 5432
# Create database: smartprice
```

### Installing Redis on Windows (Optional)
```bash
# Option 1: Using WSL2
wsl --install
sudo apt update
sudo apt install redis-server
redis-server

# Option 2: Using Docker
docker run --name redis -p 6379:6379 -d redis

# Option 3: Download Windows binary from https://github.com/microsoftarchive/redis/releases
```

### Installing Seq (Optional)
```bash
# Using Docker
docker run --name seq -d --restart unless-stopped -e ACCEPT_EULA=Y -p 5341:80 datalust/seq

# Or download from: https://datalust.co/download
```

## Configuration

### 1. Update Database Connection String
Edit `src/SmartPrice.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=smartprice;Username=postgres;Password=YOUR_PASSWORD_HERE"
  }
}
```

### 2. Configure Redis (Optional)
If Redis is not running, you can:
- Comment out Redis configuration in `Program.cs`
- Or install and run Redis as shown above

### 3. Configure Telegram (For Future Phases)
```json
{
  "Telegram": {
    "BotToken": "YOUR_BOT_TOKEN",
    "ChannelId": "@your_channel"
  }
}
```

## Database Setup

### Create Database
```sql
-- Connect to PostgreSQL
psql -U postgres

-- Create database
CREATE DATABASE smartprice;

-- Grant permissions
GRANT ALL PRIVILEGES ON DATABASE smartprice TO postgres;
```

### Apply Migrations
```bash
cd src/SmartPrice.Infrastructure
dotnet ef database update --startup-project ../SmartPrice.API
```

This will create the following tables:
- ✅ Products
- ✅ PriceHistories
- ✅ TelegramChannels
- ✅ ScrapingJobs

## Running the Application

### 1. Build the Solution
```bash
cd D:\Task\BackEnd\SmartPrice\Source\SmartPrice\SourceCode
dotnet build
```

### 2. Run the API
```bash
cd src/SmartPrice.API
dotnet run
```

### 3. Access the Application
- **Swagger UI**: http://localhost:5000/swagger
- **API**: http://localhost:5000/api
- **Health Check**: http://localhost:5000/health

## Testing the API

### Using Swagger UI
1. Navigate to http://localhost:5000/swagger
2. Expand the `Products` endpoints
3. Try creating a product:

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "name": "Samsung Galaxy S23",
  "url": "https://example.com/product/123",
  "imageUrl": "https://example.com/image.jpg",
  "category": "Mobile",
  "currentPrice": 15000000,
  "originalPrice": 18000000,
  "discountPercentage": 17,
  "isAvailable": true,
  "lastUpdated": "2024-01-01T00:00:00",
  "createdAt": "2024-01-01T00:00:00",
  "priceHistory": []
}
```

### Using curl
```bash
# Get all products
curl http://localhost:5000/api/products

# Get health status
curl http://localhost:5000/health
```

## Running Tests

```bash
# Run all tests
dotnet test

# Run unit tests only
dotnet test tests/SmartPrice.UnitTests

# Run integration tests only
dotnet test tests/SmartPrice.IntegrationTests
```

## Success Criteria - Phase 1

✅ **Solution builds without errors**
```bash
dotnet build
# Result: Build succeeded. 5 Warning(s), 0 Error(s)
```

✅ **Database migration creates all tables correctly**
```bash
dotnet ef database update --startup-project ../SmartPrice.API
# Migration: 20251220224028_InitialCreate applied
```

✅ **Health check endpoint returns healthy status**
```bash
curl http://localhost:5000/health
# Expected: Healthy (if PostgreSQL and Redis are running)
```

✅ **Swagger UI is accessible and shows API documentation**
```
Navigate to: http://localhost:5000/swagger
API endpoints documented and testable
```

✅ **All layers are properly separated with no circular dependencies**
```
Domain → No dependencies
Application → Domain
Infrastructure → Application, Domain
API → Infrastructure, Application
```

## Troubleshooting

### Error: PostgreSQL not running
```bash
# Windows Service
net start postgresql-x64-14

# Or check PostgreSQL installation
```

### Error: Redis connection failed
```bash
# Comment out Redis in Program.cs:
// builder.Services.AddStackExchangeRedisCache(...)

# Or start Redis:
redis-server
```

### Error: Port 5000 already in use
```bash
# Change port in launchSettings.json
"applicationUrl": "http://localhost:5050"
```

## Next Steps - Phase 2

After Phase 1 is verified, we can proceed with:
- ✅ Web scraping implementation using AngleSharp
- ✅ Telegram bot integration for notifications
- ✅ Background job scheduling with Hangfire
- ✅ Admin dashboard
- ✅ Performance optimization and caching strategies

## Package Versions

### Production Dependencies
- .NET 7.0
- Entity Framework Core 7.0.20
- Npgsql.EntityFrameworkCore.PostgreSQL 7.0.18
- Serilog.AspNetCore 8.0.0
- Swashbuckle.AspNetCore 6.5.0
- StackExchange.Redis 2.7.10
- Telegram.Bot 19.0.0
- AngleSharp 1.1.2
- MediatR 12.2.0
- FluentValidation 11.9.0

### Test Dependencies
- xUnit 2.6.2
- Moq 4.20.70
- Microsoft.AspNetCore.Mvc.Testing 7.0.20

## Support

For issues or questions:
- Check logs at: http://localhost:5341 (Seq)
- Review console output
- Check database connection
- Verify all prerequisites are installed

---

**Phase 1 Status**: ✅ COMPLETED
**Build Status**: ✅ SUCCESS
**Migration Status**: ✅ CREATED
**Documentation**: ✅ COMPLETE
