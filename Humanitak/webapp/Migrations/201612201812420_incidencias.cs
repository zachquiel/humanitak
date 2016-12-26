namespace SmartAdminMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class incidencias : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Incidences",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Type = c.String(),
                        ExtraHours = c.Int(nullable: false),
                        Employee_Id = c.Long(),
                        Enterprise_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.Employee_Id)
                .ForeignKey("dbo.Enterprises", t => t.Enterprise_Id)
                .Index(t => t.Employee_Id)
                .Index(t => t.Enterprise_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Incidences", "Enterprise_Id", "dbo.Enterprises");
            DropForeignKey("dbo.Incidences", "Employee_Id", "dbo.Employees");
            DropIndex("dbo.Incidences", new[] { "Enterprise_Id" });
            DropIndex("dbo.Incidences", new[] { "Employee_Id" });
            DropTable("dbo.Incidences");
        }
    }
}
