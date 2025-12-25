# Performance Test Checklist

## Target URLs

- `/Stock/Index`
- `/SalesOrder/Index`
- `/PurchaseOrder/Index`
- `/CreditSalesOrder/Index`
- `/Transfer/Index`

## Dataset Scenarios

- Use a **large-data customer/concern** with:
  - 50k+ stock rows
  - 10k+ sales orders in a month
  - 10k+ purchase orders in a month
  - 10k+ credit sales entries
  - 10k+ transfers

## What to Look for in Logs

- **Request timing log** in log4net:
  - `Request {Controller}/{Action} ConcernId=... TotalMs=... DbMs=...`
- **Slow request SQL log** (only when total time ≥ 800ms):
  - `SlowRequestSql {Controller}/{Action} ...` with sanitized EF SQL.

## Expected Behavior

- List pages return **paged data** by default (page=1, pageSize=50).
- DB time should stay proportional to page size, not total dataset size.
- UI rendering should be faster due to smaller page payloads.

