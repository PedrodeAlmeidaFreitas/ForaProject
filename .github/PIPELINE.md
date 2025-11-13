# 🔄 CI/CD Pipeline Overview

## Quick Reference

[![CI Status](https://github.com/PedrodeAlmeidaFreitas/ForaProject/workflows/CI%20-%20Build%20and%20Test/badge.svg)](https://github.com/PedrodeAlmeidaFreitas/ForaProject/actions)
[![CD Status](https://github.com/PedrodeAlmeidaFreitas/ForaProject/workflows/CD%20-%20Docker%20Build%20and%20Deploy/badge.svg)](https://github.com/PedrodeAlmeidaFreitas/ForaProject/actions)
[![CodeQL](https://github.com/PedrodeAlmeidaFreitas/ForaProject/workflows/CodeQL%20Security%20Analysis/badge.svg)](https://github.com/PedrodeAlmeidaFreitas/ForaProject/security/code-scanning)

## 🚀 Automated Workflows

| Workflow | Trigger | Purpose |
|----------|---------|---------|
| **CI - Build and Test** | Push, PR | Build, test, coverage |
| **CD - Docker Deploy** | Push to main, Tags | Build & deploy Docker images |
| **CodeQL Analysis** | Push, PR, Weekly | Security scanning |
| **Dependency Review** | PR | Vulnerability checks |
| **PR Quality Checks** | PR | Convention validation |
| **Performance** | Push to main | Load testing |

## 📋 Pipeline Features

### ✅ Continuous Integration
- **Automated Testing**: All test suites run automatically
  - Domain layer tests
  - Application layer tests
  - API layer tests
  - Integration tests with SQL Server
- **Code Coverage**: Minimum 80% threshold enforced
- **Coverage Reports**: Auto-generated and commented on PRs
- **Code Quality**: Format checking with `dotnet format`
- **Security Scanning**: NuGet vulnerability audits

### 🐳 Continuous Deployment
- **Multi-platform Builds**: `linux/amd64` and `linux/arm64`
- **Container Registry**: GitHub Container Registry (ghcr.io)
- **Automated Tagging**: Semantic versioning + commit SHA
- **Environment Deployments**: Staging and Production
- **Automatic Releases**: Created on version tags

### 🔐 Security
- **CodeQL Analysis**: Weekly security scans
- **Dependency Scanning**: Automated vulnerability detection
- **License Compliance**: GPL license blocking
- **Container Scanning**: Docker image security checks

### 📊 Quality Gates
- **Conventional Commits**: Enforced commit message format
- **PR Size Checks**: Warnings for large PRs (>50 files or >1000 lines)
- **Required Labels**: PRs must be labeled
- **Code Coverage**: 80% minimum coverage required

## 🛠️ Local Development

### Run CI Checks Locally

```bash
# Full validation (recommended before pushing)
./validate-ci.sh

# Individual checks
dotnet restore
dotnet build
dotnet test
dotnet format --verify-no-changes
```

### Test with Docker

```bash
# Build and test locally
docker compose up --build

# Run specific service
docker compose up sqlserver
```

### Code Coverage

```bash
# Generate coverage report
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings

# Install report generator
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generate HTML report
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport"
```

## 📝 Contribution Workflow

### 1. Create Feature Branch
```bash
git checkout -b feat/your-feature-name
```

### 2. Make Changes
- Follow [Clean Architecture guidelines](.github/copilot-instructions.md)
- Write tests for new code
- Follow coding standards

### 3. Validate Locally
```bash
./validate-ci.sh
```

### 4. Commit with Convention
```bash
git commit -m "feat(scope): add new feature"
```

**Commit Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation
- `test`: Tests
- `refactor`: Refactoring
- `perf`: Performance
- `ci`: CI/CD changes
- `chore`: Other changes

### 5. Push and Create PR
```bash
git push origin feat/your-feature-name
```

Use the [PR template](.github/PULL_REQUEST_TEMPLATE.md) to describe changes.

### 6. Automated Checks
- ✅ Build verification
- ✅ All tests pass
- ✅ Code coverage meets threshold
- ✅ No security vulnerabilities
- ✅ Conventional commit format
- ✅ Code quality checks pass

### 7. Review and Merge
- Address review comments
- Ensure all checks pass
- Squash and merge to main

## 🚀 Deployment Process

### Staging Deployment
Automatic on merge to `main`:
1. Merge PR to `main` branch
2. CI/CD builds Docker image
3. Pushes to container registry
4. Auto-deploys to staging environment

### Production Deployment
Manual via version tags:
```bash
# Create and push tag
git tag -a v1.0.0 -m "Release version 1.0.0"
git push origin v1.0.0
```

This triggers:
1. Docker image build
2. Multi-platform build
3. Push to registry with version tag
4. Deploy to production (with approval)
5. GitHub release creation

## 🔧 Configuration

### Environment Variables

**CI/CD (GitHub Actions):**
- `GITHUB_TOKEN`: Automatically provided
- No additional secrets required for basic functionality

**Local Development:**
```bash
export ASPNETCORE_ENVIRONMENT=Development
export ConnectionStrings__DefaultConnection="Server=localhost,1433;Database=ForaProjectDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
```

### Coverage Settings

Edit `coverlet.runsettings`:
```xml
<Configuration>
  <Format>cobertura,opencover</Format>
  <Exclude>[*.Tests]*,[*]*.Migrations.*</Exclude>
  <ExcludeByAttribute>Obsolete,GeneratedCode</ExcludeByAttribute>
</Configuration>
```

## 📚 Additional Resources

- [CI/CD Documentation](.github/CICD.md) - Detailed pipeline documentation
- [Pull Request Template](.github/PULL_REQUEST_TEMPLATE.md)
- [Bug Report Template](.github/ISSUE_TEMPLATE/bug_report.md)
- [Feature Request Template](.github/ISSUE_TEMPLATE/feature_request.md)
- [Coding Guidelines](.github/copilot-instructions.md)

## 🐛 Troubleshooting

### CI Failures

**Build fails:**
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

**Tests fail:**
```bash
# Run tests with verbose output
dotnet test --verbosity detailed
```

**Coverage below threshold:**
```bash
# Generate coverage report locally
dotnet test --collect:"XPlat Code Coverage"
```

**Format check fails:**
```bash
# Auto-format code
dotnet format
```

### Docker Issues

**Build fails:**
```bash
# Check Dockerfile syntax
docker build -t foraproject:test .

# Check compose configuration
docker compose config
```

**Container won't start:**
```bash
# Check logs
docker compose logs api
docker compose logs sqlserver
```

## 💡 Tips

1. **Run `./validate-ci.sh` before pushing** - Catches issues early
2. **Keep PRs small** - Easier to review and less likely to have conflicts
3. **Write meaningful commit messages** - Helps with changelog generation
4. **Add tests for new features** - Maintains code coverage
5. **Use PR templates** - Ensures all information is provided
6. **Monitor CI/CD runs** - Fix issues promptly

## 📞 Support

For CI/CD issues:
1. Check workflow logs in the [Actions tab](../../actions)
2. Review [CI/CD documentation](.github/CICD.md)
3. Open an issue using the appropriate template

---

**Note:** Replace `PedrodeAlmeidaFreitas` in badge URLs with your actual GitHub username.
