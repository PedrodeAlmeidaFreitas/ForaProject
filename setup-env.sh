#!/bin/bash

# Environment Setup Script for ForaProject
# This script creates a .env file from .env.example

set -e

echo "🔧 Setting up ForaProject environment..."
echo ""

# Check if .env already exists
if [ -f .env ]; then
    echo "⚠️  .env file already exists!"
    read -p "Do you want to overwrite it? (y/N): " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        echo "❌ Setup cancelled."
        exit 0
    fi
fi

# Copy .env.example to .env
echo "📝 Creating .env file from .env.example..."
cp .env.example .env

echo ""
echo "✅ .env file created successfully!"
echo ""
echo "⚠️  IMPORTANT: Update the following values in .env file:"
echo "   - SA_PASSWORD: Use a strong password (current: YourStrong!Passw0rd123)"
echo ""
echo "📝 Edit .env file:"
echo "   nano .env"
echo "   # or"
echo "   code .env"
echo ""
echo "🚀 After updating .env, start the application:"
echo "   docker compose up --build"
echo ""
