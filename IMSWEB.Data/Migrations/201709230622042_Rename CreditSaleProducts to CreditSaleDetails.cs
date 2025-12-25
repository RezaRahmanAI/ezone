namespace IMSWEB.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameCreditSaleProductstoCreditSaleDetails : DbMigration
    {
        public override void Up()
        {
            //RenameTable(name: "dbo.CreditSaleProducts", newName: "CreditSaleDetails");
            //DropPrimaryKey("dbo.CreditSaleDetails");
            //AddColumn("dbo.CreditSaleDetails", "CreditSaleDetailsID", c => c.Int(nullable: false, identity: true));
            //AddPrimaryKey("dbo.CreditSaleDetails", "CreditSaleDetailsID");
            //DropColumn("dbo.CreditSaleDetails", "CreditSaleProductsID");
        }
        
        public override void Down()
        {
            //AddColumn("dbo.CreditSaleDetails", "CreditSaleProductsID", c => c.Int(nullable: false, identity: true));
            //DropPrimaryKey("dbo.CreditSaleDetails");
            //DropColumn("dbo.CreditSaleDetails", "CreditSaleDetailsID");
            //AddPrimaryKey("dbo.CreditSaleDetails", "CreditSaleProductsID");
            //RenameTable(name: "dbo.CreditSaleDetails", newName: "CreditSaleProducts");
        }
    }
}
