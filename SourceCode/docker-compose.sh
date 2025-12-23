#!/bin/bash

# SmartPrice Docker Management Script

set -e

# رنگ‌ها
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# تابع برای نشان دادن پیام‌ها
success() {
    echo -e "${GREEN}✅ $1${NC}"
}

error() {
    echo -e "${RED}❌ $1${NC}"
}

info() {
    echo -e "${BLUE}ℹ️ $1${NC}"
}

warning() {
    echo -e "${YELLOW}⚠️ $1${NC}"
}

# نمایش کمک
show_help() {
    cat << EOF
╔════════════════════════════════════════════════════════════════╗
║           SmartPrice Docker Management Script                  ║
║                                                                ║
║  دستورات:                                                      ║
║  ─────────────────────────────────────────────────────────────║
║  ./docker-compose.sh build          - ساخت Docker images     ║
║  ./docker-compose.sh up             - اجرای سرویس‌ها          ║
║  ./docker-compose.sh down           - متوقف کردن              ║
║  ./docker-compose.sh restart        - Restart                 ║
║  ./docker-compose.sh logs           - دیدن لاگ‌ها              ║
║  ./docker-compose.sh ps             - لیست containers        ║
║  ./docker-compose.sh status         - بررسی وضعیت            ║
║  ./docker-compose.sh migrate        - Database migration     ║
║  ./docker-compose.sh clean          - حذف کامل               ║
║  ./docker-compose.sh rebuild        - ساخت از صفر            ║
║  ./docker-compose.sh shell-api      - ورود به API           ║
║  ./docker-compose.sh shell-postgres - ورود به PostgreSQL    ║
║                                                                ║
║  دسترسی‌های سریع:                                               ║
║  ─────────────────────────────────────────────────────────────║
║  API Swagger:   http://localhost:5000/swagger                ║
║  Health Check:  http://localhost:5000/health                 ║
║  Seq Logs:      http://localhost:5341                        ║
║  PostgreSQL:    localhost:5432 (postgres/admin123)          ║
║  Redis:         localhost:6379                               ║
╚════════════════════════════════════════════════════════════════╝
EOF
}

# تابع برای بررسی Docker
check_docker() {
    if ! command -v docker &> /dev/null; then
        error "Docker نصب نشده یا در مسیر نیست"
        error "برای نصب Docker: https://docs.docker.com/install/"
        exit 1
    fi
    
    if ! command -v docker-compose &> /dev/null; then
        error "docker-compose نصب نشده یا در مسیر نیست"
        error "برای نصب: https://docs.docker.com/compose/install/"
        exit 1
    fi
    
    success "Docker موجود است: $(docker --version)"
}

# دستورات
case "${1:-help}" in
    build)
        check_docker
        info "📦 در حال ساخت Docker images..."
        docker-compose build
        success "Images با موفقیت ساخته شدند"
        ;;

    up)
        check_docker
        info "🚀 در حال اجرای سرویس‌ها..."
        docker-compose up -d
        success "سرویس‌ها اجرا شدند!"
        info "منتظر بمانید 30 ثانیه برای آماده شدن سرویس‌ها..."
        sleep 5
        "$0" status
        ;;

    down)
        check_docker
        info "⏹️ در حال متوقف کردن سرویس‌ها..."
        docker-compose down
        success "سرویس‌ها متوقف شدند"
        ;;

    restart)
        check_docker
        info "🔄 در حال restart سرویس‌ها..."
        docker-compose restart
        success "سرویس‌ها restart شدند"
        ;;

    logs)
        check_docker
        info "📋 نمایش لاگ‌های API (برای خروج: Ctrl+C)..."
        docker-compose logs -f api
        ;;

    logs-all)
        check_docker
        info "📋 نمایش تمام لاگ‌ها (برای خروج: Ctrl+C)..."
        docker-compose logs -f
        ;;

    ps)
        check_docker
        info "📊 لیست containers:"
        docker-compose ps
        ;;

    status)
        check_docker
        info "🔍 بررسی وضعیت سرویس‌ها..."
        echo ""
        docker-compose ps
        echo ""
        info "بررسی Health Check..."
        if curl -s http://localhost:5000/health > /dev/null; then
            success "API سالم است"
        else
            warning "API هنوز آماده نیست یا در حال اجرا نیست"
        fi
        ;;

    migrate)
        check_docker
        info "🗄️ در حال اجرای migrations..."
        docker-compose exec -T api dotnet ef database update --startup-project src/SmartPrice.API
        success "Migrations تکمیل شدند"
        ;;

    clean)
        check_docker
        warning "این عمل تمام containers و volumes را حذف خواهد کرد"
        read -p "آیا مطمئن هستید؟ (yes/no): " confirmation
        if [ "$confirmation" = "yes" ]; then
            info "پاکسازی..."
            docker-compose down -v
            success "پاکسازی تکمیل شد"
        else
            info "عمل لغو شد"
        fi
        ;;

    rebuild)
        check_docker
        warning "این عمل تمام چیز را حذف و دوباره ساخت خواهد کرد"
        read -p "آیا مطمئن هستید؟ (yes/no): " confirmation
        if [ "$confirmation" = "yes" ]; then
            info "حذف قدیمی..."
            docker-compose down -v
            info "ساخت دوباره..."
            docker-compose build
            info "اجرای سرویس‌ها..."
            docker-compose up -d
            success "Rebuild تکمیل شد!"
        else
            info "عمل لغو شد"
        fi
        ;;

    shell-api)
        check_docker
        info "ورود به API container..."
        docker-compose exec api sh
        ;;

    shell-postgres)
        check_docker
        info "ورود به PostgreSQL..."
        docker-compose exec postgres psql -U postgres -d smartprice
        ;;

    shell-redis)
        check_docker
        info "ورود به Redis..."
        docker-compose exec redis redis-cli
        ;;

    help|--help|-h)
        show_help
        ;;

    *)
        error "دستور نامعتبر: $1"
        show_help
        exit 1
        ;;
esac

exit 0
