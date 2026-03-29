# Phase 7: Economy

**Goal:** Weekly cash flow accurate. Season-end prize money distributed. Debt/bankruptcy works.

**Deliverable:** Budget changes correctly every week. Financial reports accurate.

---

## Weekly Pipeline

1. Deduct player wages (base + TSI-based variable)
2. Deduct staff wages
3. If home match: add gate receipts
4. Add sponsorship installment
5. Check debt limits (warn -200K, restrict -200K, bankrupt -500K)

## Season-End

- Membership fees: 30 USD × fans
- League prize money by final position
- Cup prize money (Phase 11)
- Adjust fans ±10% (promotion/relegation)

## Services

- `ITSICalculationService` - Total Skill Index (exponential skill weighting)
- `IWageCalculationService` - player/staff wage calculation
- `IEconomyService` - weekly cash flow, season-end processing
- `IFinanceReportService` - financial reporting UI support
