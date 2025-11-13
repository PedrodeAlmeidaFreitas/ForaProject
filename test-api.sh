#!/bin/bash

# Test script for ForaProject API - Batch Import and Calculate

set -e

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

API_URL="http://localhost:5000/api/v1"

echo "========================================="
echo "ForaProject API - Test Script"
echo "========================================="
echo ""

# Step 1: Check API is running
echo -n "1. Checking if API is running... "
if curl -s -f "${API_URL}/companies" > /dev/null 2>&1; then
    echo -e "${GREEN}✓${NC}"
else
    echo -e "${RED}✗${NC}"
    echo "Error: API is not responding. Please start with: ./docker-dev.sh build"
    exit 1
fi

# Step 2: Get current company count
echo -n "2. Getting current company count... "
BEFORE_COUNT=$(curl -s "${API_URL}/companies" | grep -o '"id"' | wc -l)
echo -e "${GREEN}$BEFORE_COUNT companies${NC}"

# Step 3: Batch import companies
echo ""
echo "3. Importing companies from ciks-to-import.json..."
echo "   This will take a while due to SEC API rate limiting (30-60 seconds)..."
echo ""

# Use a longer timeout for batch import (2 minutes)
BATCH_RESPONSE=$(curl -s --max-time 120 -X POST "${API_URL}/companies/import/batch" \
    -H "Content-Type: application/json" \
    -d @ciks-to-import.json)

# Count the number of companies returned (batch import returns array of CompanyDto)
if [ -n "$BATCH_RESPONSE" ]; then
    SUCCESS_COUNT=$(echo "$BATCH_RESPONSE" | grep -o '"id"' | wc -l)
    
    if [ "$SUCCESS_COUNT" -gt 0 ]; then
        echo -e "   ${GREEN}✓ Successfully imported: $SUCCESS_COUNT companies${NC}"
    else
        echo -e "   ${YELLOW}⚠ No companies were imported${NC}"
        echo "   Response: $(echo "$BATCH_RESPONSE" | head -c 200)..."
    fi
else
    echo -e "   ${RED}✗ Batch import failed or timed out${NC}"
fi

# Step 4: Get updated company count
echo ""
echo -n "4. Getting updated company count... "
AFTER_COUNT=$(curl -s "${API_URL}/companies" | grep -o '"id"' | wc -l)
echo -e "${GREEN}$AFTER_COUNT companies${NC}"
echo "   New companies added: $((AFTER_COUNT - BEFORE_COUNT))"

# Step 5: Calculate fundable amounts for all companies
echo ""
echo "5. Calculating fundable amounts for all companies..."
CALC_RESPONSE=$(curl -s -X POST "${API_URL}/fundableamounts/calculate/all")

PROCESSED=$(echo "$CALC_RESPONSE" | grep -o '"processedCount":[0-9]*' | grep -o '[0-9]*' | tail -1)

if [ -n "$PROCESSED" ] && [ "$PROCESSED" -gt 0 ]; then
    echo -e "   ${GREEN}✓ Processed: $PROCESSED companies${NC}"
else
    echo -e "   ${YELLOW}⚠ No companies to process or calculation failed${NC}"
    echo "   Response: $(echo "$CALC_RESPONSE" | head -c 200)"
fi

# Step 6: Get companies with fundable amounts
echo ""
echo "6. Getting companies with calculated fundable amounts..."
FUNDABLE=$(curl -s "${API_URL}/fundableamounts" | grep -o '"id"' | wc -l)
echo -e "   ${GREEN}Companies with fundable amounts: $FUNDABLE${NC}"

# Step 7: Show sample companies
echo ""
echo "7. Sample companies with fundable amounts:"
echo ""

# Get first 3 companies with fundable amounts
curl -s "${API_URL}/fundableamounts" | python3 -m json.tool 2>/dev/null | head -50 || \
curl -s "${API_URL}/fundableamounts" | grep -A 5 '"entityName"' | head -20

echo ""
echo "========================================="
echo "Test Summary:"
echo "  Companies before:           $BEFORE_COUNT"
echo "  Companies after:            $AFTER_COUNT"
echo "  Successfully imported:      ${SUCCESS_COUNT:-0}"
echo "  Fundable amount calculated: $FUNDABLE"
echo "========================================="
echo ""
echo "Next steps:"
echo "  - View Swagger UI: http://localhost:5000/swagger"
echo "  - Get all companies: curl ${API_URL}/companies"
echo "  - Get fundable companies: curl ${API_URL}/fundableamounts"
echo ""
