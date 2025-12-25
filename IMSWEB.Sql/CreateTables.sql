CREATE TABLE [dbo].[Companies](
	[CompanyID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](250) NOT NULL,
	[Name] [varchar](350) NOT NULL,
	[ConcernID] [int] NOT NULL,
 CONSTRAINT [PK_Companies] PRIMARY KEY CLUSTERED 
(
	[CompanyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Companies]  WITH CHECK ADD  CONSTRAINT [FK_Companies_SisterConcerns] FOREIGN KEY([ConcernID])
REFERENCES [dbo].[SisterConcerns] ([ConcernID])
GO

ALTER TABLE [dbo].[Companies] CHECK CONSTRAINT [FK_Companies_SisterConcerns]
GO

----------------------------------------For Categorys----------------------------------------------------------------------------------

CREATE TABLE [dbo].[Categorys](
	[CategoryID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](150) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[ConcernID] [int] NOT NULL,
 CONSTRAINT [PK_Categorys] PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Categorys]  WITH CHECK ADD  CONSTRAINT [FK_Categorys_SisterConcerns] FOREIGN KEY([ConcernID])
REFERENCES [dbo].[SisterConcerns] ([ConcernID])
GO

ALTER TABLE [dbo].[Categorys] CHECK CONSTRAINT [FK_Categorys_SisterConcerns]
GO

-----------------------------------------------------------For Color--------------------------------------------------------------------------
CREATE TABLE [dbo].[Colors](
	[ColorID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](250) NOT NULL,
	[Code] [varchar](250) NOT NULL,
	[ConcernID] [int] NOT NULL,
 CONSTRAINT [PK_Colors] PRIMARY KEY CLUSTERED 
(
	[ColorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Colors]  WITH CHECK ADD  CONSTRAINT [FK_Colors_SisterConcerns] FOREIGN KEY([ConcernID])
REFERENCES [dbo].[SisterConcerns] ([ConcernID])
GO

ALTER TABLE [dbo].[Colors] CHECK CONSTRAINT [FK_Colors_SisterConcerns]
GO
-------------------------------------------------------For Product---------------------------------------------------------------------


CREATE TABLE [dbo].[Products](
	[ProductID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](50) NOT NULL,
	[ProductName] [nvarchar](max) NOT NULL,
	[PicturePath] [varchar](250) NULL,
	[CategoryID] [int] NOT NULL,
	[CompanyID] [int] NOT NULL,
	[PWDiscount] [decimal](18, 2) NOT NULL,
	[DisDurationFDate] [datetime] NULL,
	[DisDurationToDate] [datetime] NULL,
	[UnitType] [int] NULL,
	[ConcernID] [int] NOT NULL,
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[ProductID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Products] ADD  CONSTRAINT [DF_Products_PWDiscount]  DEFAULT ((0)) FOR [PWDiscount]
GO

ALTER TABLE [dbo].[Products] ADD  CONSTRAINT [DF_Products_DisDurationFDate]  DEFAULT ('31 Dec 2017') FOR [DisDurationFDate]
GO

ALTER TABLE [dbo].[Products] ADD  CONSTRAINT [DF_Products_DisDurationToDate]  DEFAULT ('31 Dec 2017') FOR [DisDurationToDate]
GO

ALTER TABLE [dbo].[Products] ADD  CONSTRAINT [DF_Products_ProductType]  DEFAULT ((1)) FOR [UnitType]
GO

ALTER TABLE [dbo].[Products] ADD  DEFAULT ((1)) FOR [ConcernID]
GO

ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Categorys] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[Categorys] ([CategoryID])
GO

ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Categorys]
GO

ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Companies] FOREIGN KEY([CompanyID])
REFERENCES [dbo].[Companies] ([CompanyID])
GO

ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Companies]
GO

------------------------------------------------For Designation--------------------------------------------------------------------


CREATE TABLE [dbo].[Designations](
	[DesignationID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](150) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Designations] PRIMARY KEY CLUSTERED 
(
	[DesignationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


-------------------------------------------For Employee --------------------------------------------------------------------------



CREATE TABLE [dbo].[Employees](
	[EmployeeID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](150) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[FName] [nvarchar](max) NULL,
	[MName] [nvarchar](max) NULL,
	[ContactNo] [varchar](150) NULL,
	[EmailID] [varchar](150) NULL,
	[NID] [varchar](150) NULL,
	[BloodGroup] [varchar](50) NULL,
	[JoiningDate] [datetime] NULL,
	[PresentAdd] [nvarchar](max) NULL,
	[PermanentAdd] [nvarchar](max) NULL,
	[DesignationID] [int] NOT NULL,
	[GrossSalary] [decimal](18, 2) NOT NULL,
	[PhotoPath] [varchar](250) NULL,
	[DOB] [datetime] NULL,
	[SRDueLimit] [decimal](18, 2) NOT NULL,
	[ConcernID] [int] NOT NULL,
 CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED 
(
	[EmployeeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Employees] ADD  CONSTRAINT [DF_Employees_GrossSalary]  DEFAULT ((0)) FOR [GrossSalary]
GO

ALTER TABLE [dbo].[Employees] ADD  CONSTRAINT [DF_Employees_SRDueLimit]  DEFAULT ((0)) FOR [SRDueLimit]
GO

ALTER TABLE [dbo].[Employees]  WITH CHECK ADD  CONSTRAINT [FK_Employees_Designations] FOREIGN KEY([DesignationID])
REFERENCES [dbo].[Designations] ([DesignationID])
GO

ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [FK_Employees_Designations]
GO

ALTER TABLE [dbo].[Employees]  WITH CHECK ADD  CONSTRAINT [FK_Employees_SisterConcerns] FOREIGN KEY([ConcernID])
REFERENCES [dbo].[SisterConcerns] ([ConcernID])
GO

ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [FK_Employees_SisterConcerns]
GO


-------------------------------------------------For Customer----------------------------------------------------


CREATE TABLE [dbo].[Customers](
	[CustomerID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](50) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[FName] [nvarchar](max) NULL,
	[CompanyName] [nvarchar](max) NULL,
	[ContactNo] [varchar](150) NULL,
	[EmailID] [varchar](250) NULL,
	[NID] [varchar](150) NULL,
	[Address] [nvarchar](max) NULL,
	[PhotoPath] [varchar](150) NULL,
	[TotalDue] [decimal](18, 2) NOT NULL,
	[RefName] [nvarchar](max) NULL,
	[RefContact] [varchar](150) NULL,
	[RefFName] [nvarchar](max) NULL,
	[RefAddress] [nvarchar](max) NULL,
	[CustomerType] [int] NOT NULL,
	[EmployeeID] [int] NOT NULL,
	[CusDueLimit] [decimal](18, 2) NOT NULL,
	[ConcernID] [int] NOT NULL,
	[OpeningDue] [decimal](18, 2) NULL,
 CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Customers] ADD  CONSTRAINT [DF_Customers_TotalDue]  DEFAULT ((0)) FOR [TotalDue]
GO

ALTER TABLE [dbo].[Customers] ADD  CONSTRAINT [DF_Customers_CustomerType]  DEFAULT ((1)) FOR [CustomerType]
GO

ALTER TABLE [dbo].[Customers] ADD  CONSTRAINT [DF_Customers_CusDueLimit]  DEFAULT ((0)) FOR [CusDueLimit]
GO

ALTER TABLE [dbo].[Customers] ADD  CONSTRAINT [DF_Customers_OpeningDue]  DEFAULT ((0.0)) FOR [OpeningDue]
GO

ALTER TABLE [dbo].[Customers]  WITH CHECK ADD  CONSTRAINT [FK_Customers_Employees] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[Employees] ([EmployeeID])
GO

ALTER TABLE [dbo].[Customers] CHECK CONSTRAINT [FK_Customers_Employees]
GO

ALTER TABLE [dbo].[Customers]  WITH CHECK ADD  CONSTRAINT [FK_Customers_SisterConcerns] FOREIGN KEY([ConcernID])
REFERENCES [dbo].[SisterConcerns] ([ConcernID])
GO

ALTER TABLE [dbo].[Customers] CHECK CONSTRAINT [FK_Customers_SisterConcerns]
GO


----------------------------------------------------- For Suppliers-----------------------------------------------------



CREATE TABLE [dbo].[Suppliers](
	[SupplierID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](50) NULL,
	[Name] [nvarchar](max) NULL,
	[OwnerName] [nvarchar](max) NULL,
	[ContactNo] [varchar](150) NULL,
	[Address] [nvarchar](max) NULL,
	[PhotoPath] [varchar](250) NULL,
	[TotalDue] [decimal](18, 2) NOT NULL,
	[ConcernID] [int] NOT NULL,
	[OpeningDue] [decimal](18, 2) NULL,
 CONSTRAINT [PK_Suppliers] PRIMARY KEY CLUSTERED 
(
	[SupplierID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Suppliers] ADD  CONSTRAINT [DF_Suppliers_TotalDue]  DEFAULT ((0)) FOR [TotalDue]
GO

ALTER TABLE [dbo].[Suppliers] ADD  CONSTRAINT [DF_Suppliers_OpeningDue]  DEFAULT ((0.0)) FOR [OpeningDue]
GO

ALTER TABLE [dbo].[Suppliers]  WITH CHECK ADD  CONSTRAINT [FK_Suppliers_SisterConcerns] FOREIGN KEY([ConcernID])
REFERENCES [dbo].[SisterConcerns] ([ConcernID])
GO

ALTER TABLE [dbo].[Suppliers] CHECK CONSTRAINT [FK_Suppliers_SisterConcerns]
GO


----------------------------------------------------For Cash Collection------------------------------------------------------------------



CREATE TABLE [dbo].[CashCollections](
	[CashCollectionID] [int] IDENTITY(1,1) NOT NULL,
	[PaymentType] [int] NULL,
	[BankName] [varchar](250) NULL,
	[BranchName] [varchar](250) NULL,
	[EntryDate] [datetime] NULL,
	[Amount] [decimal](18, 0) NULL,
	[AccountNo] [varchar](350) NULL,
	[MBAccountNo] [varchar](350) NULL,
	[BKashNo] [varchar](350) NULL,
	[TransactionType] [int] NULL,
	[CustomerID] [int] NULL,
	[SupplierID] [int] NULL,
	[ConcernID] [int] NOT NULL,
	[ReceiptNo] [varchar](300) NULL,
	[CreatedBy] [int] NOT NULL,
	[CreateDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[AdjustAmt] [decimal](18, 2) NOT NULL,
	[BalanceDue] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_CashCollections] PRIMARY KEY CLUSTERED 
(
	[CashCollectionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CashCollections] ADD  CONSTRAINT [DF_CashCollections_CreatedBy]  DEFAULT ((1)) FOR [CreatedBy]
GO

ALTER TABLE [dbo].[CashCollections] ADD  CONSTRAINT [DF_CashCollections_AdjustAmt]  DEFAULT ((0)) FOR [AdjustAmt]
GO

ALTER TABLE [dbo].[CashCollections] ADD  CONSTRAINT [DF_CashCollections_BalanceDue]  DEFAULT ((0)) FOR [BalanceDue]
GO

ALTER TABLE [dbo].[CashCollections]  WITH CHECK ADD  CONSTRAINT [FK_CashCollections_SisterConcerns] FOREIGN KEY([ConcernID])
REFERENCES [dbo].[SisterConcerns] ([ConcernID])
GO

ALTER TABLE [dbo].[CashCollections] CHECK CONSTRAINT [FK_CashCollections_SisterConcerns]
GO


---------------------------------------------------------ExpenseItems--------------------------------------------------------------------------
CREATE TABLE [dbo].[ExpenseItems](
	[ExpenseItemID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](150) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_ExpenseItems] PRIMARY KEY CLUSTERED 
(
	[ExpenseItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-------------------------------------------------------Expenditure-----------------------------------------------------------------------------



CREATE TABLE [dbo].[Expenditures](
	[ExpenditureID] [int] IDENTITY(1,1) NOT NULL,
	[EntryDate] [datetime] NOT NULL,
	[Purpose] [nvarchar](max) NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[ExpenseItemID] [int] NOT NULL,
	[VoucherNo] [varchar](max) NOT NULL,
	[ConcernID] [int] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreateDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Expenditures] PRIMARY KEY CLUSTERED 
(
	[ExpenditureID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Expenditures] ADD  CONSTRAINT [DF_Expenditures_Amount]  DEFAULT ((0)) FOR [Amount]
GO

ALTER TABLE [dbo].[Expenditures] ADD  CONSTRAINT [DF_Expenditures_CreatedBy]  DEFAULT ((1)) FOR [CreatedBy]
GO

ALTER TABLE [dbo].[Expenditures]  WITH CHECK ADD  CONSTRAINT [FK_Expenditures_ExpenseItems] FOREIGN KEY([ExpenseItemID])
REFERENCES [dbo].[ExpenseItems] ([ExpenseItemID])
GO

ALTER TABLE [dbo].[Expenditures] CHECK CONSTRAINT [FK_Expenditures_ExpenseItems]
GO

ALTER TABLE [dbo].[Expenditures]  WITH CHECK ADD  CONSTRAINT [FK_Expenditures_SisterConcerns] FOREIGN KEY([ConcernID])
REFERENCES [dbo].[SisterConcerns] ([ConcernID])
GO

ALTER TABLE [dbo].[Expenditures] CHECK CONSTRAINT [FK_Expenditures_SisterConcerns]
GO


-----------------------------------------------------SalesOffer------------------------------------------------------------------------


CREATE TABLE [dbo].[SaleOffers](
	[OfferID] [int] IDENTITY(1,1) NOT NULL,
	[OfferCode] [varchar](250) NULL,
	[ProductID] [int] NOT NULL,
	[FromDate] [datetime] NULL,
	[ToDate] [datetime] NULL,
	[Description] [varchar](1000) NULL,
	[OfferValue] [decimal](18, 2) NOT NULL,
	[OfferType] [int] NOT NULL,
	[ThresholdValue] [decimal](18, 2) NOT NULL,
	[Status] [int] NULL,
	[ConcernID] [int] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_SaleOffers] PRIMARY KEY CLUSTERED 
(
	[OfferID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SaleOffers] ADD  CONSTRAINT [DF_SaleOffers_OfferValue]  DEFAULT ((0)) FOR [OfferValue]
GO

ALTER TABLE [dbo].[SaleOffers] ADD  CONSTRAINT [DF_SaleOffers_ThresholdValue]  DEFAULT ((0)) FOR [ThresholdValue]
GO

ALTER TABLE [dbo].[SaleOffers]  WITH CHECK ADD  CONSTRAINT [FK_SaleOffers_Products] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductID])
GO

ALTER TABLE [dbo].[SaleOffers] CHECK CONSTRAINT [FK_SaleOffers_Products]
GO

ALTER TABLE [dbo].[SaleOffers]  WITH CHECK ADD  CONSTRAINT [FK_SaleOffers_SisterConcerns] FOREIGN KEY([ConcernID])
REFERENCES [dbo].[SisterConcerns] ([ConcernID])
GO

ALTER TABLE [dbo].[SaleOffers] CHECK CONSTRAINT [FK_SaleOffers_SisterConcerns]
GO


------------------------------------------------------------Price Protection---------------------------------------------------------------------



CREATE TABLE [dbo].[PriceProtections](
	[PProtectionID] [int] IDENTITY(1,1) NOT NULL,
	[ProductID] [int] NOT NULL,
	[PChangeDate] [datetime] NULL,
	[PrvPrice] [decimal](18, 2) NOT NULL,
	[ChangePrice] [decimal](18, 2) NOT NULL,
	[POrderID] [int] NOT NULL,
	[ConcernID] [int] NOT NULL,
	[ColorID] [int] NOT NULL,
	[SupplierID] [int] NOT NULL,
	[PrvStockQty] [int] NOT NULL,
 CONSTRAINT [PK_PriceProtection] PRIMARY KEY CLUSTERED 
(
	[PProtectionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PriceProtections] ADD  CONSTRAINT [DF_PriceProtection_PrvPrice]  DEFAULT ((0)) FOR [PrvPrice]
GO

ALTER TABLE [dbo].[PriceProtections] ADD  CONSTRAINT [DF_PriceProtection_ChangePrice]  DEFAULT ((0)) FOR [ChangePrice]
GO

ALTER TABLE [dbo].[PriceProtections]  WITH CHECK ADD  CONSTRAINT [FK_PriceProtection_Colors] FOREIGN KEY([ColorID])
REFERENCES [dbo].[Colors] ([ColorID])
GO

ALTER TABLE [dbo].[PriceProtections] CHECK CONSTRAINT [FK_PriceProtection_Colors]
GO

ALTER TABLE [dbo].[PriceProtections]  WITH CHECK ADD  CONSTRAINT [FK_PriceProtection_POrders] FOREIGN KEY([POrderID])
REFERENCES [dbo].[POrders] ([POrderID])
GO

ALTER TABLE [dbo].[PriceProtections] CHECK CONSTRAINT [FK_PriceProtection_POrders]
GO

ALTER TABLE [dbo].[PriceProtections]  WITH CHECK ADD  CONSTRAINT [FK_PriceProtection_Products] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductID])
GO

ALTER TABLE [dbo].[PriceProtections] CHECK CONSTRAINT [FK_PriceProtection_Products]
GO

ALTER TABLE [dbo].[PriceProtections]  WITH CHECK ADD  CONSTRAINT [FK_PriceProtection_SisterConcerns] FOREIGN KEY([ConcernID])
REFERENCES [dbo].[SisterConcerns] ([ConcernID])
GO

ALTER TABLE [dbo].[PriceProtections] CHECK CONSTRAINT [FK_PriceProtection_SisterConcerns]
GO

ALTER TABLE [dbo].[PriceProtections]  WITH CHECK ADD  CONSTRAINT [FK_PriceProtections_Suppliers] FOREIGN KEY([SupplierID])
REFERENCES [dbo].[Suppliers] ([SupplierID])
GO

ALTER TABLE [dbo].[PriceProtections] CHECK CONSTRAINT [FK_PriceProtections_Suppliers]
GO


----------------------------------------------------------For POrders-------------------------------------------------------------------------


CREATE TABLE [dbo].[POrders](
	[POrderID] [int] IDENTITY(1,1) NOT NULL,
	[OrderDate] [datetime] NOT NULL,
	[ChallanNo] [varchar](150) NOT NULL,
	[SupplierID] [int] NOT NULL,
	[GrandTotal] [decimal](18, 2) NOT NULL,
	[TDiscount] [decimal](18, 2) NOT NULL,
	[TotalAmt] [decimal](18, 2) NOT NULL,
	[RecAmt] [decimal](18, 2) NOT NULL,
	[PaymentDue] [decimal](18, 2) NOT NULL,
	[TotalDue] [decimal](18, 2) NOT NULL,
	[Status] [int] NOT NULL,
	[PPDisAmt] [decimal](18, 2) NOT NULL,
	[NetDiscount] [decimal](18, 2) NOT NULL,
	[LaborCost] [decimal](18, 2) NOT NULL,
	[AdjAmount] [decimal](18, 2) NOT NULL,
	[ConcernID] [int] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreateDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_POrders] PRIMARY KEY CLUSTERED 
(
	[POrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[POrders] ADD  CONSTRAINT [DF_POrders_Stattus]  DEFAULT ((0)) FOR [Status]
GO

ALTER TABLE [dbo].[POrders] ADD  CONSTRAINT [DF_POrders_PPDisAmt]  DEFAULT ((0)) FOR [PPDisAmt]
GO

ALTER TABLE [dbo].[POrders] ADD  CONSTRAINT [DF_POrders_NetDiscount]  DEFAULT ((0)) FOR [NetDiscount]
GO

ALTER TABLE [dbo].[POrders] ADD  CONSTRAINT [DF_POrders_LaborCost]  DEFAULT ((0)) FOR [LaborCost]
GO

ALTER TABLE [dbo].[POrders] ADD  CONSTRAINT [DF_POrders_CreatedBy]  DEFAULT ((1)) FOR [CreatedBy]
GO

ALTER TABLE [dbo].[POrders]  WITH CHECK ADD  CONSTRAINT [FK_POrders_SisterConcerns] FOREIGN KEY([ConcernID])
REFERENCES [dbo].[SisterConcerns] ([ConcernID])
GO

ALTER TABLE [dbo].[POrders] CHECK CONSTRAINT [FK_POrders_SisterConcerns]
GO

ALTER TABLE [dbo].[POrders]  WITH CHECK ADD  CONSTRAINT [FK_POrders_Suppliers] FOREIGN KEY([SupplierID])
REFERENCES [dbo].[Suppliers] ([SupplierID])
GO

ALTER TABLE [dbo].[POrders] CHECK CONSTRAINT [FK_POrders_Suppliers]
GO


---------------------------------------------------------For POrderDetails------------------------------------------------------



CREATE TABLE [dbo].[POrderDetails](
	[POrderDetailID] [int] IDENTITY(1,1) NOT NULL,
	[ProductID] [int] NOT NULL,
	[Quantity] [decimal](18, 2) NOT NULL,
	[UnitPrice] [decimal](18, 2) NOT NULL,
	[TAmount] [decimal](18, 2) NOT NULL,
	[POrderID] [int] NOT NULL,
	[PPDISPer] [decimal](18, 2) NOT NULL,
	[PPDISAmt] [decimal](18, 2) NOT NULL,
	[MRPRate] [decimal](18, 2) NOT NULL,
	[ColorID] [int] NULL,
 CONSTRAINT [PK_POrderDetails] PRIMARY KEY CLUSTERED 
(
	[POrderDetailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[POrderDetails] ADD  CONSTRAINT [DF_POrderDetails_PPDISPer]  DEFAULT ((0)) FOR [PPDISPer]
GO

ALTER TABLE [dbo].[POrderDetails] ADD  CONSTRAINT [DF_POrderDetails_PPDISAmt]  DEFAULT ((0)) FOR [PPDISAmt]
GO

ALTER TABLE [dbo].[POrderDetails] ADD  CONSTRAINT [DF_POrderDetails_MRPRate]  DEFAULT ((0)) FOR [MRPRate]
GO

ALTER TABLE [dbo].[POrderDetails]  WITH CHECK ADD  CONSTRAINT [FK_POrderDetails_Colors] FOREIGN KEY([ColorID])
REFERENCES [dbo].[Colors] ([ColorID])
GO

ALTER TABLE [dbo].[POrderDetails] CHECK CONSTRAINT [FK_POrderDetails_Colors]
GO

ALTER TABLE [dbo].[POrderDetails]  WITH CHECK ADD  CONSTRAINT [FK_POrderDetails_POrders] FOREIGN KEY([POrderID])
REFERENCES [dbo].[POrders] ([POrderID])
GO

ALTER TABLE [dbo].[POrderDetails] CHECK CONSTRAINT [FK_POrderDetails_POrders]
GO

ALTER TABLE [dbo].[POrderDetails]  WITH CHECK ADD  CONSTRAINT [FK_POrderDetails_Products] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductID])
GO

ALTER TABLE [dbo].[POrderDetails] CHECK CONSTRAINT [FK_POrderDetails_Products]
GO


---------------------------------------------------------For POProductDetails-----------------------------------------------------------------


---------------------------------------------------------For Stocks----------------------------------------------------------------------------

---------------------------------------------------------For StockDetails---------------------------------------------------------------------

---------------------------------------------------------For SOrders---------------------------------------------------------------------------


---------------------------------------------------------For SOrderDetails---------------------------------------------------------------------

---------------------------------------------------------For SRVisits--------------------------------------------------------------------------

---------------------------------------------------------For SRVisitDetails--------------------------------------------------------------------

---------------------------------------------------------For SRVProductDetails----------------------------------------------------------------

---------------------------------------------------------For CreditSales----------------------------------------------------------------------

---------------------------------------------------------For CreditSaleDetails----------------------------------------------------------------

---------------------------------------------------------For CreditSalesSchedules------------------------------------------------------------


