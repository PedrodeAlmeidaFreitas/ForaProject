#!/bin/bash

# CI/CD Local Validation Script
# Run this before pushing to ensure CI will pass

set -e

echo "🚀 ForaProject - Local CI Validation"
echo "======================================"
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to print status
print_status() {
    if [ $1 -eq 0 ]; then
        echo -e "${GREEN}✅ $2${NC}"
    else
        echo -e "${RED}❌ $2${NC}"
        exit 1
    fi
}

print_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

# Check if .NET is installed
echo "Checking prerequisites..."
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}❌ .NET SDK is not installed${NC}"
    exit 1
fi
print_status 0 ".NET SDK found"

# Check if Docker is running (optional)
if command -v docker &> /dev/null && docker info &> /dev/null; then
    print_status 0 "Docker is running"
    DOCKER_AVAILABLE=true
else
    print_warning "Docker is not available - skipping Docker-related checks"
    DOCKER_AVAILABLE=false
fi

echo ""
echo "📦 Restoring dependencies..."
dotnet restore ForaProject.sln --verbosity quiet
print_status $? "Dependencies restored"

echo ""
echo "🏗️  Building solution..."
dotnet build ForaProject.sln --configuration Release --no-restore --verbosity quiet
print_status $? "Build successful"

echo ""
echo "🎨 Checking code formatting..."
dotnet format ForaProject.sln --verify-no-changes --verbosity diagnostic > /dev/null 2>&1
if [ $? -ne 0 ]; then
    print_warning "Code formatting issues detected - run 'dotnet format' to fix"
    dotnet format ForaProject.sln --verbosity minimal
    print_status 0 "Code formatted automatically"
else
    print_status 0 "Code formatting is correct"
fi

echo ""
echo "🧪 Running tests..."

echo "  → Domain tests..."
dotnet test tests/ForaProject.Domain.Tests/ForaProject.Domain.Tests.csproj \
    --configuration Release --no-build --verbosity quiet --nologo
print_status $? "Domain tests passed"

echo "  → Application tests..."
dotnet test tests/ForaProject.Application.Tests/ForaProject.Application.Tests.csproj \
    --configuration Release --no-build --verbosity quiet --nologo
print_status $? "Application tests passed"

echo "  → API tests..."
dotnet test tests/ForaProject.API.Tests/ForaProject.API.Tests.csproj \
    --configuration Release --no-build --verbosity quiet --nologo
print_status $? "API tests passed"

if [ "$DOCKER_AVAILABLE" = true ]; then
    echo ""
    echo "  → Integration tests (requires SQL Server)..."
    
    # Start SQL Server if not running
    if ! docker ps | grep -q foraproject-sqlserver; then
        echo "    Starting SQL Server container..."
        docker compose up -d sqlserver
        echo "    Waiting for SQL Server to be ready..."
        sleep 15
    fi
    
    # Use environment variable or default test password
    SQL_PASSWORD="${SQL_SERVER_PASSWORD:-ForaProject@2024!}"
    export ConnectionStrings__DefaultConnection="Server=localhost,1433;Database=ForaProjectDb_Test;User Id=sa;Password=${SQL_PASSWORD};TrustServerCertificate=True;"
    
    dotnet test tests/ForaProject.IntegrationTests/ForaProject.IntegrationTests.csproj \
        --configuration Release --no-build --verbosity quiet --nologo
    print_status $? "Integration tests passed"
else
    print_warning "Skipping integration tests (Docker not available)"
fi

echo ""
echo "📊 Generating code coverage..."
dotnet test ForaProject.sln \
    --configuration Release \
    --no-build \
    --verbosity quiet \
    --collect:"XPlat Code Coverage" \
    --results-directory ./TestResults \
    --settings coverlet.runsettings > /dev/null 2>&1
print_status $? "Coverage collected"

# Check coverage threshold
if command -v xmllint &> /dev/null; then
    COVERAGE_FILE=$(find TestResults -name "coverage.cobertura.xml" | head -1)
    if [ -f "$COVERAGE_FILE" ]; then
        COVERAGE=$(xmllint --xpath "string(//coverage/@line-rate)" "$COVERAGE_FILE" 2>/dev/null)
        COVERAGE_PERCENT=$(echo "$COVERAGE * 100" | bc -l | xargs printf "%.2f")
        
        if (( $(echo "$COVERAGE < 80" | bc -l) )); then
            print_warning "Code coverage is ${COVERAGE_PERCENT}% (below 80% threshold)"
        else
            print_status 0 "Code coverage is ${COVERAGE_PERCENT}% (meets 80% threshold)"
        fi
    fi
fi

echo ""
echo "🔍 Running security audit..."
dotnet list package --vulnerable --include-transitive > /tmp/nuget-audit.log 2>&1
if grep -q "has the following vulnerable packages" /tmp/nuget-audit.log; then
    print_warning "Vulnerable packages detected - review /tmp/nuget-audit.log"
    cat /tmp/nuget-audit.log
else
    print_status 0 "No vulnerable packages found"
fi

if [ "$DOCKER_AVAILABLE" = true ]; then
    echo ""
    echo "🐳 Building Docker image..."
    docker build -t foraproject:local-test . --quiet
    print_status $? "Docker image built successfully"
fi

echo ""
echo "✅ Checking commit messages..."
if git log --oneline -1 | grep -qE "^[a-f0-9]+ (feat|fix|docs|style|refactor|perf|test|build|ci|chore|revert)(\(.+\))?: "; then
    print_status 0 "Last commit message follows convention"
else
    print_warning "Last commit message doesn't follow Conventional Commits format"
    echo "    Current message: $(git log --oneline -1)"
    echo "    Expected format: type(scope): description"
fi

echo ""
echo "======================================"
echo -e "${GREEN}🎉 All validation checks passed!${NC}"
echo "======================================"
echo ""
echo "You can now safely push your changes."
echo ""
echo "Next steps:"
echo "  git push origin <branch-name>"
echo ""
