#!/usr/bin/env pwsh

<#
.SYNOPSIS
    SmartPrice Docker Management Script for Windows PowerShell

.DESCRIPTION
    راهنمای استفاده:
    .\docker-compose.ps1 -Command build     # ساخت images
    .\docker-compose.ps1 -Command up        # اجرا
    .\docker-compose.ps1 -Command down      # متوقف
    .\docker-compose.ps1 -Command logs      # دیدن لاگ‌ها
    .\docker-compose.ps1 -Command migrate   # اجرای migrations
#>

param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("build", "up", "down", "logs", "ps", "restart", "clean", "rebuild", "migrate", "status", "shell-api", "shell-postgres")]
    [string]$Command
)

$ErrorActionPreference = "Stop"

# رنگ‌ها
$colors = @{
    Success = 'Green'
    Error = 'Red'
    Info = 'Cyan'
    Warning = 'Yellow'
}

function Write-Success {
    param([string]$Message)
    Write-Host "✅ $Message" -ForegroundColor $colors.Success
}

function Write-Error-Custom {
    param([string]$Message)
    Write-Host "❌ $Message" -ForegroundColor $colors.Error
}

function Write-Info {
    param([string]$Message)
    Write-Host "ℹ️ $Message" -ForegroundColor $colors.Info
}

function Write-Warning-Custom {
    param([string]$Message)
    Write-Host "⚠️ $Message" -ForegroundColor $colors.Warning
}

# تابع برای بررسی Docker
function Test-Docker {
    try {
        $version = docker --version
        Write-Success "Docker موجود: $version"
        return $true
    }
    catch {
        Write-Error-Custom "Docker Desktop نصب نشده یا اجرا نمی‌شود!"
        Write-Info "برای نصب Docker Desktop: https://www.docker.com/products/docker-desktop/"
        return $false
    }
}

# تابع برای نمایش کمک
function Show-Help {
    Write-Host @"
╔════════════════════════════════════════════════════════════════╗
║           SmartPrice Docker Management Script                  ║
║                                                                ║
║  دستورات:                                                      ║
║  ─────────────────────────────────────────────────────────────║
║  build          - ساخت Docker images                          ║
║  up             - اجرای تمام سرویس‌ها                           ║
║  down           - متوقف کردن سرویس‌ها                           ║
║  restart        - Restart سرویس‌ها                              ║
║  logs           - دیدن لاگ‌های API                              ║
║  ps             - لیست containers                             ║
║  status         - وضعیت سلامت سرویس‌ها                          ║
║  migrate        - اجرای database migrations                   ║
║  clean          - حذف تمام containers و volumes              ║
║  rebuild        - ساخت از صفر (clean + build + up)            ║
║  shell-api      - ورود به API container                       ║
║  shell-postgres - ورود به PostgreSQL                          ║
║                                                                ║
║  دسترسی‌های سریع:                                               ║
║  ─────────────────────────────────────────────────────────────║
║  API Swagger:   http://localhost:5000/swagger                ║
║  Health Check:  http://localhost:5000/health                 ║
║  Seq Logs:      http://localhost:5341                        ║
║  PostgreSQL:    localhost:5432 (user: postgres/admin123)    ║
║  Redis:         localhost:6379                               ║
║                                                                ║
║  مثال‌های استفاده:                                             ║
║  ─────────────────────────────────────────────────────────────║
║  .\docker-compose.ps1 -Command up                            ║
║  .\docker-compose.ps1 -Command logs                          ║
║  .\docker-compose.ps1 -Command migrate                       ║
║  .\docker-compose.ps1 -Command rebuild                       ║
╚════════════════════════════════════════════════════════════════╝
"@
}

# دستورات
switch ($Command) {
    "build" {
        if (-not (Test-Docker)) { exit 1 }
        Write-Info "📦 در حال ساخت Docker images..."
        docker-compose build
        Write-Success "Images با موفقیت ساخته شدند"
    }

    "up" {
        if (-not (Test-Docker)) { exit 1 }
        Write-Info "🚀 در حال اجرای سرویس‌ها..."
        docker-compose up -d
        Write-Success "سرویس‌ها اجرا شدند!"
        Write-Info "منتظر بمانید 30 ثانیه برای آماده شدن سرویس‌ها..."
        Start-Sleep -Seconds 5
        & $PSScriptRoot\docker-compose.ps1 -Command status
    }

    "down" {
        if (-not (Test-Docker)) { exit 1 }
        Write-Info "⏹️ در حال متوقف کردن سرویس‌ها..."
        docker-compose down
        Write-Success "سرویس‌ها متوقف شدند"
    }

    "restart" {
        if (-not (Test-Docker)) { exit 1 }
        Write-Info "🔄 در حال restart سرویس‌ها..."
        docker-compose restart
        Write-Success "سرویس‌ها restart شدند"
    }

    "logs" {
        if (-not (Test-Docker)) { exit 1 }
        Write-Info "📋 نمایش لاگ‌های API..."
        docker-compose logs -f api
    }

    "ps" {
        if (-not (Test-Docker)) { exit 1 }
        Write-Info "📊 لیست containers:"
        docker-compose ps
    }

    "status" {
        if (-not (Test-Docker)) { exit 1 }
        Write-Info "🔍 بررسی وضعیت سرویس‌ها..."
        Write-Host ""
        docker-compose ps
        Write-Host ""
        Write-Info "بررسی Health Check..."
        try {
            $health = Invoke-RestMethod -Uri "http://localhost:5000/health" -ErrorAction SilentlyContinue
            Write-Success "API Health: $($health.status)"
        }
        catch {
            Write-Warning-Custom "API هنوز آماده نیست یا در حال اجرا نیست"
        }
    }

    "migrate" {
        if (-not (Test-Docker)) { exit 1 }
        Write-Info "🗄️ در حال اجرای migrations..."
        docker-compose exec -T api dotnet ef database update --startup-project src/SmartPrice.API
        Write-Success "Migrations تکمیل شدند"
    }

    "clean" {
        if (-not (Test-Docker)) { exit 1 }
        Write-Warning-Custom "این عمل تمام containers و volumes را حذف خواهد کرد"
        $confirmation = Read-Host "آیا مطمئن هستید؟ (yes/no)"
        if ($confirmation -eq "yes") {
            Write-Info "پاکسازی..."
            docker-compose down -v
            Write-Success "پاکسازی تکمیل شد"
        }
        else {
            Write-Info "عمل لغو شد"
        }
    }

    "rebuild" {
        if (-not (Test-Docker)) { exit 1 }
        Write-Warning-Custom "این عمل تمام چیز را حذف و دوباره ساخت خواهد کرد"
        $confirmation = Read-Host "آیا مطمئن هستید؟ (yes/no)"
        if ($confirmation -eq "yes") {
            Write-Info "حذف قدیمی..."
            docker-compose down -v
            Write-Info "ساخت دوباره..."
            docker-compose build
            Write-Info "اجرای سرویس‌ها..."
            docker-compose up -d
            Write-Success "Rebuild تکمیل شد!"
        }
        else {
            Write-Info "عمل لغو شد"
        }
    }

    "shell-api" {
        if (-not (Test-Docker)) { exit 1 }
        Write-Info "ورود به API container..."
        docker-compose exec api sh
    }

    "shell-postgres" {
        if (-not (Test-Docker)) { exit 1 }
        Write-Info "ورود به PostgreSQL..."
        docker-compose exec postgres psql -U postgres -d smartprice
    }

    default {
        Write-Error-Custom "دستور نامعتبر: $Command"
        Show-Help
        exit 1
    }
}

exit 0
