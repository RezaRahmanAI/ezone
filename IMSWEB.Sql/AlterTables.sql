/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Users ADD
	EmployeeID int NOT NULL CONSTRAINT DF_Users_EmployeeID DEFAULT ((0))
GO
ALTER TABLE dbo.Users SET (LOCK_ESCALATION = TABLE)
GO
COMMIT





--Date: 06-Jun-2018
/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.CreditSales ADD
	LastPayAdjAmt decimal(18, 2) NOT NULL CONSTRAINT DF_CreditSales_LastPayAdjAmt DEFAULT ((0))
GO
ALTER TABLE dbo.CreditSales SET (LOCK_ESCALATION = TABLE)
GO
COMMIT


--Date : 11-APR-2018
/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.CreditSalesSchedules ADD
	HireValue decimal(18, 2) NOT NULL CONSTRAINT DF_CreditSalesSchedules_HireValue DEFAULT ((0)),
	NetValue decimal(18, 2) NOT NULL CONSTRAINT DF_CreditSalesSchedules_NetValue DEFAULT ((0))
GO
ALTER TABLE dbo.CreditSalesSchedules SET (LOCK_ESCALATION = TABLE)
GO
COMMIT




--Date: 26-Mar-2018
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Products
	DROP CONSTRAINT FK_Products_Companies
GO
ALTER TABLE dbo.Companies SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Products
	DROP CONSTRAINT FK_Products_Categorys
GO
ALTER TABLE dbo.Categorys SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Products
	DROP CONSTRAINT DF_Products_PWDiscount
GO
ALTER TABLE dbo.Products
	DROP CONSTRAINT DF_Products_DisDurationFDate
GO
ALTER TABLE dbo.Products
	DROP CONSTRAINT DF_Products_DisDurationToDate
GO
ALTER TABLE dbo.Products
	DROP CONSTRAINT DF_Products_ProductType
GO
ALTER TABLE dbo.Products
	DROP CONSTRAINT DF__Products__Concer__24BD5A91
GO
ALTER TABLE dbo.Products
	DROP CONSTRAINT DF_Products_ProductType_1
GO
CREATE TABLE dbo.Tmp_Products
	(
	ProductID int NOT NULL IDENTITY (1, 1),
	Code varchar(50) NOT NULL,
	ProductName nvarchar(MAX) NOT NULL,
	PicturePath varchar(250) NULL,
	CategoryID int NOT NULL,
	CompanyID int NOT NULL,
	PWDiscount decimal(18, 2) NOT NULL,
	DisDurationFDate datetime NULL,
	DisDurationToDate datetime NULL,
	UnitType int NULL,
	ConcernID int NOT NULL,
	ProductType int NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Products SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_Products ADD CONSTRAINT
	DF_Products_PWDiscount DEFAULT ((0)) FOR PWDiscount
GO
ALTER TABLE dbo.Tmp_Products ADD CONSTRAINT
	DF_Products_DisDurationFDate DEFAULT ('31 Dec 2017') FOR DisDurationFDate
GO
ALTER TABLE dbo.Tmp_Products ADD CONSTRAINT
	DF_Products_DisDurationToDate DEFAULT ('31 Dec 2017') FOR DisDurationToDate
GO
ALTER TABLE dbo.Tmp_Products ADD CONSTRAINT
	DF_Products_ProductType DEFAULT ((1)) FOR UnitType
GO
ALTER TABLE dbo.Tmp_Products ADD CONSTRAINT
	DF__Products__Concer__24BD5A91 DEFAULT ((1)) FOR ConcernID
GO
ALTER TABLE dbo.Tmp_Products ADD CONSTRAINT
	DF_Products_ProductType_2 DEFAULT 1 FOR ProductType
GO
SET IDENTITY_INSERT dbo.Tmp_Products ON
GO
IF EXISTS(SELECT * FROM dbo.Products)
	 EXEC('INSERT INTO dbo.Tmp_Products (ProductID, Code, ProductName, PicturePath, CategoryID, CompanyID, PWDiscount, DisDurationFDate, DisDurationToDate, UnitType, ConcernID)
		SELECT ProductID, Code, ProductName, PicturePath, CategoryID, CompanyID, PWDiscount, DisDurationFDate, DisDurationToDate, UnitType, ConcernID FROM dbo.Products WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_Products OFF
GO
ALTER TABLE dbo.ROProductDetails
	DROP CONSTRAINT FK_ReturnProductDetails_Products
GO
ALTER TABLE dbo.ROrderDetails
	DROP CONSTRAINT FK_ReturnDetails_Products
GO
ALTER TABLE dbo.StockDetails
	DROP CONSTRAINT FK_StockDetails_Products
GO
ALTER TABLE dbo.SOrderDetails
	DROP CONSTRAINT FK_SOrderDetails_Products
GO
ALTER TABLE dbo.POrderDetails
	DROP CONSTRAINT FK_POrderDetails_Products
GO
ALTER TABLE dbo.POProductDetails
	DROP CONSTRAINT FK_POProductDetails_Products
GO
ALTER TABLE dbo.Stocks
	DROP CONSTRAINT FK_Stocks_Products
GO
ALTER TABLE dbo.CreditSaleDetails
	DROP CONSTRAINT FK_CreditSaleProducts_Products
GO
ALTER TABLE dbo.DamageProducts
	DROP CONSTRAINT FK_DamageProducts_Products
GO
ALTER TABLE dbo.SRVisitDetails
	DROP CONSTRAINT FK_SRVisitDetails_Products
GO
ALTER TABLE dbo.SaleOffers
	DROP CONSTRAINT FK_SaleOffers_Products
GO
ALTER TABLE dbo.SRVProductDetails
	DROP CONSTRAINT FK_SRVProductDetails_Products
GO
ALTER TABLE dbo.PriceProtections
	DROP CONSTRAINT FK_PriceProtection_Products
GO
DROP TABLE dbo.Products
GO
EXECUTE sp_rename N'dbo.Tmp_Products', N'Products', 'OBJECT' 
GO
ALTER TABLE dbo.Products ADD CONSTRAINT
	PK_Products PRIMARY KEY CLUSTERED 
	(
	ProductID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Products ADD CONSTRAINT
	FK_Products_Categorys FOREIGN KEY
	(
	CategoryID
	) REFERENCES dbo.Categorys
	(
	CategoryID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Products ADD CONSTRAINT
	FK_Products_Companies FOREIGN KEY
	(
	CompanyID
	) REFERENCES dbo.Companies
	(
	CompanyID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.PriceProtections ADD CONSTRAINT
	FK_PriceProtection_Products FOREIGN KEY
	(
	ProductID
	) REFERENCES dbo.Products
	(
	ProductID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.PriceProtections SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.SRVProductDetails ADD CONSTRAINT
	FK_SRVProductDetails_Products FOREIGN KEY
	(
	ProductID
	) REFERENCES dbo.Products
	(
	ProductID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.SRVProductDetails SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.SaleOffers ADD CONSTRAINT
	FK_SaleOffers_Products FOREIGN KEY
	(
	ProductID
	) REFERENCES dbo.Products
	(
	ProductID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.SaleOffers SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.SRVisitDetails ADD CONSTRAINT
	FK_SRVisitDetails_Products FOREIGN KEY
	(
	ProductID
	) REFERENCES dbo.Products
	(
	ProductID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.SRVisitDetails SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.DamageProducts ADD CONSTRAINT
	FK_DamageProducts_Products FOREIGN KEY
	(
	ProductID
	) REFERENCES dbo.Products
	(
	ProductID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.DamageProducts SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.CreditSaleDetails ADD CONSTRAINT
	FK_CreditSaleProducts_Products FOREIGN KEY
	(
	ProductID
	) REFERENCES dbo.Products
	(
	ProductID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.CreditSaleDetails SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Stocks ADD CONSTRAINT
	FK_Stocks_Products FOREIGN KEY
	(
	ProductID
	) REFERENCES dbo.Products
	(
	ProductID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Stocks SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.POProductDetails ADD CONSTRAINT
	FK_POProductDetails_Products FOREIGN KEY
	(
	ProductID
	) REFERENCES dbo.Products
	(
	ProductID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.POProductDetails SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.POrderDetails ADD CONSTRAINT
	FK_POrderDetails_Products FOREIGN KEY
	(
	ProductID
	) REFERENCES dbo.Products
	(
	ProductID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.POrderDetails SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.SOrderDetails ADD CONSTRAINT
	FK_SOrderDetails_Products FOREIGN KEY
	(
	ProductID
	) REFERENCES dbo.Products
	(
	ProductID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.SOrderDetails SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.StockDetails ADD CONSTRAINT
	FK_StockDetails_Products FOREIGN KEY
	(
	ProductID
	) REFERENCES dbo.Products
	(
	ProductID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.StockDetails SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.ROrderDetails ADD CONSTRAINT
	FK_ReturnDetails_Products FOREIGN KEY
	(
	ProductID
	) REFERENCES dbo.Products
	(
	ProductID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.ROrderDetails SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.ROProductDetails ADD CONSTRAINT
	FK_ReturnProductDetails_Products FOREIGN KEY
	(
	ProductID
	) REFERENCES dbo.Products
	(
	ProductID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.ROProductDetails SET (LOCK_ESCALATION = TABLE)
GO
COMMIT