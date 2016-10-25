namespace SmartAdminMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Empresas1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Enterprises", "State", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Enterprises", "State", c => c.Int(nullable: false));
        }
    }
}
