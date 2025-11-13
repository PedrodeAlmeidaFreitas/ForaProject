#!/bin/bash

# ForaProject - Docker Development Helper Script

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

print_info() {
    echo -e "${YELLOW}ℹ $1${NC}"
}

# Function to check if Docker is running
check_docker() {
    if ! docker info > /dev/null 2>&1; then
        print_error "Docker is not running. Please start Docker and try again."
        exit 1
    fi
    print_success "Docker is running"
}

# Function to start services
start_services() {
    print_info "Starting services..."
    docker compose up -d
    print_success "Services started"
}

# Function to stop services
stop_services() {
    print_info "Stopping services..."
    docker compose down
    print_success "Services stopped"
}

# Function to view logs
view_logs() {
    docker compose logs -f
}

# Function to run migrations
run_migrations() {
    print_info "Waiting for SQL Server to be ready..."
    sleep 15
    
    print_info "Running database migrations from host..."
    # Run migrations from host using connection string for Docker SQL Server
    CONNECTIONSTRING="Server=localhost,1433;Database=ForaProjectDb;User Id=sa;Password=ForaProject@2024!;TrustServerCertificate=True;MultipleActiveResultSets=true"
    dotnet ef database update --project src/ForaProject.Infrastructure --startup-project src/ForaProject.API --connection "$CONNECTIONSTRING"
    print_success "Migrations completed"
}

# Function to build and start
build_and_start() {
    print_info "Building and starting services..."
    docker compose up --build -d
    print_success "Build completed and services started"
    
    run_migrations
}

# Function to clean up everything
clean_all() {
    print_info "Stopping services and removing volumes..."
    docker compose down -v
    print_success "Cleanup completed"
}

# Function to show status
show_status() {
    docker compose ps
}

# Main menu
case "${1:-}" in
    start)
        check_docker
        start_services
        ;;
    stop)
        stop_services
        ;;
    restart)
        stop_services
        start_services
        ;;
    build)
        check_docker
        build_and_start
        ;;
    logs)
        view_logs
        ;;
    migrate)
        run_migrations
        ;;
    clean)
        clean_all
        ;;
    status)
        show_status
        ;;
    *)
        echo "ForaProject Docker Helper"
        echo ""
        echo "Usage: $0 {start|stop|restart|build|logs|migrate|clean|status}"
        echo ""
        echo "Commands:"
        echo "  start   - Start all services"
        echo "  stop    - Stop all services"
        echo "  restart - Restart all services"
        echo "  build   - Build and start all services (includes migrations)"
        echo "  logs    - View service logs"
        echo "  migrate - Run database migrations"
        echo "  clean   - Stop services and remove volumes"
        echo "  status  - Show service status"
        exit 1
        ;;
esac
