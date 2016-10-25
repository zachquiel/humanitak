namespace SmartAdminMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Empresas4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Enterprises", "Operations", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Enterprises", "Operations");
        }
    }
}
