namespace IMSWEB.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropRoleandOldUser : DbMigration
    {
        public override void Up()
        {            
            AddColumn("dbo.Users", "ConcernID", c => c.Int(nullable: false));
            CreateIndex("dbo.Users", "ConcernID");
            AddForeignKey("dbo.Users", "ConcernID", "dbo.SisterConcerns", "ConcernID");
        }

        public override void Down()
        {

        }
    }
}
