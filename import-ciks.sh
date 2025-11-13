#!/bin/bash

# Import all CIKs from the requirements

API_URL="http://localhost:5000/api/v1/companies/import"

# List of CIKs from requirements
CIKS=(
    18926 892553 1516524 1858912 1828248 1819493 60086 1853630 1761312 1851182
    1034665 927628 1125259 1547660 1393311 1757143 1958217 312070 310522 1861841
    1037868 1696355 1166834 915912 1085277 831259 882291 1521036 1824502 1015739
    884625 1001093 878927 21175 1439124 52827 1730773 1867287 1685428 1007587
    92103 1641751 6845 1231457 947263 895421 1988979 1848898 844798 1541309
    1858007 1729944 726958 1691271 730272 1308106 88414 1108134 1849058 1435617
    1857518 64803 1912458 1447380 1232384 1141758 1549922 914475 1498382 1400897
    314808 1323885 1526520 1550695 1634293 1756708 1540159 1076691 1580088 1532346
    923796 1849635 1872292 1227857 1646311 1710350 1476159 1844642 1967078 14278
    933267 1157557 1560293 217416 1798562 1038074 1843370
)

echo "Starting batch import of ${#CIKS[@]} companies..."
echo ""

SUCCESS=0
FAILED=0
FAILED_CIKS=()

for CIK in "${CIKS[@]}"; do
    echo -n "Importing CIK $CIK... "
    
    RESPONSE=$(curl -s -X POST "$API_URL" \
        -H "Content-Type: application/json" \
        -d "{\"cik\": $CIK}")
    
    if echo "$RESPONSE" | grep -q '"id"'; then
        echo "✓ Success"
        ((SUCCESS++))
    else
        echo "✗ Failed"
        ((FAILED++))
        FAILED_CIKS+=($CIK)
    fi
    
    # Small delay to avoid overwhelming the SEC API
    sleep 0.5
done

echo ""
echo "========================================="
echo "Import Summary:"
echo "  Total:   ${#CIKS[@]}"
echo "  Success: $SUCCESS"
echo "  Failed:  $FAILED"

if [ ${#FAILED_CIKS[@]} -gt 0 ]; then
    echo ""
    echo "Failed CIKs:"
    for CIK in "${FAILED_CIKS[@]}"; do
        echo "  - $CIK"
    done
fi

echo "========================================="
