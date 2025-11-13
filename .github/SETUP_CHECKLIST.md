# 🎯 CI/CD Setup Checklist

Use this checklist to ensure your CI/CD pipeline is properly configured and operational.

## ✅ Initial Setup

### Repository Configuration
- [ ] Fork or clone the repository
- [ ] Update `PedrodeAlmeidaFreitas` in badge URLs in README.md
- [ ] Review and update author information in CHANGELOG.md
- [ ] Make `validate-ci.sh` executable: `chmod +x validate-ci.sh`

### GitHub Settings
- [ ] Enable GitHub Actions in repository settings
- [ ] Enable Dependabot security updates
- [ ] Enable Dependabot version updates
- [ ] Enable vulnerability alerts

## ✅ Branch Protection

### Main Branch Protection
- [ ] Require pull request reviews (minimum: 1)
- [ ] Dismiss stale approvals when new commits are pushed
- [ ] Require status checks to pass before merging:
  - [ ] `build-and-test` (CI workflow)
  - [ ] `code-quality` (CI workflow)
  - [ ] `CodeQL` (Security workflow)
- [ ] Require branches to be up to date before merging
- [ ] Require conversation resolution before merging
- [ ] Include administrators in restrictions
- [ ] Restrict force pushes

### Develop Branch Protection (if using)
- [ ] Require pull request reviews (minimum: 1)
- [ ] Require status checks to pass

## ✅ Environments (Optional)

### Staging Environment
- [ ] Create `staging` environment
- [ ] Configure environment URL (e.g., https://staging.yourdomain.com)
- [ ] Add environment secrets if needed
- [ ] Set deployment branch to `main` only

### Production Environment
- [ ] Create `production` environment
- [ ] Configure environment URL (e.g., https://yourdomain.com)
- [ ] Add required reviewers (2+ recommended)
- [ ] Add environment secrets
- [ ] Set deployment branch to tags matching `v*`
- [ ] Enable wait timer (optional, e.g., 5 minutes)

## ✅ Secrets and Variables

### Repository Secrets
- [ ] `GITHUB_TOKEN` - ✅ Automatically provided
- [ ] Add deployment-specific secrets (optional):
  - [ ] `DEPLOYMENT_KEY` (if using SSH deployments)
  - [ ] `SLACK_WEBHOOK` (if using Slack notifications)
  - [ ] `SONAR_TOKEN` (if using SonarCloud)

### Repository Variables
- [ ] `DOCKER_REGISTRY` (default: ghcr.io)
- [ ] `IMAGE_NAME` (default: repository name)

## ✅ Code Coverage

### Coverage Configuration
- [ ] Verify `coverlet.runsettings` exists
- [ ] Adjust coverage threshold if needed (default: 80%)
- [ ] Verify coverage exclusions are appropriate
- [ ] Test coverage locally: `dotnet test --collect:"XPlat Code Coverage"`

### Coverage Reporting
- [ ] Enable GitHub Pages (optional, for hosting reports)
- [ ] Configure Codecov integration (optional)
- [ ] Verify PR coverage comments are working

## ✅ Security

### CodeQL Analysis
- [ ] CodeQL workflow is enabled
- [ ] Weekly scans are scheduled
- [ ] Review security alerts in Security tab
- [ ] Configure code scanning alerts

### Dependency Scanning
- [ ] Dependabot is enabled
- [ ] Review dependency alerts
- [ ] Configure automatic security updates
- [ ] Set up PR auto-merge for minor updates (optional)

### Security Policies
- [ ] Create SECURITY.md file (optional)
- [ ] Define vulnerability disclosure policy
- [ ] Set up security contacts

## ✅ Quality Gates

### PR Quality Checks
- [ ] Conventional Commits enforcement is active
- [ ] PR size warnings are configured
- [ ] Label requirements are set
- [ ] PR template is being used

### Code Quality
- [ ] `dotnet format` checks are passing
- [ ] Security scan is configured
- [ ] No blocking vulnerabilities in dependencies

## ✅ Docker & Deployment

### Container Registry
- [ ] GitHub Container Registry is enabled
- [ ] Registry is public or access tokens are configured
- [ ] Multi-platform builds are working (amd64, arm64)
- [ ] Images are tagged correctly

### Deployment Targets
- [ ] Staging deployment target is configured
- [ ] Production deployment target is configured
- [ ] Deployment scripts are updated with actual targets
- [ ] Health checks are implemented
- [ ] Rollback procedures are documented

## ✅ Documentation

### Project Documentation
- [ ] README.md is updated with CI/CD badges
- [ ] CHANGELOG.md is initialized
- [ ] Contributing guidelines are clear
- [ ] Architecture documentation is up to date

### CI/CD Documentation
- [ ] `.github/CICD.md` is reviewed
- [ ] `.github/PIPELINE.md` is reviewed
- [ ] `.github/PIPELINE_SETUP.md` is reviewed
- [ ] Workflow diagrams are clear

### Templates
- [ ] PR template is customized
- [ ] Bug report template is customized
- [ ] Feature request template is customized

## ✅ Testing

### Local Testing
- [ ] Run `./validate-ci.sh` successfully
- [ ] All unit tests pass locally
- [ ] All integration tests pass locally
- [ ] Code coverage meets threshold locally
- [ ] Docker build completes successfully

### CI Testing
- [ ] Create a test PR to verify workflows
- [ ] Verify all CI checks pass
- [ ] Verify coverage report is generated
- [ ] Verify PR comments are posted
- [ ] Verify security scans complete

### CD Testing
- [ ] Test staging deployment (merge to main)
- [ ] Test production deployment (create version tag)
- [ ] Verify Docker images are published
- [ ] Verify GitHub releases are created
- [ ] Test deployment rollback

## ✅ Monitoring & Alerts

### GitHub Actions
- [ ] Set up workflow failure notifications
- [ ] Monitor workflow run times
- [ ] Review workflow logs regularly
- [ ] Set up status badges on README

### Application Monitoring
- [ ] Set up application logging
- [ ] Configure health check endpoints
- [ ] Set up uptime monitoring
- [ ] Configure alerting for critical issues

## ✅ Team Onboarding

### Developer Setup
- [ ] Update team with new CI/CD process
- [ ] Share local validation script usage
- [ ] Document commit message conventions
- [ ] Provide PR creation guidelines

### Access & Permissions
- [ ] Grant team members appropriate repository access
- [ ] Configure CODEOWNERS file (optional)
- [ ] Set up team-specific notifications
- [ ] Document approval requirements

## ✅ Maintenance

### Regular Tasks
- [ ] Review Dependabot PRs weekly
- [ ] Monitor security alerts
- [ ] Update dependencies regularly
- [ ] Review and update workflows quarterly

### Performance
- [ ] Monitor workflow execution times
- [ ] Optimize long-running jobs
- [ ] Use caching where appropriate
- [ ] Review and clean up old workflow runs

## ✅ Final Verification

### Pre-Launch Checklist
- [ ] All workflows are green
- [ ] Coverage threshold is met
- [ ] No security vulnerabilities
- [ ] Documentation is complete
- [ ] Team is trained on new process

### First Production Release
- [ ] Create first version tag: `v1.0.0`
- [ ] Verify production deployment
- [ ] Monitor for issues
- [ ] Update CHANGELOG.md
- [ ] Announce release to team

## 📝 Notes

### Customization Points
1. **Coverage Threshold**: Adjust in `.github/workflows/ci.yml` (line ~95)
2. **PR Size Limits**: Adjust in `.github/workflows/pr-checks.yml` (lines ~31-32)
3. **Deployment URLs**: Update in `.github/workflows/cd.yml`
4. **Branch Names**: Adjust workflow triggers if using different branch names

### Common Issues
- **Coverage not calculated**: Ensure `coverlet.runsettings` is in root directory
- **Integration tests fail**: Verify SQL Server service is healthy
- **Docker build fails**: Check Dockerfile and context paths
- **Deployments fail**: Verify environment secrets are set correctly

### Support Resources
- GitHub Actions Docs: https://docs.github.com/en/actions
- Troubleshooting Guide: `.github/CICD.md`
- Local validation: `./validate-ci.sh`

---

**Last Updated:** November 12, 2025  
**Review Frequency:** Quarterly or when making significant changes

## 🎉 Completion

Once all items are checked:
1. Commit all changes
2. Push to repository
3. Create a test PR to verify everything works
4. Celebrate! 🎊

Your CI/CD pipeline is now production-ready!
