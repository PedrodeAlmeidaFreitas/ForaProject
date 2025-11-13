# ForaProject - FinTech Company Fundable Amount Calculator

[![CI Status](https://github.com/PedrodeAlmeidaFreitas/ForaProject/workflows/CI%20-%20Build%20and%20Test/badge.svg)](https://github.com/PedrodeAlmeidaFreitas/ForaProject/actions)
[![CD Status](https://github.com/PedrodeAlmeidaFreitas/ForaProject/workflows/CD%20-%20Docker%20Build%20and%20Deploy/badge.svg)](https://github.com/PedrodeAlmeidaFreitas/ForaProject/actions)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

A .NET 8 application built with Clean Architecture principles to calculate fundable amounts for companies based on SEC EDGAR financial data.

## 📋 Overview

This application imports company financial data from the SEC EDGAR API and calculates two types of fundable amounts based on specific business rules.

### Business Rules

**Data Requirements:**
- Company must have income data for all years between 2018 and 2022
- Company must have positive income in both 2021 and 2022
- If these conditions are not met, fundable amounts are $0

**Standard Fundable Amount Calculation:**

Using the highest income between 2018 and 2022:
- If income ≥ $10B: **12.33%** of income
- If income < $10B: **21.51%** of income

**Special Fundable Amount Calculation:**

Starts with the Standard Fundable Amount, then applies modifiers:
- If company name starts with a vowel (A, E, I, O, U): **+15%** to standard amount
- If 2022 income < 2021 income: **-25%** from standard amount

### Example Calculations

**Company A** (High Income, starts with 'A', decreasing income):
- Highest income 2018-2022: $15B
- Standard: $15B × 12.33% = $1,849,500,000
- Special: $1,849,500,000 × 1.15 (vowel) × 0.75 (decrease) = $1,597,687,500

**Company B** (Low Income, starts with 'B', increasing income):
- Highest income 2018-2022: $5B
- Standard: $5B × 21.51% = $1,075,500,000
- Special: $1,075,500,000 (no modifiers) = $1,075,500,000

## 🏗️ Architecture

Built following **Clean Architecture** and **Domain-Driven Design (DDD)** principles:

```
├── ForaProject.Domain          # Core business logic and entities
├── ForaProject.Application     # Application services and use cases
├── ForaProject.Infrastructure  # Data persistence and external services
└── ForaProject.API            # REST API controllers and HTTP layer
```

### Design Patterns & Principles
- ✅ **SOLID Principles**
- ✅ **DRY** (Don't Repeat Yourself)
- ✅ **KISS** (Keep It Simple, Stupid)
- ✅ **Repository Pattern**
- ✅ **Unit of Work Pattern**
- ✅ **Value Objects**
- ✅ **Aggregates**

## 🚀 Getting Started

### Prerequisites

**Option 1: Docker (Recommended)**
- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)

**Option 2: Local Development**
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB, Express, or Full)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Installation

#### 🐳 Option 1: Using Docker (Recommended)

1. **Clone the repository**
   ```bash
   git clone https://github.com/PedrodeAlmeidaFreitas/ForaProject.git
   cd ForaProject
   ```

2. **Set up environment variables**
   ```bash
   # Quick setup with default values
   ./setup-env.sh
   
   # Or manually copy and edit
   cp .env.example .env
   # Edit .env and update SA_PASSWORD with a strong password
   ```

3. **Start the application with Docker Compose**
   ```bash
   docker compose up --build
   ```

   This will:
   - Start SQL Server 2022 container on port 1433
   - Build and start the API container on port 5000
   - Create the database and run migrations automatically
   - Set up a bridge network between containers
   - Use environment variables from `.env` file

4. **Access the API**
   - Swagger UI: http://localhost:5000/swagger
   - API Base URL: http://localhost:5000/api/v1

4. **Stop the application**
   ```bash
   docker compose down
   ```

5. **Stop and remove volumes (clean slate)**
   ```bash
   docker compose down -v
   ```

#### 💻 Option 2: Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/PedrodeAlmeidaFreitas/ForaProject.git
   cd ForaProject
   ```

2. **Update Connection String**
   
   Edit `src/ForaProject.API/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=ForaProjectDb;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

3. **Run Database Migrations**
   ```bash
   dotnet ef database update --project src/ForaProject.Infrastructure --startup-project src/ForaProject.API
   ```

4. **Build the Solution**
   ```bash
   dotnet build
   ```

5. **Run the API**
   ```bash
   cd src/ForaProject.API
   dotnet run
   ```

6. **Access Swagger UI**
   
   Open your browser and navigate to:
   ```
   https://localhost:5001/swagger
   ```

## 📚 API Endpoints

### Companies

- `GET /api/v1/companies` - Get all companies
- `GET /api/v1/companies/cik/{cik}` - Get company by CIK
- `GET /api/v1/companies/{id}` - Get company by ID
- `POST /api/v1/companies` - Create company manually
- `POST /api/v1/companies/import` - Import from SEC EDGAR
- `POST /api/v1/companies/import/batch` - Batch import
- `DELETE /api/v1/companies/{id}` - Delete company

### Fundable Amounts

- `GET /api/v1/fundableamounts` - Get all fundable companies
- `GET /api/v1/fundableamounts/letter/{letter}` - Filter by starting letter
- `POST /api/v1/fundableamounts/calculate/{cik}` - Calculate for one company
- `POST /api/v1/fundableamounts/calculate/all` - Calculate for all companies

## 🧪 Example Usage

### Import Apple Inc. (CIK: 320193)

```bash
curl -X POST https://localhost:5001/api/v1/companies/import \
  -H "Content-Type: application/json" \
  -d '{"cik": 320193}'
```

### Sample CIKs to Test

- **Apple Inc.**: 320193
- **Microsoft Corp**: 789019
- **Alphabet Inc.**: 1652044
- **Amazon.com Inc**: 1018724
- **Meta Platforms Inc.**: 1326801

## 🧰 Technologies Used

- **.NET 8** - Framework
- **Entity Framework Core 8** - ORM
- **SQL Server 2022** - Database
- **Docker & Docker Compose** - Containerization
- **AutoMapper** - Object mapping
- **FluentValidation** - Input validation
- **Swagger/OpenAPI** - API documentation

## 🐳 Docker Configuration

### Services

- **sqlserver**: SQL Server 2022 Developer Edition
  - Port: 1433
  - Volume: `sqlserver_data` for data persistence
  - Health check enabled

- **api**: ForaProject.API
  - Port: 5000 (HTTP)
  - Multi-stage build for optimized image size
  - Waits for SQL Server health check before starting

### Environment Variables

The application uses a `.env` file for configuration. Available variables:

| Variable | Description | Default |
|----------|-------------|---------|
| `SA_PASSWORD` | SQL Server SA password | `YourStrong!Passw0rd123` |
| `MSSQL_PID` | SQL Server edition | `Developer` |
| `DB_SERVER` | Database server name | `sqlserver` |
| `DB_NAME` | Database name | `ForaProjectDb` |
| `DB_USER` | Database user | `sa` |
| `ASPNETCORE_ENVIRONMENT` | ASP.NET Core environment | `Development` |
| `ASPNETCORE_URLS` | API URLs | `http://+:80` |
| `API_PORT` | Host port for API | `5000` |

**Setup:**

```bash
# Use setup script (recommended)
./setup-env.sh

# Or manually
cp .env.example .env
nano .env  # Update SA_PASSWORD and other values
```

**Security:** The `.env` file is ignored by git. Never commit it to version control!

### Docker Commands

```bash
# Build and start all services
docker compose up --build

# Start services in detached mode
docker compose up -d

# View logs
docker compose logs -f

# Stop all services
docker compose down

# Stop and remove volumes
docker compose down -v

# Rebuild specific service
docker compose build api
```

## 📁 Project Structure

```
ForaProject/
├── .github/
│   ├── workflows/                 # GitHub Actions CI/CD pipelines
│   ├── ISSUE_TEMPLATE/           # Issue templates
│   ├── CICD.md                   # CI/CD documentation
│   ├── PIPELINE.md               # Pipeline overview
│   └── PULL_REQUEST_TEMPLATE.md  # PR template
├── src/
│   ├── ForaProject.Domain/        # Business logic, entities, value objects
│   ├── ForaProject.Application/   # Services, DTOs, validators
│   ├── ForaProject.Infrastructure/# EF Core, repositories, migrations
│   └── ForaProject.API/          # Controllers, middleware, filters
├── tests/
│   ├── ForaProject.Domain.Tests/
│   ├── ForaProject.Application.Tests/
│   ├── ForaProject.API.Tests/
│   └── ForaProject.IntegrationTests/
├── docker-compose.yml            # Multi-container configuration
├── Dockerfile                    # API container image
├── coverlet.runsettings          # Code coverage settings
├── validate-ci.sh                # Local CI validation script
├── CHANGELOG.md                  # Version history
└── ForaProject.sln              # Solution file
```

## 🔄 CI/CD Pipeline

This project uses **GitHub Actions** for automated CI/CD:

- ✅ **Continuous Integration**: Automated builds, tests, and code coverage
- 🐳 **Docker Images**: Multi-platform builds pushed to GitHub Container Registry
- 🔐 **Security Scanning**: CodeQL analysis and dependency audits
- 📊 **Code Coverage**: 80% minimum threshold with automatic PR comments
- 🚀 **Automated Deployments**: Staging and production deployments

### Quick Start

```bash
# Run all CI checks locally before pushing
./validate-ci.sh

# View detailed CI/CD documentation
cat .github/PIPELINE.md
```

**Learn more:**
- [Pipeline Overview](.github/PIPELINE.md) - Quick reference guide
- [Detailed CI/CD Docs](.github/CICD.md) - Complete documentation
- [Contributing Guide](#-contributing) - Contribution workflow

## 🤝 Contributing

We welcome contributions! Please follow these steps:

### 1. Fork and Clone
```bash
git clone https://github.com/PedrodeAlmeidaFreitas/ForaProject.git
cd ForaProject
```

### 2. Create a Feature Branch
```bash
git checkout -b feat/your-feature-name
```

Use conventional commit prefixes: `feat`, `fix`, `docs`, `test`, `refactor`, `perf`, `ci`, `chore`

### 3. Make Your Changes
- Follow [Clean Architecture guidelines](.github/copilot-instructions.md)
- Write tests for new functionality
- Maintain or improve code coverage (80% minimum)
- Follow SOLID principles and DDD patterns

### 4. Run Local Validation
```bash
./validate-ci.sh
```

This runs:
- Build verification
- All test suites
- Code coverage check
- Code formatting validation
- Security audit

### 5. Commit with Conventional Format
```bash
git commit -m "feat(domain): add new entity for X"
git commit -m "fix(api): correct validation in Y controller"
git commit -m "docs(readme): update installation instructions"
```

### 6. Push and Create PR
```bash
git push origin feat/your-feature-name
```

Then create a Pull Request using the [PR template](.github/PULL_REQUEST_TEMPLATE.md)

### 7. PR Review Process
- ✅ All CI checks must pass
- ✅ Code coverage threshold met (80%)
- ✅ At least one approval required
- ✅ Conventional commit format
- ✅ No security vulnerabilities

### Reporting Issues

Use the appropriate issue template:
- [🐛 Bug Report](.github/ISSUE_TEMPLATE/bug_report.md)
- [✨ Feature Request](.github/ISSUE_TEMPLATE/feature_request.md)

### Code of Conduct

- Be respectful and inclusive
- Follow the project's coding standards
- Write clear commit messages and PR descriptions
- Keep PRs focused and reasonably sized

## � Additional Documentation

- **[Workflow Status Guide](WORKFLOW_STATUS.md)** - Understand GitHub Actions workflows and expected behavior
- **[Workflow Fixes](WORKFLOW_FIXES.md)** - Quick fixes for common CI/CD failures
- **[Quickstart Guide](QUICKSTART.md)** - Get started quickly with development
- **[CIK Import Guide](CIK_IMPORT_GUIDE.md)** - Import company data from SEC EDGAR
- **[Coverage Report](COVERAGE.md)** - Code coverage metrics and goals
- **[Changelog](CHANGELOG.md)** - Version history and changes

## �📄 License

MIT License
