# CIK Import Instructions

## 📦 CIK List for Import

This project includes **97 CIK (Central Index Key)** numbers to import from the SEC EDGAR database.

### Files Created

1. **ciks-to-import.json** - JSON file for batch import via API
2. **import-ciks.sh** - Shell script to import CIKs one by one with progress tracking
3. **test-api.sh** - Complete test script that imports and calculates fundable amounts

## 🚀 Quick Import

### Option 1: Automated Test (Recommended)

This will import all CIKs and calculate fundable amounts automatically:

```bash
./test-api.sh
```

This script will:
- ✅ Check if API is running
- ✅ Import all 97 CIKs in batch
- ✅ Calculate fundable amounts for all companies
- ✅ Show summary statistics
- ✅ Display sample results

### Option 2: Batch Import via API

Import all CIKs at once using the batch endpoint:

```bash
curl -X POST http://localhost:5000/api/v1/companies/import/batch \
  -H "Content-Type: application/json" \
  -d @ciks-to-import.json
```

### Option 3: Individual Import with Progress

Import CIKs one by one to see detailed progress:

```bash
./import-ciks.sh
```

This will show:
```
Importing CIK 18926... ✓ Success
Importing CIK 892553... ✓ Success
Importing CIK 1516524... ✗ Failed
...
```

## 📊 CIK List (97 Companies)

```
18926, 892553, 1516524, 1858912, 1828248, 1819493, 60086, 1853630, 1761312, 1851182,
1034665, 927628, 1125259, 1547660, 1393311, 1757143, 1958217, 312070, 310522, 1861841,
1037868, 1696355, 1166834, 915912, 1085277, 831259, 882291, 1521036, 1824502, 1015739,
884625, 1001093, 878927, 21175, 1439124, 52827, 1730773, 1867287, 1685428, 1007587,
92103, 1641751, 6845, 1231457, 947263, 895421, 1988979, 1848898, 844798, 1541309,
1858007, 1729944, 726958, 1691271, 730272, 1308106, 88414, 1108134, 1849058, 1435617,
1857518, 64803, 1912458, 1447380, 1232384, 1141758, 1549922, 914475, 1498382, 1400897,
314808, 1323885, 1526520, 1550695, 1634293, 1756708, 1540159, 1076691, 1580088, 1532346,
923796, 1849635, 1872292, 1227857, 1646311, 1710350, 1476159, 1844642, 1967078, 14278,
933267, 1157557, 1560293, 217416, 1798562, 1038074, 1843370
```

## 🔍 After Import

### View All Imported Companies

```bash
curl http://localhost:5000/api/v1/companies
```

### Calculate Fundable Amounts

```bash
# Calculate for all companies
curl -X POST http://localhost:5000/api/v1/fundableamounts/calculate/all

# Get companies with fundable amounts
curl http://localhost:5000/api/v1/fundableamounts
```

### Filter by Company Name

```bash
# Get companies starting with 'A'
curl http://localhost:5000/api/v1/fundableamounts/letter/A

# Get companies starting with 'M'
curl http://localhost:5000/api/v1/fundableamounts/letter/M
```

## ⚠️ Important Notes

### SEC API Rate Limiting

The SEC EDGAR API has rate limits:
- **10 requests per second** maximum
- The import scripts include delays to respect this limit
- Batch import may take several minutes

### Expected Failures

Not all CIKs will import successfully because:
- ✅ Company may not have data for years 2018-2022
- ✅ Company may not have the required 10-K filings
- ✅ Company may not have revenue data in the expected format
- ✅ SEC API may return errors for some CIKs

This is **normal behavior** - the application will import what it can and skip invalid entries.

### Data Validation

After import, companies need income data for **all years 2018-2022** to calculate fundable amounts:
- Companies without complete data will have $0 fundable amounts
- Only companies with valid data appear in `/fundableamounts` endpoint

## 📈 Expected Results

Based on the business rules:

**Standard Fundable Amount:**
- High income companies (≥ $10B): **12.33%** of highest income
- Lower income companies (< $10B): **21.51%** of highest income

**Special Fundable Amount:**
- Starts with Standard amount
- **+15%** if company name starts with vowel (A, E, I, O, U)
- **-25%** if 2022 income < 2021 income

## 🧪 Testing Individual CIKs

```bash
# Import specific company
curl -X POST http://localhost:5000/api/v1/companies/import \
  -H "Content-Type: application/json" \
  -d '{"cik": 320193}'

# Calculate fundable amount for specific CIK
curl -X POST http://localhost:5000/api/v1/fundableamounts/calculate/320193

# Get company by CIK
curl http://localhost:5000/api/v1/companies/cik/320193
```

## 📚 More Information

- **QUICKSTART.md** - Docker setup and daily usage
- **README.md** - Complete API documentation
- **Swagger UI** - http://localhost:5000/swagger (interactive API docs)

---

**Ready to import?** Run `./test-api.sh` to get started! 🚀
