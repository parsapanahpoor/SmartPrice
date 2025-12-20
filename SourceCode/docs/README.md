# SmartPrice - Price Tracking & Comparison System

## Overview
SmartPrice is a professional ASP.NET Core application for price tracking and comparison in Iranian markets. Built with Clean Architecture principles, it provides a scalable and maintainable solution for monitoring product prices across multiple sources.

## Architecture
The project follows Clean Architecture with the following layers:

### 1. Domain Layer (`SmartPrice.Domain`)
- Core business entities
- Enums and value objects
- No external dependencies

**Entities:**
- `Product`: Represents tracked products
- `PriceHistory`: Historical price records
- `TelegramChannel`: Telegram channels for notifications
- `ScrapingJob`: Web scraping job records

### 2. Application Layer (`SmartPrice.Application`)
- Business logic and use cases
- Interfaces and contracts
- DTOs for data transfer

**Key Interfaces:**
- `IProductRepository`: Product data access
- `IScraper`: Web scraping functionality
- `ITelegramService`: Telegram bot integration

### 3. Infrastructure Layer (`SmartPrice.Infrastructure`)
- External concerns implementation
- Database context and configurations
- Repository implementations

**Technologies:**
- PostgreSQL with Entity Framework Core
- Redis for caching
- AngleSharp for web scraping
- Telegram.Bot for notifications

### 4. API Layer (`SmartPrice.API`)
- RESTful Web API
- Background services
- Health checks and monitoring

## Tech Stack
- **.NET 8.0**: Latest LTS version
- **PostgreSQL**: Primary database
- **Redis**: Distributed caching
- **RabbitMQ**: Message queue (future)
- **Serilog**: Structured logging
- **Seq**: Log aggregation
- **Swagger**: API documentation

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- PostgreSQL 14+
- Redis 7+
- Visual Studio 2022 or JetBrains Rider

### Configuration
Update `appsettings.json` with your connection strings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=smartprice;Username=postgres;Password=your_password"
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "Telegram": {
    "BotToken": "YOUR_BOT_TOKEN",
    "ChannelId": "@your_channel"
  }
}
```

### Database Migration
```bash
cd src/SmartPrice.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../SmartPrice.API
dotnet ef database update --startup-project ../SmartPrice.API
```

### Running the Application
```bash
cd src/SmartPrice.API
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

## API Endpoints

### Products
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create new product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product

### Health Check
- `GET /health` - Application health status

## Project Structure
```
SmartPrice/
??? src/
?   ??? SmartPrice.Domain/           # Domain entities
?   ??? SmartPrice.Application/      # Business logic
?   ??? SmartPrice.Infrastructure/   # Data access
?   ??? SmartPrice.API/              # Web API
??? tests/
?   ??? SmartPrice.UnitTests/
?   ??? SmartPrice.IntegrationTests/
??? docs/                            # Documentation
```

## Development Guidelines

### Code Style
- Follow C# coding conventions
- Use nullable reference types
- Enable ImplicitUsings
- XML documentation for public APIs

### SOLID Principles
- Single Responsibility
- Open/Closed
- Liskov Substitution
- Interface Segregation
- Dependency Inversion

## Testing
```bash
# Run all tests
dotnet test

# Run unit tests only
dotnet test tests/SmartPrice.UnitTests

# Run integration tests only
dotnet test tests/SmartPrice.IntegrationTests
```

## Logging
Logs are written to:
- Console (Development)
- Seq (http://localhost:5341)

## Monitoring
Health checks available at `/health` endpoint, monitoring:
- PostgreSQL database connection
- Redis cache connection

## Future Phases
- **Phase 2**: Web scraping implementation
- **Phase 3**: Telegram bot integration
- **Phase 4**: Background job scheduling
- **Phase 5**: Admin dashboard
- **Phase 6**: Performance optimization

## License
Private project - All rights reserved

## Support
For issues and questions, contact: support@smartprice.ir
