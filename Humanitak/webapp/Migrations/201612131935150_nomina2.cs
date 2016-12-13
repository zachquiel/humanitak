namespace SmartAdminMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nomina2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PayDays", "Enterprise_Id", c => c.Long());
            CreateIndex("dbo.PayDays", "Enterprise_Id");
            AddForeignKey("dbo.PayDays", "Enterprise_Id", "dbo.Enterprises", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PayDays", "Enterprise_Id", "dbo.Enterprises");
            DropIndex("dbo.PayDays", new[] { "Enterprise_Id" });
            DropColumn("dbo.PayDays", "Enterprise_Id");
        }
    }
}
