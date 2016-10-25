namespace SmartAdminMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Empresas2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Enterprises", "Commission", c => c.Int(nullable: false));
            DropColumn("dbo.Enterprises", "Commision");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Enterprises", "Commision", c => c.Int(nullable: false));
            DropColumn("dbo.Enterprises", "Commission");
        }
    }
}
