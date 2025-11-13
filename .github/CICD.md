# CI/CD Pipeline Documentation

## 📋 Overview

This project uses GitHub Actions for Continuous Integration and Continuous Deployment. The pipeline is designed following best practices for .NET applications with comprehensive testing, code quality checks, and automated deployments.

## 🔄 Workflows

### 1. CI - Build and Test (`ci.yml`)

**Trigger:** Push to `main`/`develop`, Pull Requests

**Jobs:**
- **Build and Test**: Compiles the solution and runs all test suites
  - Domain tests
  - Application tests
  - API tests
  - Integration tests (with SQL Server)
  - Code coverage collection (80% threshold)
  - Coverage report generation

- **Code Quality**: Runs code quality checks
  - `dotnet format` verification
  - Security scanning

**Artifacts:**
- Test results (`.trx` files)
- Code coverage report (HTML + Cobertura)
- Coverage summary on PR comments

### 2. CD - Docker Build and Deploy (`cd.yml`)

**Trigger:** Push to `main`, version tags (`v*.*.*`)

**Jobs:**
- **Build and Push**: Creates multi-platform Docker images
  - Builds for `linux/amd64` and `linux/arm64`
  - Pushes to GitHub Container Registry (ghcr.io)
  - Tags: `latest`, `main`, version tags, commit SHA

- **Deploy to Staging**: Automatic deployment on main branch
  - Environment: `staging`

- **Deploy to Production**: Automatic deployment on version tags
  - Environment: `production`
  - Creates GitHub releases

### 3. CodeQL Security Analysis (`codeql.yml`)

**Trigger:** Push, Pull Requests, Weekly schedule (Mondays 6 AM), Manual

**Purpose:**
- Static code analysis for security vulnerabilities
- Code quality issues detection
- SAST (Static Application Security Testing)

### 4. Dependency Review (`dependency-review.yml`)

**Trigger:** Pull Requests

**Jobs:**
- **Dependency Review**: Checks for vulnerable dependencies
  - Fails on high/critical vulnerabilities
  - Checks license compliance (blocks GPL-2.0, GPL-3.0)
  
- **NuGet Audit**: Scans NuGet packages for known vulnerabilities
  - Checks direct and transitive dependencies

### 5. PR Quality Checks (`pr-checks.yml`)

**Trigger:** Pull Requests (opened, updated)

**Validations:**
- PR title follows Conventional Commits format
- PR size warnings (>50 files or >1000 lines)
- Commit message format validation
- Ensures PR has at least one label

### 6. Performance Benchmarks (`performance.yml`)

**Trigger:** Push to `main`, Pull Requests, Manual

**Purpose:**
- API load testing
- Performance regression detection
- Response time monitoring

## 🚀 Getting Started

### Prerequisites

1. **GitHub Repository Secrets:**
   ```
   GITHUB_TOKEN (automatically provided)
   ```

2. **GitHub Environments** (optional, for deployment):
   - `staging`
   - `production`

### Setting Up Environments

1. Go to repository Settings → Environments
2. Create `staging` and `production` environments
3. Add environment protection rules:
   - Required reviewers for production
   - Branch restrictions

### Running Workflows Locally

Use [act](https://github.com/nektos/act) to test GitHub Actions locally:

```bash
# Install act
brew install act  # macOS
# or
curl https://raw.githubusercontent.com/nektos/act/master/install.sh | sudo bash

# Run CI workflow
act -j build-and-test

# Run with secrets
act -j build-and-test -s GITHUB_TOKEN=your_token
```

## 📊 Code Coverage

### Coverage Configuration

Coverage settings are in `coverlet.runsettings`:
- **Format:** Cobertura + OpenCover
- **Excluded:** Tests, Migrations, Generated code
- **Threshold:** 80% minimum

### Viewing Coverage Reports

1. **In Pull Requests:** Automatically commented by the bot
2. **In Actions:** Download from artifacts
3. **Locally:** 
   ```bash
   dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
   dotnet tool install -g dotnet-reportgenerator-globaltool
   reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport"
   ```

## 🐳 Docker Image Registry

### Image Tags

Images are tagged with:
- `latest` - latest main branch build
- `main` - main branch builds
- `develop` - develop branch builds
- `v1.2.3` - semantic version tags
- `main-sha123456` - commit SHA tags

### Pulling Images

```bash
# Pull latest
docker pull ghcr.io/your-username/foraproject:latest

# Pull specific version
docker pull ghcr.io/your-username/foraproject:v1.0.0
```

## 🔐 Security

### Security Scanning

1. **CodeQL**: Weekly scans + on every push/PR
2. **Dependency Review**: On every PR
3. **NuGet Audit**: Checks for vulnerable packages
4. **Container Scanning**: Docker images scanned during build

### Security Best Practices

- All secrets stored in GitHub Secrets
- SQL Server password is test-only (not used in production)
- Multi-stage Docker builds for smaller images
- Non-root container user (recommended)
- TLS/SSL enforced in production

## 📝 Commit Convention

Follow [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>(<scope>): <description>

[optional body]

[optional footer]
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation
- `style`: Code style changes
- `refactor`: Code refactoring
- `perf`: Performance improvement
- `test`: Test changes
- `build`: Build system changes
- `ci`: CI/CD changes
- `chore`: Other changes

**Examples:**
```
feat(api): add company import endpoint
fix(domain): correct fundable amount calculation for vowel companies
docs(readme): update deployment instructions
test(application): add unit tests for company service
```

## 🏷️ PR Labels

Recommended labels:
- `bug` - Bug fixes
- `enhancement` - New features
- `documentation` - Documentation updates
- `dependencies` - Dependency updates
- `breaking-change` - Breaking changes
- `good-first-issue` - Good for newcomers
- `help-wanted` - Extra attention needed

## 🔄 Deployment Process

### Staging Deployment

1. Merge PR to `main`
2. CI pipeline runs automatically
3. Docker image built and pushed
4. Auto-deploys to staging environment
5. Run smoke tests

### Production Deployment

1. Create a version tag:
   ```bash
   git tag -a v1.0.0 -m "Release version 1.0.0"
   git push origin v1.0.0
   ```
2. CD pipeline triggers
3. Docker image built with version tag
4. Deploys to production (with approval if configured)
5. GitHub release created automatically

### Rollback

```bash
# Revert to previous version
docker pull ghcr.io/your-username/foraproject:v1.0.0
docker-compose up -d
```

## 🛠️ Customization

### Adding New Workflows

1. Create file in `.github/workflows/`
2. Use existing workflows as templates
3. Test locally with `act`
4. Create PR with new workflow

### Modifying Build Steps

Edit the relevant workflow file:
- `ci.yml` - Build and test steps
- `cd.yml` - Deployment steps
- `codeql.yml` - Security scanning

### Changing Coverage Threshold

Edit `ci.yml`:
```yaml
if (( $(echo "$COVERAGE < 0.80" | bc -l) )); then  # Change 0.80 to desired threshold
```

## 📞 Support

For issues with CI/CD:
1. Check workflow logs in Actions tab
2. Review this documentation
3. Check GitHub Actions documentation
4. Open an issue in the repository

## 🔗 Related Documentation

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Docker Documentation](https://docs.docker.com/)
- [Conventional Commits](https://www.conventionalcommits.org/)
- [Semantic Versioning](https://semver.org/)
