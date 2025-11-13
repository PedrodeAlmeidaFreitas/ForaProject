# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- CI/CD pipeline with GitHub Actions
  - Automated build and test workflow
  - Code coverage reporting (80% threshold)
  - Docker image building and publishing
  - Automated deployments to staging and production
  - Security scanning with CodeQL
  - Dependency vulnerability scanning
  - PR quality checks and validation
  - Performance benchmarking
- Dependabot configuration for automated dependency updates
- Pull request template with comprehensive checklist
- Issue templates for bug reports and feature requests
- CI/CD documentation
- Local validation script (`validate-ci.sh`)
- Code coverage configuration (`coverlet.runsettings`)

### Changed
- Enhanced project documentation with CI/CD information

### Security
- Added automated security scanning
- Implemented dependency review on PRs
- Added NuGet package vulnerability audits

## [1.0.0] - YYYY-MM-DD

### Added
- Initial release of ForaProject
- Clean Architecture implementation with DDD principles
- Domain layer with entities, value objects, and aggregates
- Application layer with services and DTOs
- Infrastructure layer with EF Core and repositories
- API layer with REST endpoints
- Company import from SEC EDGAR API
- Fundable amount calculation engine
- Docker support with docker-compose
- Comprehensive test suite (Domain, Application, API, Integration)
- Swagger/OpenAPI documentation

[Unreleased]: https://github.com/PedrodeAlmeidaFreitas/ForaProject/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/PedrodeAlmeidaFreitas/ForaProject/releases/tag/v1.0.0
