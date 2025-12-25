IF EXISTS ( SELECT *
            FROM   sys.objects
            WHERE  object_id = OBJECT_ID(N'dbo.GetBankBalance')
                   AND type IN ( N'P', N'PC' ) )
BEGIN
   DROP PROCEDURE [dbo].[GetBankBalance];
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--------------------------------------------------------------------------------------------------------------------

CREATE PROC [dbo].[GetBankBalance]
(
	@asOnDate DATETIME,
	@ConcernID INT

)
  
AS 
BEGIN
	DECLARE
			@openingBal DECIMAL(18, 2),
			@fundIn DECIMAL(18, 2),
			@fundOut DECIMAL(18, 2),
			@anotherFundIn DECIMAL(18, 2)

	SET @openingBal = CAST((
		SELECT ISNULL(SUM(b.OpeningBalance),0) FROM Banks b
		WHERE b.ConcernID IN(SELECT sc.ConcernID FROM SisterConcerns sc WHERE sc.ConcernID = @ConcernID OR sc.ParentID IN(@ConcernID))
	) AS DECIMAL(18,2))


	SET @fundIn = CAST(
		(
			SELECT ISNULL(SUM(bt.Amount),0) FROM BankTransactions bt
			JOIN Banks b ON bt.BankID = b.BankID
			WHERE bt.ConcernID IN(SELECT sc.ConcernID FROM SisterConcerns sc WHERE sc.ConcernID = @ConcernID OR sc.ParentID IN(@ConcernID)) AND
			bt.TransactionType IN(1, 3, 9, 7) AND
			CAST(bt.TranDate AS DATE) <= CAST(@asOnDate AS DATE)
		) AS DECIMAL(18, 2)
	)

	SET @anotherFundIn = CAST(
		(
			SELECT ISNULL(SUM(bt.Amount),0) FROM BankTransactions bt
			JOIN Banks b ON bt.AnotherBankID = b.BankID
			WHERE bt.ConcernID IN(SELECT sc.ConcernID FROM SisterConcerns sc WHERE sc.ConcernID = @ConcernID OR sc.ParentID IN(@ConcernID))
			AND bt.TransactionType IN(5) 
			AND CAST(bt.TranDate AS DATE) <= CAST(@asOnDate AS DATE)
		) AS DECIMAL(18, 2)
	)

	SET @fundOut = CAST(
		(
			SELECT ISNULL(SUM(bt.Amount),0) FROM BankTransactions bt
			JOIN Banks b ON bt.BankID = b.BankID
			WHERE bt.ConcernID IN(SELECT sc.ConcernID FROM SisterConcerns sc WHERE sc.ConcernID = @ConcernID OR sc.ParentID IN(@ConcernID)) AND
			bt.TransactionType IN(2, 4, 8, 6, 5) AND
			CAST(bt.TranDate AS DATE) <= CAST(@asOnDate AS DATE)
		) AS DECIMAL(18, 2)
	)

	SELECT @openingBal + @fundIn + @anotherFundIn - @fundOut
END


--EXEC GetBankBalance '2022-09-26', 5126