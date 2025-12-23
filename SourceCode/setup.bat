@echo off
echo ====================================
echo SmartPrice - Quick Setup
echo ====================================
echo.

echo Step 1: Starting Docker containers...
docker-compose -f docker-compose.dev.yml up -d
echo Waiting for services to be ready...
timeout /t 15 /nobreak >nul
echo [OK] Docker containers started
echo.

echo Step 2: Testing PostgreSQL connection...
docker exec smartprice-postgres pg_isready -U postgres
if %errorlevel% neq 0 (
    echo ERROR: PostgreSQL not ready. Please wait and try again.
    pause
    exit /b 1
)
echo [OK] PostgreSQL is ready
echo.

echo Step 3: Running database migration...
cd src\SmartPrice.API
dotnet ef database update --project ..\SmartPrice.Infrastructure
if %errorlevel% neq 0 (
    echo ERROR: Migration failed!
    echo.
    echo Troubleshooting:
    echo 1. Make sure PostgreSQL is running: docker ps
    echo 2. Check logs: docker logs smartprice-postgres
    echo 3. See MIGRATION_TROUBLESHOOTING.md for more help
    pause
    exit /b 1
)
echo [OK] Migration completed
echo.

echo ====================================
echo Setup Complete!
echo ====================================
echo.
echo Ready to run! Execute:
echo   dotnet run
echo.
echo Then open: http://localhost:5000
echo.
echo Default credentials:
echo   Username: admin
echo   Password: Admin@123
echo.
pause
