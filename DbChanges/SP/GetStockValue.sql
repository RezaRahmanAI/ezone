IF EXISTS ( SELECT *
            FROM   sys.objects
            WHERE  object_id = OBJECT_ID(N'dbo.GetStockValue')
                   AND type IN ( N'P', N'PC' ) )
BEGIN
   DROP PROCEDURE [dbo].[GetStockValue];
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--------------------------------------------------------------------------------------------------------------------

CREATE PROC [dbo].[GetStockValue]
(
	@asOnDate DATETIME,
	@ConcernID INT

)
  
AS 
BEGIN
	SELECT CAST((piv.PO + piv.POReturn + piv.TransferIn + piv.TransferOut + piv.SO + piv.CrSO + piv.SOReturn + piv.Replacement) AS DECIMAL(18,2)) FROM
(

	SELECT ISNULL(SUM(po.PurchaseRate), 0) PurchaseRate, 'PO' Id FROM
	(
		SELECT 
		SUM((POD.UnitPrice - ((PO.TDiscount + PO.AdjAmount) * POD.UnitPrice) / (PO.GrandTotal - PO.NetDiscount + PO.TDiscount)) * pod.Quantity) PurchaseRate
		--ISNULL(SUM(sd.PRate), 0) PurchaseRate, ISNULL(SUM(pod.Quantity), 0) Quantity 
		
		FROM POrderDetails pod
		JOIN POrders po ON pod.POrderID = po.POrderID
		JOIN Products p ON pod.ProductID = p.ProductID
		JOIN Categorys cat ON p.CategoryID = cat.CategoryID
		JOIN Companies com ON p.CompanyID = com.CompanyID
		JOIN Colors clr ON pod.ColorID = clr.ColorID

		WHERE po.ConcernID = @ConcernID AND po.Status = 1 AND CAST(po.OrderDate AS DATE) <= CAST(@asOnDate AS DATE)
		GROUP BY pod.ProductID, pod.ColorID
	) po

	UNION ALL

	SELECT ISNULL(SUM(por.PurchaseRate), 0) PurchaseRate, 'POReturn' Id FROM
	(
		SELECT 
		SUM(ISNULL(pod.UnitPrice, 0) * ISNULL(-pod.Quantity, 0)) PurchaseRate 
		FROM POrderDetails pod
		JOIN POrders po ON pod.POrderID = po.POrderID
		JOIN Products p ON pod.ProductID = p.ProductID
		JOIN Categorys cat ON p.CategoryID = cat.CategoryID
		JOIN Companies com ON p.CompanyID = com.CompanyID
		JOIN Colors clr ON pod.ColorID = clr.ColorID

		WHERE po.ConcernID = @ConcernID AND po.Status = 5 AND CAST(po.OrderDate AS DATE) <= CAST(@asOnDate AS DATE)
		GROUP BY pod.ProductID, pod.ColorID
	) por

	UNION ALL

	SELECT ISNULL(SUM(tr.PurchaseRate), 0) PurchaseRate, 'TransferIn' Id FROM
	(
		SELECT 
		SUM(ISNULL(td.PRate, 0) * ISNULL(td.Quantity, 0)) PurchaseRate FROM Transfers t
		JOIN TransferDetails td ON t.TransferID = td.TransferID
		JOIN Products p ON td.ToProductID = p.ProductID
		JOIN Colors c ON td.ToColorID = c.ColorID

		WHERE t.ToConcernID = @ConcernID AND t.Status = 1 AND CAST(t.TransferDate AS DATE) <= CAST(@asOnDate AS DATE)
		GROUP BY P.ProductID, c.ColorID
	) tr

	UNION ALL

	SELECT ISNULL(SUM(trO.PurchaseRate), 0) PurchaseRate, 'TransferOut' Id FROM
	(
		SELECT 
		SUM(ISNULL(td.PRate, 0) * ISNULL(-td.Quantity, 0)) PurchaseRate FROM Transfers t
		JOIN TransferDetails td ON t.TransferID = td.TransferID
		JOIN Products p ON td.ProductID = p.ProductID
		JOIN StockDetails sd1 ON td.SDetailID = sd1.SDetailID
		JOIN Colors c ON sd1.ColorID = c.ColorID

		WHERE t.FromConcernID = @ConcernID AND t.Status = 1 AND CAST(t.TransferDate AS DATE) <= CAST(@asOnDate AS DATE)
		GROUP BY P.ProductID, c.ColorID
	) trO

	UNION ALL

	SELECT ISNULL(SUM(so.PurchaseRate), 0) PurchaseRate, 'SO' Id FROM
	(
		SELECT 
		SUM(ISNULL(sd.PRate, 0) * ISNULL(-sod.Quantity, 0))  PurchaseRate FROM SOrders so
		JOIN SOrderDetails sod ON so.SOrderID = sod.SOrderID
		JOIN StockDetails sd ON sod.SDetailID = sd.SDetailID

		WHERE so.ConcernID = @ConcernID AND so.Status = 1 AND CAST(so.InvoiceDate AS DATE) <= CAST(@asOnDate AS DATE)
		GROUP BY sd.ProductID, sd.ColorID
	) so

	UNION ALL

	SELECT ISNULL(SUM(crso.PurchaseRate), 0) PurchaseRate, 'CrSO' Id FROM
	(
		SELECT 
		SUM(ISNULL(sd.PRate, 0) * ISNULL(-csd.Quantity, 0)) PurchaseRate FROM CreditSales cs
		JOIN CreditSaleDetails csd ON cs.CreditSalesID = csd.CreditSalesID
		JOIN StockDetails sd ON csd.StockDetailID = sd.SDetailID

		WHERE cs.ConcernID = @ConcernID AND cs.IsStatus = 1 AND CAST(cs.SalesDate AS DATE) <= CAST(@asOnDate AS DATE)
		GROUP BY sd.ProductID, sd.ColorID
	) crso

	UNION ALL

	SELECT ISNULL(SUM(sor.PurchaseRate), 0) PurchaseRate, 'SOReturn' Id FROM
	(
		SELECT 
		SUM(ISNULL(sd.PRate, 0) * ISNULL(rd.Quantity, 0))  PurchaseRate FROM ROrders r
		JOIN ROrderDetails rd ON r.ROrderID = rd.ROrderID
		JOIN StockDetails sd ON rd.StockDetailID = sd.SDetailID

		WHERE r.ConcernID = @ConcernID AND CAST(r.ReturnDate AS DATE) <= CAST(@asOnDate AS DATE)
		GROUP BY sd.ProductID, sd.ColorID
	) sor

	UNION ALL

	SELECT ISNULL(SUM(rep.PurchaseRate), 0) PurchaseRate, 'Replacement' Id FROM
	(
		SELECT 
		SUM(ISNULL(sd.PRate, 0) * ISNULL(-sod.Quantity, 0)) PurchaseRate FROM SOrders so
		JOIN SOrderDetails sod ON so.SOrderID = sod.SOrderID
		JOIN StockDetails sd ON sod.SDetailID = sd.SDetailID

		WHERE so.ConcernID = @ConcernID AND so.Status = 1 AND sod.RStockDetailId > 0 AND sod.RepOrderID > 0 AND CAST(so.InvoiceDate AS DATE) <= CAST(@asOnDate AS DATE)
		GROUP BY sd.ProductID, sd.ColorID
	) rep

) t1

PIVOT(
		SUM(t1.PurchaseRate)
		FOR t1.Id IN(PO, POReturn, TransferIn, TransferOut, SO, CrSO, SOReturn, Replacement
		)
	) piv
END


--EXEC GetStockValue '2022-08-18', 5126