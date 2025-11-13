# ForaProject - Quick Start Guide

## 🚀 Quick Start with Docker

### First Time Setup

```bash
# 1. Make sure Docker is running
docker --version
docker compose --version

# 2. Build and start everything (this will take a few minutes)
./docker-dev.sh build

# 3. Access the API
# Swagger UI: http://localhost:5000/swagger
# API: http://localhost:5000/api/v1
```

### Daily Development

```bash
# Start services
./docker-dev.sh start

# View logs
./docker-dev.sh logs

# Stop services
./docker-dev.sh stop

# Check status
./docker-dev.sh status
```

### Database Operations

```bash
# Run migrations
./docker-dev.sh migrate

# Clean database and start fresh
./docker-dev.sh clean
./docker-dev.sh build
```

### Testing the API

```bash
# Quick test - Import all CIKs and calculate fundable amounts
./test-api.sh

# Or import CIKs individually (with progress tracking)
./import-ciks.sh

# Batch import using JSON file
curl -X POST http://localhost:5000/api/v1/companies/import/batch \
  -H "Content-Type: application/json" \
  -d @ciks-to-import.json
```

## 🔧 Manual Docker Commands

### Start Everything

```bash
docker compose up --build
```

### Start in Background

```bash
docker compose up -d
```

### View Logs

```bash
# All services
docker compose logs -f

# Only API
docker compose logs -f api

# Only SQL Server
docker compose logs -f sqlserver
```

### Stop Services

```bash
# Stop without removing volumes
docker compose down

# Stop and remove volumes (fresh start)
docker compose down -v
```

### Connect to SQL Server

```bash
# From host machine
docker exec -it foraproject-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "ForaProject@2024!"

# Inside sqlcmd
1> SELECT name FROM sys.databases;
2> GO
```

### Access Running Containers

```bash
# Shell into API container
docker exec -it foraproject-api /bin/bash

# Shell into SQL Server container
docker exec -it foraproject-sqlserver /bin/bash
```

## 🧪 Testing the API

### Import a Company

```bash
# Apple Inc. (CIK: 320193)
curl -X POST http://localhost:5000/api/v1/companies/import \
  -H "Content-Type: application/json" \
  -d '{"cik": 320193}'
```

### Get All Companies

```bash
curl http://localhost:5000/api/v1/companies
```

### Calculate Fundable Amount

```bash
curl -X POST http://localhost:5000/api/v1/fundableamounts/calculate/320193
```

### Calculate All Fundable Amounts

```bash
curl -X POST http://localhost:5000/api/v1/fundableamounts/calculate/all
```

## 📦 Sample CIKs for Testing

- **Apple Inc.**: 320193
- **Microsoft Corp**: 789019
- **Alphabet Inc.**: 1652044
- **Amazon.com Inc**: 1018724
- **Meta Platforms Inc.**: 1326801

## 🛠️ Troubleshooting

### Port Already in Use

If ports 1433 or 5000 are already in use:

```bash
# Check what's using the port
sudo lsof -i :5000
sudo lsof -i :1433

# Kill the process
sudo kill -9 <PID>
```

Or edit `docker compose.yml` to use different ports:

```yaml
ports:
  - "5001:80"  # Change 5000 to 5001
```

### SQL Server Container Fails to Start

```bash
# Check logs
docker compose logs sqlserver

# Remove volumes and restart
docker compose down -v
docker compose up -d
```

### API Can't Connect to Database

```bash
# Check if SQL Server is healthy
docker compose ps

# Restart API container
docker compose restart api
```

### Permission Denied on docker-dev.sh

```bash
chmod +x docker-dev.sh
```

## 🔐 Security Notes

### ⚠️ IMPORTANT for Production

The default password `ForaProject@2024!` is for **development only**.

For production:

1. Create a `.env` file:
   ```bash
   cp .env.example .env
   ```

2. Edit `.env` with a strong password:
   ```
   SA_PASSWORD=YourStrongProductionPassword!
   ```

3. Update `docker compose.yml` to use environment variables:
   ```yaml
   environment:
     - SA_PASSWORD=${SA_PASSWORD}
   ```

4. **Never commit `.env` to git** (it's already in `.gitignore`)

## 📚 Additional Resources

- [.NET 8 Documentation](https://docs.microsoft.com/dotnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [Docker Documentation](https://docs.docker.com/)
- [SEC EDGAR API](https://www.sec.gov/edgar/sec-api-documentation)

## 🆘 Getting Help

If you encounter issues:

1. Check the logs: `./docker-dev.sh logs`
2. Verify services are running: `./docker-dev.sh status`
3. Try a clean restart: `./docker-dev.sh clean && ./docker-dev.sh build`
4. Check Docker resources (CPU, Memory)
5. Review the README.md and STATUS.md files
