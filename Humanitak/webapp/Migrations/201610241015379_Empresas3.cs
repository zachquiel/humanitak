namespace SmartAdminMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Empresas3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.EnterpriseImages", "Enterprise_Id", "dbo.Enterprises");
            DropIndex("dbo.EnterpriseImages", new[] { "Enterprise_Id" });
            DropColumn("dbo.EnterpriseImages", "Enterprise_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EnterpriseImages", "Enterprise_Id", c => c.Long());
            CreateIndex("dbo.EnterpriseImages", "Enterprise_Id");
            AddForeignKey("dbo.EnterpriseImages", "Enterprise_Id", "dbo.Enterprises", "Id");
        }
    }
}
