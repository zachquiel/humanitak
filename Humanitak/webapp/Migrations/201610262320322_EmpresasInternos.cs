namespace SmartAdminMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmpresasInternos : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Criteria = c.String(),
                        Overtime = c.Boolean(nullable: false),
                        OvertimeThreshold = c.Int(nullable: false),
                        DoubleTimeHours = c.Int(nullable: false),
                        Enterprise_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Enterprises", t => t.Enterprise_Id)
                .Index(t => t.Enterprise_Id);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        LastName = c.String(),
                        Ssn = c.String(),
                        Curp = c.String(),
                        Rfc = c.String(),
                        Gender = c.String(),
                        DailySalary = c.Double(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        Bank = c.String(),
                        AccountNumber = c.String(),
                        OffDays = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        Address = c.String(),
                        Area = c.String(),
                        ZipCode = c.String(),
                        City = c.String(),
                        State = c.String(),
                        Phone = c.String(),
                        HasSocialSecurity = c.Boolean(nullable: false),
                        DoB = c.DateTime(nullable: false),
                        PlaceOfBirth = c.String(),
                        IdNumber = c.String(),
                        MaritalStatus = c.String(),
                        Department_Id = c.Long(),
                        Position_Id = c.Long(),
                        Enterprise_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departments", t => t.Department_Id)
                .ForeignKey("dbo.Positions", t => t.Position_Id)
                .ForeignKey("dbo.Enterprises", t => t.Enterprise_Id)
                .Index(t => t.Department_Id)
                .Index(t => t.Position_Id)
                .Index(t => t.Enterprise_Id);
            
            CreateTable(
                "dbo.Positions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Enterprise_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Enterprises", t => t.Enterprise_Id)
                .Index(t => t.Enterprise_Id);
            
            CreateTable(
                "dbo.Perceptions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        KeyName = c.String(),
                        Description = c.String(),
                        Formula = c.String(),
                        Enterprise_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Enterprises", t => t.Enterprise_Id)
                .Index(t => t.Enterprise_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Positions", "Enterprise_Id", "dbo.Enterprises");
            DropForeignKey("dbo.Perceptions", "Enterprise_Id", "dbo.Enterprises");
            DropForeignKey("dbo.Employees", "Enterprise_Id", "dbo.Enterprises");
            DropForeignKey("dbo.Departments", "Enterprise_Id", "dbo.Enterprises");
            DropForeignKey("dbo.Employees", "Position_Id", "dbo.Positions");
            DropForeignKey("dbo.Employees", "Department_Id", "dbo.Departments");
            DropIndex("dbo.Perceptions", new[] { "Enterprise_Id" });
            DropIndex("dbo.Positions", new[] { "Enterprise_Id" });
            DropIndex("dbo.Employees", new[] { "Enterprise_Id" });
            DropIndex("dbo.Employees", new[] { "Position_Id" });
            DropIndex("dbo.Employees", new[] { "Department_Id" });
            DropIndex("dbo.Departments", new[] { "Enterprise_Id" });
            DropTable("dbo.Perceptions");
            DropTable("dbo.Positions");
            DropTable("dbo.Employees");
            DropTable("dbo.Departments");
        }
    }
}
