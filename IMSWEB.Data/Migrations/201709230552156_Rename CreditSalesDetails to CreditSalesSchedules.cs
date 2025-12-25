namespace IMSWEB.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameCreditSalesDetailstoCreditSalesSchedules : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CreditSalesDetails", "CreditSalesID", "dbo.CreditSales");
            DropIndex("dbo.CreditSalesDetails", new[] { "CreditSalesID" });
            CreateTable(
                "dbo.CreditSalesSchedules",
                c => new
                    {
                        CSScheduleID = c.Int(nullable: false, identity: true),
                        MonthDate = c.DateTime(nullable: false),
                        Balance = c.Decimal(nullable: false, storeType: "money"),
                        InstallmentAmt = c.Decimal(nullable: false, storeType: "money"),
                        PaymentDate = c.DateTime(nullable: false),
                        CreditSalesID = c.Int(nullable: false),
                        PaymentStatus = c.String(nullable: false, maxLength: 150),
                        InterestAmount = c.Decimal(storeType: "money"),
                        ClosingBalance = c.Decimal(nullable: false, storeType: "money"),
                        ScheduleNo = c.Int(nullable: false),
                        Remarks = c.String(),
                        IsUnExpected = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CSScheduleID)
                .ForeignKey("dbo.CreditSales", t => t.CreditSalesID, cascadeDelete: true)
                .Index(t => t.CreditSalesID);
            
            DropTable("dbo.CreditSalesDetails");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CreditSalesDetails",
                c => new
                    {
                        CSDetailsID = c.Int(nullable: false, identity: true),
                        MonthDate = c.DateTime(nullable: false),
                        Balance = c.Decimal(nullable: false, storeType: "money"),
                        InstallmentAmt = c.Decimal(nullable: false, storeType: "money"),
                        PaymentDate = c.DateTime(nullable: false),
                        CreditSalesID = c.Int(nullable: false),
                        PaymentStatus = c.String(nullable: false, maxLength: 150),
                        InterestAmount = c.Decimal(storeType: "money"),
                        ClosingBalance = c.Decimal(nullable: false, storeType: "money"),
                        ScheduleNo = c.Int(nullable: false),
                        Remarks = c.String(),
                        IsUnExpected = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CSDetailsID);
            
            DropForeignKey("dbo.CreditSalesSchedules", "CreditSalesID", "dbo.CreditSales");
            DropIndex("dbo.CreditSalesSchedules", new[] { "CreditSalesID" });
            DropTable("dbo.CreditSalesSchedules");
            CreateIndex("dbo.CreditSalesDetails", "CreditSalesID");
            AddForeignKey("dbo.CreditSalesDetails", "CreditSalesID", "dbo.CreditSales", "CreditSalesID", cascadeDelete: true);
        }
    }
}
