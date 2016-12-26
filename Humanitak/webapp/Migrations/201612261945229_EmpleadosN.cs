namespace SmartAdminMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmpleadosN : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.Employees", "PayingEnterprise_Id", "dbo.Enterprises");
            AddColumn("dbo.Employees", "Enterprise_Id", c => c.Long());
            AddColumn("dbo.Employees", "SecondaryEnterprise_Id", c => c.Long());
            CreateIndex("dbo.Employees", "Enterprise_Id");
            CreateIndex("dbo.Employees", "SecondaryEnterprise_Id");
            AddForeignKey("dbo.Employees", "SecondaryEnterprise_Id", "dbo.Enterprises", "Id");
            //AddForeignKey("dbo.Employees", "Enterprise_Id", "dbo.Enterprises", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Employees", "Enterprise_Id", "dbo.Enterprises");
            DropForeignKey("dbo.Employees", "SecondaryEnterprise_Id", "dbo.Enterprises");
            DropIndex("dbo.Employees", new[] { "SecondaryEnterprise_Id" });
            DropIndex("dbo.Employees", new[] { "Enterprise_Id" });
            DropColumn("dbo.Employees", "SecondaryEnterprise_Id");
            DropColumn("dbo.Employees", "Enterprise_Id");
            AddForeignKey("dbo.Employees", "PayingEnterprise_Id", "dbo.Enterprises", "Id");
        }
    }
}
