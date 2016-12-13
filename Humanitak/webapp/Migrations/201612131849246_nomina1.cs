namespace SmartAdminMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nomina1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PayDays", "PayStartDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PayDays", "PayStartDate");
        }
    }
}
