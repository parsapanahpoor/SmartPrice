#!/bin/bash
# Docker entrypoint script for PostgreSQL initialization

set -e

# Wait for PostgreSQL to be ready
echo "Waiting for PostgreSQL to be ready..."
until PGPASSWORD=$POSTGRES_PASSWORD psql -h localhost -U $POSTGRES_USER -d $POSTGRES_DB -c '\l' > /dev/null 2>&1; do
  echo "Postgres is unavailable - sleeping"
  sleep 1
done

echo "PostgreSQL is ready!"
echo "Database initialization complete"
