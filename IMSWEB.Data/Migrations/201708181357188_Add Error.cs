namespace IMSWEB.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddError : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Errors",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Message = c.String(nullable: false, maxLength: 1500, unicode: false),
                        StackTrace = c.String(nullable: false, maxLength: 1500, unicode: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Errors");
        }
    }
}
