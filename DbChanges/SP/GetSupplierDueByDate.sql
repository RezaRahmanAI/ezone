IF EXISTS ( SELECT *
            FROM   sys.objects
            WHERE  object_id = OBJECT_ID(N'dbo.GetSupplierDueByDate')
                   AND type IN ( N'P', N'PC' ) )
BEGIN
   DROP PROCEDURE [dbo].[GetSupplierDueByDate];
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--------------------------------------------------------------------------------------------------------------------

CREATE PROC [dbo].[GetSupplierDueByDate]
(
	@asOnDate DATETIME,
	@ConcernID INT

)
  
AS 
BEGIN
	DECLARE @OpeningDue DECIMAL(18, 2) = 0.0

	SET @OpeningDue = (SELECT SUM(s.OpeningDue) OpeningDue FROM Suppliers s WHERE s.ConcernID = @ConcernID)

	SELECT 
		CAST((
			(@OpeningDue) + 
			(
				(piv.TotalPurchase - piv.POReturn + piv.CashCollectionInterest) - 
				(piv.CashCollection  + piv.BankCollection + piv.RecAmount - piv.POReturnCashBack + piv.Adjustment)
			)
		) AS DECIMAL(18, 2))

	FROM
	(
	--SELECT ISNULL(SUM(ISNULL(P.TotalAmt, 0)), 0) PrevPurchase, 'PreviousPurchase' Id FROM POrders p
	--JOIN Suppliers s ON p.SupplierID = s.SupplierID
	--WHERE p.ConcernID = @ConcernID AND p.Status = 1 AND CAST(p.OrderDate AS DATE) < CAST(DATEADD(DAY, -1, @asOnDate) AS DATE)

	--UNION ALL

	SELECT ISNULL(SUM(ISNULL(P.TotalAmt, 0)), 0) PrevPurchase, 'TotalPurchase' Id FROM POrders p
	JOIN Suppliers s ON p.SupplierID = s.SupplierID
	WHERE p.ConcernID IN(SELECT sc.ConcernID FROM SisterConcerns sc WHERE sc.ConcernID = @ConcernID OR sc.ParentID IN(@ConcernID)) AND p.Status = 1 
	AND CAST(p.OrderDate AS DATE) <= CAST(@asOnDate AS DATE)
	--UNION ALL

	--SELECT ISNULL(SUM(ISNULL(P.TotalAmt, 0)), 0) PrevPurchase, 'PreviousPOReturn' Id FROM POrders p
	--JOIN Suppliers s ON p.SupplierID = s.SupplierID
	--WHERE p.ConcernID = @ConcernID AND p.Status = 5 AND CAST(p.OrderDate AS DATE) < CAST(DATEADD(DAY, -1, @asOnDate) AS DATE)

	UNION ALL
	SELECT ISNULL(SUM(ISNULL(P.TotalAmt, 0)), 0) PrevPurchase, 'POReturn' Id FROM POrders p
	JOIN Suppliers s ON p.SupplierID = s.SupplierID
	WHERE p.ConcernID IN(SELECT sc.ConcernID FROM SisterConcerns sc WHERE sc.ConcernID = @ConcernID OR sc.ParentID IN(@ConcernID)) AND p.Status = 5 
	AND CAST(p.OrderDate AS DATE) <= CAST(@asOnDate AS DATE)
	--UNION ALL
	--SELECT ISNULL(SUM(ISNULL(P.RecAmt, 0)), 0) PrevPurchase, 'PrevRecAmount' Id FROM POrders p
	--JOIN Suppliers s ON p.SupplierID = s.SupplierID
	--WHERE p.ConcernID = @ConcernID AND p.Status = 1 AND CAST(p.OrderDate AS DATE) < CAST(DATEADD(DAY, -1, @asOnDate) AS DATE)

	UNION ALL

	SELECT ISNULL(SUM(ISNULL(P.RecAmt, 0)), 0) PrevPurchase, 'RecAmount' Id FROM POrders p
	JOIN Suppliers s ON p.SupplierID = s.SupplierID
	WHERE p.ConcernID IN(SELECT sc.ConcernID FROM SisterConcerns sc WHERE sc.ConcernID = @ConcernID OR sc.ParentID IN(@ConcernID)) AND p.Status = 1 
	AND CAST(p.OrderDate AS DATE) <= CAST(@asOnDate AS DATE)
	--UNION ALL
	--SELECT ISNULL(SUM(ISNULL(cc.Amount, 0)), 0) PrevSales, 'PrevCashCollection' Id FROM CashCollections cc
	--JOIN Suppliers s ON cc.SupplierID = s.SupplierID
	--WHERE cc.ConcernID = @ConcernID AND CAST(cc.EntryDate AS DATE) < CAST(DATEADD(DAY, -1, @asOnDate) AS DATE)

	UNION ALL
	SELECT ISNULL(SUM(ISNULL(cc.Amount, 0)), 0) PrevSales, 'CashCollection' Id FROM CashCollections cc
	JOIN Suppliers s ON cc.SupplierID = s.SupplierID
	WHERE cc.ConcernID IN(SELECT sc.ConcernID FROM SisterConcerns sc WHERE sc.ConcernID = @ConcernID OR sc.ParentID IN(@ConcernID))  AND cc.TransactionType = 2
	AND CAST(cc.EntryDate AS DATE) <= CAST(@asOnDate AS DATE)

	UNION ALL
	SELECT ISNULL(SUM(ISNULL(cc.InterestAmt, 0)), 0) PrevSales, 'CashCollectionInterest' Id FROM CashCollections cc
	JOIN Suppliers s ON cc.SupplierID = s.SupplierID
	WHERE cc.ConcernID IN(SELECT sc.ConcernID FROM SisterConcerns sc WHERE sc.ConcernID = @ConcernID OR sc.ParentID IN(@ConcernID))  AND cc.TransactionType = 2
	AND CAST(cc.EntryDate AS DATE) <= CAST(@asOnDate AS DATE)

	UNION ALL
	SELECT ISNULL(SUM(ISNULL(cc.Amount, 0)), 0) PrevSales, 'CashCollectionReturn' Id FROM CashCollections cc
	JOIN Suppliers s ON cc.SupplierID = s.SupplierID
	WHERE cc.ConcernID IN(SELECT sc.ConcernID FROM SisterConcerns sc WHERE sc.ConcernID = @ConcernID OR sc.ParentID IN(@ConcernID))  AND cc.TransactionType = 3
	AND CAST(cc.EntryDate AS DATE) <= CAST(@asOnDate AS DATE)

	--UNION ALL
	--SELECT ISNULL(SUM(ISNULL(bt.Amount, 0)), 0.00) PrevSales, 'PrevBankCollection' Id FROM BankTransactions bt
	--JOIN Suppliers s ON bt.SupplierID = s.SupplierID
	--WHERE bt.ConcernID = @ConcernID AND CAST(bt.TranDate AS DATE) < CAST(DATEADD(DAY, -1, @asOnDate) AS DATE)

	UNION ALL
	SELECT ISNULL(SUM(ISNULL(bt.Amount, 0)), 0) PrevSales, 'BankCollection' Id FROM BankTransactions bt
	JOIN Suppliers s ON bt.SupplierID = s.SupplierID
	WHERE bt.ConcernID IN(SELECT sc.ConcernID FROM SisterConcerns sc WHERE sc.ConcernID = @ConcernID OR sc.ParentID IN(@ConcernID))
	AND CAST(bt.TranDate AS DATE) <= CAST(@asOnDate AS DATE)

	--UNION ALL
	--SELECT ISNULL(SUM(ISNULL(P.RecAmt, 0)), 0) PrevPurchase, 'PrevPOReturnCashBack' Id FROM POrders p
	--JOIN Suppliers s ON p.SupplierID = s.SupplierID
	--WHERE p.ConcernID = @ConcernID AND p.Status = 5 AND CAST(p.OrderDate AS DATE) < CAST(DATEADD(DAY, -1, @asOnDate) AS DATE)

	UNION ALL

	SELECT ISNULL(SUM(ISNULL(P.RecAmt, 0)), 0) PrevPurchase, 'POReturnCashBack' Id FROM POrders p
	JOIN Suppliers s ON p.SupplierID = s.SupplierID
	WHERE p.ConcernID IN(SELECT sc.ConcernID FROM SisterConcerns sc WHERE sc.ConcernID = @ConcernID OR sc.ParentID IN(@ConcernID)) AND p.Status = 5 
	AND CAST(p.OrderDate AS DATE) <= CAST(@asOnDate AS DATE)

	--UNION ALL
	--SELECT ISNULL(SUM(ISNULL(cc.AdjustAmt, 0)), 0) PrevSales, 'PrevAdjustment' Id FROM CashCollections cc
	--JOIN Suppliers s ON cc.SupplierID = s.SupplierID
	--WHERE cc.ConcernID = @ConcernID AND CAST(cc.EntryDate AS DATE) < CAST(DATEADD(DAY, -1, @asOnDate) AS DATE)

	UNION ALL
	SELECT ISNULL(SUM(ISNULL(cc.AdjustAmt, 0)), 0) PrevSales, 'Adjustment' Id FROM CashCollections cc
	JOIN Suppliers s ON cc.SupplierID = s.SupplierID
	WHERE cc.ConcernID IN(SELECT sc.ConcernID FROM SisterConcerns sc WHERE sc.ConcernID = @ConcernID OR sc.ParentID IN(@ConcernID)) 
	AND CAST(cc.EntryDate AS DATE) <= CAST(@asOnDate AS DATE)
	) t1

	PIVOT(
		SUM(t1.PrevPurchase)
		FOR t1.Id IN(TotalPurchase, POReturn, RecAmount, CashCollection,CashCollectionInterest, CashCollectionReturn,
		BankCollection, POReturnCashBack,Adjustment
		)
	) piv
END


--EXEC GetSupplierDueByDate '2022-08-18', 5126