namespace SmartAdminMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Empleados : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Employees", name: "Enterprise_Id", newName: "PayingEnterprise_Id");
            RenameIndex(table: "dbo.Employees", name: "IX_Enterprise_Id", newName: "IX_PayingEnterprise_Id");
            AddColumn("dbo.Employees", "Email", c => c.String());
            AddColumn("dbo.Employees", "SsRegistrationDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Employees", "StartContractDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Employees", "EndContractDate", c => c.DateTime());
            AddColumn("dbo.Employees", "PermanentContractDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employees", "PermanentContractDate");
            DropColumn("dbo.Employees", "EndContractDate");
            DropColumn("dbo.Employees", "StartContractDate");
            DropColumn("dbo.Employees", "SsRegistrationDate");
            DropColumn("dbo.Employees", "Email");
            RenameIndex(table: "dbo.Employees", name: "IX_PayingEnterprise_Id", newName: "IX_Enterprise_Id");
            RenameColumn(table: "dbo.Employees", name: "PayingEnterprise_Id", newName: "Enterprise_Id");
        }
    }
}
