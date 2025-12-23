.PHONY: help build up down logs ps clean rebuild migrate seed restart

help:
	@echo "SmartPrice Docker Commands"
	@echo "=========================="
	@echo "make build          - Build Docker images"
	@echo "make up             - Start all containers"
	@echo "make down           - Stop all containers"
	@echo "make restart        - Restart all containers"
	@echo "make logs           - View API logs"
	@echo "make ps             - List running containers"
	@echo "make clean          - Remove all containers and volumes"
	@echo "make rebuild        - Rebuild everything from scratch"
	@echo "make migrate        - Run database migrations"
	@echo ""
	@echo "Access Points:"
	@echo "=============="
	@echo "API:       http://localhost:5000"
	@echo "Swagger:   http://localhost:5000/swagger"
	@echo "Health:    http://localhost:5000/health"
	@echo "Seq:       http://localhost:5341"
	@echo "Postgres:  localhost:5432 (postgres/admin123)"
	@echo "Redis:     localhost:6379"

build:
	@echo "Building Docker images..."
	docker-compose build

up:
	@echo "Starting containers..."
	docker-compose up -d
	@echo ""
	@echo "✅ Containers started!"
	@echo "Waiting for services to be healthy..."
	sleep 10
	@echo ""
	@echo "Services:"
	@echo "- API: http://localhost:5000/swagger"
	@echo "- Seq: http://localhost:5341"
	@echo "- Postgres: localhost:5432"
	@echo "- Redis: localhost:6379"

down:
	@echo "Stopping containers..."
	docker-compose down
	@echo "✅ Containers stopped"

logs:
	docker-compose logs -f api

logs-postgres:
	docker-compose logs -f postgres

logs-redis:
	docker-compose logs -f redis

logs-seq:
	docker-compose logs -f seq

logs-all:
	docker-compose logs -f

ps:
	docker-compose ps

clean:
	@echo "Removing all containers and volumes..."
	docker-compose down -v
	@echo "✅ Cleanup complete"

rebuild: clean build up
	@echo "✅ Rebuild complete"

migrate:
	@echo "Running migrations..."
	docker-compose exec -T api dotnet ef database update --startup-project src/SmartPrice.API
	@echo "✅ Migrations complete"

shell-api:
	docker-compose exec api sh

shell-postgres:
	docker-compose exec postgres psql -U postgres -d smartprice

shell-redis:
	docker-compose exec redis redis-cli

status:
	@echo "Container Status:"
	docker-compose ps
	@echo ""
	@echo "Health Check:"
	curl -s http://localhost:5000/health | jq . || echo "API not ready yet"

restart:
	@echo "Restarting containers..."
	docker-compose restart
	@echo "✅ Containers restarted"
