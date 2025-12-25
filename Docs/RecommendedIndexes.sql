/*
Optional index recommendations based on list/report filters and sorts.
Review with DBA before applying. These are additive only.
*/

-- Sales orders (SalesOrderExtensions: InvoiceDate range + Status + CustomerID)
CREATE NONCLUSTERED INDEX IX_SOrders_Status_InvoiceDate
ON dbo.SOrders (Status, InvoiceDate)
INCLUDE (CustomerID, InvoiceNo, TotalAmount, EditReqStatus);

-- Credit sales orders (CreditSalesOrderExtensions: SalesDate range + IsStatus + CustomerID)
CREATE NONCLUSTERED INDEX IX_CreditSales_IsStatus_SalesDate
ON dbo.CreditSales (IsStatus, SalesDate)
INCLUDE (CustomerID, InvoiceNo, NetAmount, Remaining, IsReturn);

-- Purchase orders (PurchaseOrderExtensions: Status + OrderDate + SupplierID)
CREATE NONCLUSTERED INDEX IX_POrders_Status_OrderDate
ON dbo.POrders (Status, OrderDate)
INCLUDE (SupplierID, ChallanNo, TotalAmt, EditReqStatus);

-- Transfers (TransferExtensions: TransferDate range + To/From concern)
CREATE NONCLUSTERED INDEX IX_Transfers_TransferDate_Concern
ON dbo.Transfers (TransferDate, FromConcernID, ToConcernID)
INCLUDE (TransferNo, TotalAmount, Status);

-- Stock details lookup (StockExtensions: StockDetail lookups by ProductID, ColorID, GodownID, Status)
CREATE NONCLUSTERED INDEX IX_StockDetails_Product_Color_Godown_Status
ON dbo.StockDetails (ProductID, ColorID, GodownID, Status)
INCLUDE (SRate, CreditSRate, CRSalesRate3Month, CRSalesRate12Month);
