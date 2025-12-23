@echo off
echo ====================================
echo SmartPrice - Database Test
echo ====================================
echo.

echo Checking Docker...
docker --version
if %errorlevel% neq 0 (
    echo ERROR: Docker not found! Please install Docker Desktop.
    pause
    exit /b 1
)
echo [OK] Docker is installed
echo.

echo Checking PostgreSQL container...
docker ps --filter "name=smartprice-postgres" --format "{{.Names}}" | findstr smartprice-postgres >nul
if %errorlevel% neq 0 (
    echo PostgreSQL container not running. Starting...
    docker-compose -f docker-compose.dev.yml up -d postgres
    echo Waiting for PostgreSQL to be ready...
    timeout /t 10 /nobreak >nul
) else (
    echo [OK] PostgreSQL is running
)
echo.

echo Testing PostgreSQL connection...
docker exec smartprice-postgres pg_isready -U postgres
if %errorlevel% neq 0 (
    echo ERROR: Cannot connect to PostgreSQL
    pause
    exit /b 1
)
echo [OK] PostgreSQL connection successful
echo.

echo Checking if database exists...
docker exec smartprice-postgres psql -U postgres -lqt | findstr smartprice >nul
if %errorlevel% neq 0 (
    echo Database 'smartprice' not found. Creating...
    docker exec smartprice-postgres psql -U postgres -c "CREATE DATABASE smartprice;"
    echo [OK] Database created
) else (
    echo [OK] Database 'smartprice' exists
)
echo.

echo ====================================
echo All checks passed!
echo ====================================
echo.
echo Ready for migration:
echo   cd src\SmartPrice.API
echo   dotnet ef database update --project ..\SmartPrice.Infrastructure
echo.
pause
