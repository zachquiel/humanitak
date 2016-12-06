namespace SmartAdminMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datosEmpleado : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "WorkState", c => c.String());
            AddColumn("dbo.Employees", "PatronalRegistryNo", c => c.String());
            AddColumn("dbo.Employees", "Regime", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employees", "Regime");
            DropColumn("dbo.Employees", "PatronalRegistryNo");
            DropColumn("dbo.Employees", "WorkState");
        }
    }
}
