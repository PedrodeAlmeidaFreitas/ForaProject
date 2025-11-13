# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ForaProject.sln ./
COPY src/ForaProject.Domain/ForaProject.Domain.csproj ./src/ForaProject.Domain/
COPY src/ForaProject.Application/ForaProject.Application.csproj ./src/ForaProject.Application/
COPY src/ForaProject.Infrastructure/ForaProject.Infrastructure.csproj ./src/ForaProject.Infrastructure/
COPY src/ForaProject.API/ForaProject.API.csproj ./src/ForaProject.API/

# Restore dependencies (only for main projects, ignore test projects)
RUN dotnet restore src/ForaProject.API/ForaProject.API.csproj

# Copy all source code
COPY src/ ./src/

# Build the application
WORKDIR /src/src/ForaProject.API
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "ForaProject.API.dll"]
