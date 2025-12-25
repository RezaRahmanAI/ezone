IF EXISTS ( SELECT *
            FROM   sys.objects
            WHERE  object_id = OBJECT_ID(N'dbo.GetCustomerDueByDate')
                   AND type IN ( N'P', N'PC' ) )
BEGIN
   DROP PROCEDURE [dbo].[GetCustomerDueByDate];
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--------------------------------------------------------------------------------------------------------------------

CREATE PROC [dbo].[GetCustomerDueByDate]
(
	@asOnDate DATETIME,
	@ConcernID INT

)
  
AS 
BEGIN
	DECLARE @OpeningDue DECIMAL(18, 2) = 0.0

	SET @OpeningDue = (SELECT SUM(c.OpeningDue) OpeningDue FROM Customers c WHERE c.ConcernID = @ConcernID AND c.CreditDue + c.TotalDue  != 0)

	SELECT 
		CAST((
			(@OpeningDue + piv.CashCollectionReturn)
			 + 
			(
				((piv.TotalSales + piv.TotalCrSales) - piv.SalesReturn) - 
				(piv.DownPayment + piv.CashCollection + piv.InstallmentCollection + piv.BankCollection + piv.RecAmount - piv.SalesReturnCashBack + (piv.Adjustment + piv.CrAdjustment) + piv.[Return])
			)
		) AS DECIMAL(18, 2))

	FROM
	(

	SELECT ISNULL(SUM(ISNULL(s.TotalAmount, 0)), 0) PrevSales, 'TotalSales' Id FROM SOrders s
	JOIN Customers c ON s.CustomerID = c.CustomerID
	WHERE s.ConcernID = @ConcernID AND s.Status = 1
	AND c.ConcernID = @ConcernID AND c.CreditDue + c.TotalDue  != 0
	AND CAST(s.InvoiceDate AS DATE) <= CAST(@asOnDate AS DATE)

	UNION ALL
	SELECT ISNULL(SUM(ISNULL(cs.NetAmount, 0)), 0) PrevSales, 'TotalCrSales' Id FROM CreditSales cs
	JOIN Customers c ON cs.CustomerID = c.CustomerID
	WHERE cs.ConcernID = @ConcernID 
	AND c.ConcernID = @ConcernID AND c.CreditDue + c.TotalDue  != 0 AND cs.IsStatus = 1
	AND CAST(cs.SalesDate AS DATE) <=  CAST(@asOnDate AS DATE)


	--UNION ALL

	--SELECT ISNULL(SUM(ISNULL(s.TotalAmount, 0)), 0), 'PreviousSalesReturn' Id FROM SOrders s
	--JOIN Customers c ON s.CustomerID = c.CustomerID
	--WHERE s.ConcernID = @ConcernID AND s.Status = 4 
	--AND c.ConcernID = @ConcernID AND c.CreditDue + c.TotalDue  != 0
	--AND CAST(s.InvoiceDate AS DATE) < CAST(DATEADD(DAY, -1, @asOnDate) AS DATE)

	UNION ALL
	SELECT ISNULL(SUM(ISNULL(s.TotalAmount, 0)), 0), 'SalesReturn' Id FROM SOrders s
	JOIN Customers c ON s.CustomerID = c.CustomerID
	WHERE s.ConcernID = @ConcernID 
	AND c.ConcernID = @ConcernID AND c.CreditDue + c.TotalDue  != 0
	AND s.Status = 4 AND CAST(s.InvoiceDate AS DATE) <= CAST(@asOnDate AS DATE)

	--UNION ALL
	--SELECT ISNULL(SUM(ISNULL(s.RecAmount, 0)), 0) PrevSales, 'PrevRecAmount' Id FROM SOrders s
	--JOIN Customers c ON s.CustomerID = c.CustomerID
	--WHERE s.ConcernID = @ConcernID 
	--AND c.ConcernID = @ConcernID AND c.CreditDue + c.TotalDue  != 0
	--AND s.Status = 1 AND CAST(s.InvoiceDate AS DATE) < CAST(DATEADD(DAY, -1, @asOnDate) AS DATE)

	UNION ALL

	SELECT ISNULL(SUM(ISNULL(s.RecAmount, 0)), 0) PrevSales, 'RecAmount' Id FROM SOrders s
	JOIN Customers c ON s.CustomerID = c.CustomerID
	WHERE s.ConcernID = @ConcernID 
	AND c.ConcernID = @ConcernID AND c.CreditDue + c.TotalDue  != 0
	AND s.Status = 1 AND CAST(s.InvoiceDate AS DATE) <= CAST(@asOnDate AS DATE)

	--UNION ALL
	--SELECT ISNULL(SUM(ISNULL(cs.DownPayment, 0)), 0) PrevSales, 'PrevDownPayment' Id FROM CreditSales cs
	--JOIN Customers c ON cs.CustomerID = c.CustomerID
	--WHERE cs.ConcernID = @ConcernID 
	--AND c.ConcernID = @ConcernID AND c.CreditDue + c.TotalDue  != 0 AND cs.IsStatus = 1
	--AND CAST(cs.SalesDate AS DATE) < CAST(DATEADD(DAY, -1, @asOnDate) AS DATE)

	UNION ALL
	SELECT ISNULL(SUM(ISNULL(cs.DownPayment, 0)), 0) PrevSales, 'DownPayment' Id FROM CreditSales cs
	JOIN Customers c ON cs.CustomerID = c.CustomerID
	WHERE cs.ConcernID = @ConcernID 
	AND c.ConcernID = @ConcernID AND c.CreditDue + c.TotalDue  != 0 AND cs.IsStatus = 1
	AND CAST(cs.SalesDate AS DATE) <= CAST(@asOnDate AS DATE)

	--UNION ALL
	--SELECT ISNULL(SUM(ISNULL(cc.Amount, 0)), 0) PrevSales, 'PrevCashCollection' Id FROM CashCollections cc
	--JOIN Customers c ON cc.CustomerID = c.CustomerID
	--WHERE cc.ConcernID = @ConcernID 
	--AND cc.TransactionType = 1 AND c.ConcernID = @ConcernID AND c.CreditDue + c.TotalDue  != 0
	--AND CAST(cc.EntryDate AS DATE) < CAST(DATEADD(DAY, -1, @asOnDate) AS DATE)

	UNION ALL
	SELECT ISNULL(SUM(ISNULL(cc.Amount, 0)), 0) PrevSales, 'CashCollection' Id FROM CashCollections cc
	JOIN Customers c ON cc.CustomerID = c.CustomerID
	WHERE cc.ConcernID = @ConcernID
	AND cc.TransactionType =1 AND c.ConcernID = @ConcernID AND c.CreditDue + c.TotalDue  != 0
	AND CAST(cc.EntryDate AS DATE) <= CAST(@asOnDate AS DATE)


	--UNION ALL
	--SELECT ISNULL(SUM(ISNULL(css.InstallmentAmt, 0)), 0) PrevSales, 'PrevInstallmentCollection' Id FROM CreditSales cs
	--JOIN CreditSalesSchedules css ON cs.CreditSalesID = css.CreditSalesID
	--JOIN Customers c ON cs.CustomerID = c.CustomerID
	--WHERE cs.ConcernID = @ConcernID 
	--AND c.ConcernID = @ConcernID AND c.CreditDue + c.TotalDue  != 0 AND cs.IsStatus = 1 AND css.PaymentStatus = 'Paid'
	--AND CAST(css.PaymentDate AS DATE) < CAST(DATEADD(DAY, -1, @asOnDate) AS DATE)

	UNION ALL
	SELECT ISNULL(SUM(ISNULL(css.InstallmentAmt, 0)), 0) PrevSales, 'InstallmentCollection' Id FROM CreditSales cs
	JOIN CreditSalesSchedules css ON cs.CreditSalesID = css.CreditSalesID
	JOIN Customers c ON cs.CustomerID = c.CustomerID
	WHERE cs.ConcernID = @ConcernID 
	AND c.ConcernID = @ConcernID AND c.CreditDue + c.TotalDue  != 0 AND cs.IsStatus = 1 AND css.PaymentStatus = 'Paid'
	AND CAST(css.PaymentDate AS DATE) <= CAST(@asOnDate AS DATE)

	--UNION ALL
	--SELECT ISNULL(SUM(ISNULL(bt.Amount, 0)), 0.00) PrevSales, 'PrevBankCollection' Id FROM BankTransactions bt
	--JOIN Customers c ON bt.CustomerID = c.CustomerID
	--WHERE bt.ConcernID = @ConcernID 
	--AND bt.TransactionType = 3 AND c.ConcernID = @ConcernID AND c.CreditDue + c.TotalDue  != 0
	--AND CAST(bt.TranDate AS DATE) < CAST(DATEADD(DAY, -1, @asOnDate) AS DATE)

	UNION ALL
	SELECT ISNULL(SUM(ISNULL(bt.Amount, 0)), 0) PrevSales, 'BankCollection' Id FROM BankTransactions bt
	JOIN Customers c ON bt.CustomerID = c.CustomerID
	WHERE bt.ConcernID = @ConcernID 
	AND bt.TransactionType = 3 AND c.ConcernID = @ConcernID AND c.CreditDue + c.TotalDue  != 0
	AND CAST(bt.TranDate AS DATE) <= CAST(@asOnDate AS DATE)

	--UNION ALL

	--SELECT ISNULL(SUM(ISNULL(s.RecAmount, 0)), 0), 'PrevSalesReturnCashBack' Id FROM SOrders s
	--JOIN Customers c ON s.CustomerID = c.CustomerID
	--WHERE s.ConcernID = @ConcernID AND s.Status = 4 
	--AND c.ConcernID = @ConcernID AND c.CreditDue + c.TotalDue  != 0
	--AND CAST(s.InvoiceDate AS DATE) < CAST(DATEADD(DAY, -1, @asOnDate) AS DATE)


	UNION ALL

	SELECT ISNULL(SUM(ISNULL(s.RecAmount, 0)), 0), 'SalesReturnCashBack' Id FROM SOrders s
	JOIN Customers c ON s.CustomerID = c.CustomerID
	WHERE s.ConcernID = @ConcernID AND s.Status = 4 
	AND c.ConcernID = @ConcernID AND c.CreditDue + c.TotalDue  != 0
	AND CAST(s.InvoiceDate AS DATE) <= CAST(@asOnDate AS DATE)

	--UNION ALL
	--SELECT ISNULL(SUM(ISNULL(css.LastPayAdjust, 0)), 0) PrevSales, 'PrevCrAdjustment' Id FROM CreditSales cs
	--JOIN CreditSalesSchedules css ON cs.CreditSalesID = css.CreditSalesID
	--JOIN Customers c ON cs.CustomerID = c.CustomerID
	--WHERE cs.ConcernID = @ConcernID 
	--AND c.ConcernID = @ConcernID AND c.CreditDue + c.TotalDue  != 0 AND css.PaymentStatus = 'Paid'
	--AND CAST(css.PaymentDate AS DATE) < CAST(DATEADD(DAY, -1, @asOnDate) AS DATE)

	UNION ALL
	SELECT ISNULL(SUM(ISNULL(css.LastPayAdjust, 0)), 0) PrevSales, 'CrAdjustment' Id FROM CreditSales cs
	JOIN CreditSalesSchedules css ON cs.CreditSalesID = css.CreditSalesID
	JOIN Customers c ON cs.CustomerID = c.CustomerID
	WHERE cs.ConcernID = @ConcernID 
	AND c.ConcernID = @ConcernID AND c.CreditDue + c.TotalDue  != 0 AND css.PaymentStatus = 'Paid'
	AND CAST(css.PaymentDate AS DATE) <= CAST(@asOnDate AS DATE)

	--UNION ALL
	--SELECT ISNULL(SUM(ISNULL(cc.AdjustAmt, 0)), 0) PrevSales, 'PrevAdjustment' Id FROM CashCollections cc
	--JOIN Customers c ON cc.CustomerID = c.CustomerID
	--WHERE cc.ConcernID = @ConcernID 
	--AND c.ConcernID = @ConcernID AND c.CreditDue + c.TotalDue  != 0 AND cc.TransactionType IN(1)
	--AND CAST(cc.EntryDate AS DATE) < CAST(DATEADD(DAY, -1, @asOnDate) AS DATE)


	UNION ALL
	SELECT ISNULL(SUM(ISNULL(cc.AdjustAmt, 0)), 0) PrevSales, 'Adjustment' Id FROM CashCollections cc
	JOIN Customers c ON cc.CustomerID = c.CustomerID
	WHERE cc.ConcernID = @ConcernID 
	AND c.ConcernID = @ConcernID AND c.CreditDue + c.TotalDue  != 0 AND cc.TransactionType IN(1)
	AND CAST(cc.EntryDate AS DATE) <= CAST(@asOnDate AS DATE)

	--UNION ALL
	--SELECT ISNULL(SUM(r.GrandTotal - r.PaidAmount), 0), 'PrevReturn' FROM ROrders r
	--JOIN Customers c ON r.CustomerID = c.CustomerID AND c.CreditDue + c.TotalDue  != 0
	--WHERE r.ConcernID = 5126 AND CAST(r.ReturnDate AS DATE) < CAST(DATEADD(DAY, -1, @asOnDate) AS DATE)

	UNION ALL
	SELECT ISNULL(SUM(r.GrandTotal - r.PaidAmount), 0), 'Return' FROM ROrders r
	JOIN Customers c ON r.CustomerID = c.CustomerID
	WHERE r.ConcernID = @ConcernID
	AND c.ConcernID = @ConcernID AND c.CreditDue + c.TotalDue  != 0
	AND CAST(r.ReturnDate AS DATE) <= CAST(@asOnDate AS DATE)

	UNION ALL
	SELECT ISNULL(SUM(ISNULL(cc.Amount, 0)), 0) PrevSales, 'CashCollectionReturn' Id FROM CashCollections cc
	JOIN Customers c ON cc.CustomerID = c.CustomerID
	WHERE cc.ConcernID = @ConcernID 
	AND cc.TransactionType IN(3) AND c.ConcernID = @ConcernID AND c.CreditDue + c.TotalDue  != 0
	AND CAST(cc.EntryDate AS DATE) <= CAST(@asOnDate AS DATE)

	) t1

	PIVOT(
		SUM(t1.PrevSales)
		FOR t1.Id IN( TotalSales, TotalCrSales, SalesReturn, RecAmount,
		DownPayment, CashCollection, InstallmentCollection,BankCollection,
		SalesReturnCashBack, CrAdjustment, Adjustment, [Return], CashCollectionReturn
		)
	) piv

	--SELECT SUM(c.TotalDue + c.CreditDue) FROM Customers c
	--WHERE c.ConcernID = @ConcernID
END


--EXEC GetCustomerDueByDate '2022-08-18', 5126