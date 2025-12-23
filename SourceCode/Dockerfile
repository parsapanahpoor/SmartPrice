# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /src

# Copy solution and project files from SourceCode
COPY ["SourceCode/src/SmartPrice.Domain/SmartPrice.Domain.csproj", "SourceCode/src/SmartPrice.Domain/"]
COPY ["SourceCode/src/SmartPrice.Application/SmartPrice.Application.csproj", "SourceCode/src/SmartPrice.Application/"]
COPY ["SourceCode/src/SmartPrice.Infrastructure/SmartPrice.Infrastructure.csproj", "SourceCode/src/SmartPrice.Infrastructure/"]
COPY ["SourceCode/src/SmartPrice.API/SmartPrice.API.csproj", "SourceCode/src/SmartPrice.API/"]

# Restore dependencies
RUN dotnet restore "SourceCode/src/SmartPrice.API/SmartPrice.API.csproj"

# Copy all source code
COPY SourceCode .

# Build project
WORKDIR "/src/src/SmartPrice.API"
RUN dotnet build "SmartPrice.API.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish

RUN dotnet publish "SmartPrice.API.csproj" \
    -c Release \
    -o /app/publish \
    --self-contained false \
    --no-build

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published files
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Docker
ENV DOTNET_EnableDiagnostics=0

# Expose ports
EXPOSE 5000 5001

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

# Run application
ENTRYPOINT ["dotnet", "SmartPrice.API.dll"]
