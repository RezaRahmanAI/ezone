# Performance Findings (Baseline)

> **Note:** Timings are estimates based on code inspection; runtime profiling should confirm exact values.

## Top 5 Suspect Endpoints (Large Datasets)

1. **Stock list**
   - **Page/Action:** `GET /Stock/Index` (`IMSWEB/Controllers/StockController.cs`, `Index`)
   - **Repository/Method:** `IStockService.GetAllStockAsync` → `IMSWEB.Data/Extentions/StockExtensions.GetAllStockAsync`
   - **Suspected issue:** loads full stock result set for a concern; multiple subqueries for pricing; no paging; no `AsNoTracking`.
   - **Estimated before:** 3–12s with large stock tables; high DB CPU + large memory usage.
   - **After change:** server-side paging + `AsNoTracking` (expected 300–900ms for first page).

2. **Sales order list**
   - **Page/Action:** `GET /SalesOrder/Index` (`IMSWEB/Controllers/SalesOrderController.cs`, `Index`)
   - **Repository/Method:** `ISalesOrderService.GetAllSalesOrderAsync` → `IMSWEB.Data/Extentions/SalesOrderExtensions.GetAllSalesOrderAsync`
   - **Suspected issue:** full date-range load with `ToListAsync` before paging; `AsNoTracking` missing; VAT manager logic materializes entire list.
   - **Estimated before:** 2–8s (large date ranges).
   - **After change:** paging + streaming VAT manager window (expected 250–800ms for first page).

3. **Purchase order list**
   - **Page/Action:** `GET /PurchaseOrder/Index` (`IMSWEB/Controllers/PurchaseOrderController.cs`, `Index`)
   - **Repository/Method:** `IPurchaseOrderService.GetAllPurchaseOrderAsync` → `IMSWEB.Data/Extentions/PurchaseOrderExtensions.GetAllPurchaseOrderAsync`
   - **Suspected issue:** full date-range materialization; `AsNoTracking` missing; no paging.
   - **Estimated before:** 2–6s on large datasets.
   - **After change:** paging + streaming VAT manager window (expected 250–700ms for first page).

4. **Credit sales order list**
   - **Page/Action:** `GET /CreditSalesOrder/Index` (`IMSWEB/Controllers/CreditSalesOrderController.cs`, `Index`)
   - **Repository/Method:** `ICreditSalesOrderService.GetAllSalesOrderAsync` → `IMSWEB.Data/Extentions/CreditSalesOrderExtensions.GetAllSalesOrderAsync`
   - **Suspected issue:** full date-range materialization; `AsNoTracking` missing; no paging.
   - **Estimated before:** 2–7s on large datasets.
   - **After change:** paging + streaming VAT manager window (expected 300–900ms for first page).

5. **Transfer list**
   - **Page/Action:** `GET /Transfer/Index` (`IMSWEB/Controllers/TransferController.cs`, `Index`)
   - **Repository/Method:** `ITransferService.GetAllAsync` → `IMSWEB.Data/Extentions/TransferExtensions.GetAllAsync`
   - **Suspected issue:** full date-range materialization; joins without paging; `AsNoTracking` missing.
   - **Estimated before:** 1–5s on large datasets.
   - **After change:** paging + `AsNoTracking` (expected 200–600ms for first page).

## Baseline Diagnostics Added

- **Request timing filter:** Stopwatch-based action timing + DB duration logging in `IMSWEB/Infrastructure/Core/CoreController.cs`.
- **EF SQL capture:** `DbContext.Database.Log` is routed to a per-request buffer and emitted only when request time exceeds 800ms.
- **DB timing:** `DbCommandInterceptor` totals DB time per request.

