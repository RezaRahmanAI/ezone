namespace IMSWEB.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAuditTrailProperties : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.CashCollections", "CreatedBy", c => c.Int(nullable: false));
            //AddColumn("dbo.CashCollections", "CreateDate", c => c.DateTime(nullable: false));
            //AddColumn("dbo.CashCollections", "ModifiedBy", c => c.Int());
            //AddColumn("dbo.CashCollections", "ModifiedDate", c => c.DateTime());
            //AddColumn("dbo.CreditSales", "CreatedBy", c => c.Int(nullable: false));
            //AddColumn("dbo.CreditSales", "CreateDate", c => c.DateTime(nullable: false));
            //AddColumn("dbo.CreditSales", "ModifiedBy", c => c.Int());
            //AddColumn("dbo.CreditSales", "ModifiedDate", c => c.DateTime());
            //AddColumn("dbo.DamageProducts", "CreatedBy", c => c.Int(nullable: false));
            //AddColumn("dbo.DamageProducts", "CreateDate", c => c.DateTime(nullable: false));
            //AddColumn("dbo.DamageProducts", "ModifiedBy", c => c.Int());
            //AddColumn("dbo.DamageProducts", "ModifiedDate", c => c.DateTime());
            //AddColumn("dbo.Expenditures", "CreatedBy", c => c.Int(nullable: false));
            //AddColumn("dbo.Expenditures", "CreateDate", c => c.DateTime(nullable: false));
            //AddColumn("dbo.Expenditures", "ModifiedBy", c => c.Int());
            //AddColumn("dbo.Expenditures", "ModifiedDate", c => c.DateTime());
            //AddColumn("dbo.POrders", "CreatedBy", c => c.Int(nullable: false));
            //AddColumn("dbo.POrders", "CreateDate", c => c.DateTime(nullable: false));
            //AddColumn("dbo.POrders", "ModifiedBy", c => c.Int());
            //AddColumn("dbo.POrders", "ModifiedDate", c => c.DateTime());
            //AddColumn("dbo.SOrders", "CreatedBy", c => c.Int(nullable: false));
            //AddColumn("dbo.SOrders", "CreateDate", c => c.DateTime(nullable: false));
            //AddColumn("dbo.SOrders", "ModifiedBy", c => c.Int());
            //AddColumn("dbo.SOrders", "ModifiedDate", c => c.DateTime());
            //AddColumn("dbo.Stocks", "CreatedBy", c => c.Int(nullable: false));
            //AddColumn("dbo.Stocks", "CreateDate", c => c.DateTime(nullable: false));
            //AddColumn("dbo.Stocks", "ModifiedBy", c => c.Int());
            //AddColumn("dbo.Stocks", "ModifiedDate", c => c.DateTime());
            //AddColumn("dbo.Errors", "CreatedBy", c => c.Int(nullable: false));
            //AddColumn("dbo.Errors", "CreateDate", c => c.DateTime(nullable: false));
            //AddColumn("dbo.Errors", "ModifiedBy", c => c.Int(nullable: false));
            //AddColumn("dbo.Errors", "ModifiedDate", c => c.DateTime());
            //DropColumn("dbo.Errors", "CreationDate");
            //DropColumn("dbo.Errors", "ModificationDate");
        }
        
        public override void Down()
        {
            //AddColumn("dbo.Errors", "ModificationDate", c => c.DateTime());
            //AddColumn("dbo.Errors", "CreationDate", c => c.DateTime(nullable: false));
            //DropColumn("dbo.Errors", "ModifiedDate");
            //DropColumn("dbo.Errors", "ModifiedBy");
            //DropColumn("dbo.Errors", "CreateDate");
            //DropColumn("dbo.Errors", "CreatedBy");
            //DropColumn("dbo.Stocks", "ModifiedDate");
            //DropColumn("dbo.Stocks", "ModifiedBy");
            //DropColumn("dbo.Stocks", "CreateDate");
            //DropColumn("dbo.Stocks", "CreatedBy");
            //DropColumn("dbo.SOrders", "ModifiedDate");
            //DropColumn("dbo.SOrders", "ModifiedBy");
            //DropColumn("dbo.SOrders", "CreateDate");
            //DropColumn("dbo.SOrders", "CreatedBy");
            //DropColumn("dbo.POrders", "ModifiedDate");
            //DropColumn("dbo.POrders", "ModifiedBy");
            //DropColumn("dbo.POrders", "CreateDate");
            //DropColumn("dbo.POrders", "CreatedBy");
            //DropColumn("dbo.Expenditures", "ModifiedDate");
            //DropColumn("dbo.Expenditures", "ModifiedBy");
            //DropColumn("dbo.Expenditures", "CreateDate");
            //DropColumn("dbo.Expenditures", "CreatedBy");
            //DropColumn("dbo.DamageProducts", "ModifiedDate");
            //DropColumn("dbo.DamageProducts", "ModifiedBy");
            //DropColumn("dbo.DamageProducts", "CreateDate");
            //DropColumn("dbo.DamageProducts", "CreatedBy");
            //DropColumn("dbo.CreditSales", "ModifiedDate");
            //DropColumn("dbo.CreditSales", "ModifiedBy");
            //DropColumn("dbo.CreditSales", "CreateDate");
            //DropColumn("dbo.CreditSales", "CreatedBy");
            //DropColumn("dbo.CashCollections", "ModifiedDate");
            //DropColumn("dbo.CashCollections", "ModifiedBy");
            //DropColumn("dbo.CashCollections", "CreateDate");
            //DropColumn("dbo.CashCollections", "CreatedBy");
        }
    }
}
