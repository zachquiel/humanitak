namespace SmartAdminMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nomina : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmployeePayDays",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        NaturalDays = c.Int(nullable: false),
                        DailySalary = c.Double(nullable: false),
                        Vacations = c.Double(nullable: false),
                        BreakDays = c.Double(nullable: false),
                        Holidays = c.Double(nullable: false),
                        DoublePay = c.Double(nullable: false),
                        TriplePay = c.Double(nullable: false),
                        Overtime = c.Double(nullable: false),
                        SundayPrime = c.Double(nullable: false),
                        VacationPrime = c.Double(nullable: false),
                        Perceptions = c.Double(nullable: false),
                        Deductions = c.Double(nullable: false),
                        Employee_Id = c.Long(),
                        PayDay_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.Employee_Id)
                .ForeignKey("dbo.PayDays", t => t.PayDay_Id)
                .Index(t => t.Employee_Id)
                .Index(t => t.PayDay_Id);
            
            CreateTable(
                "dbo.PayDays",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PayDate = c.DateTime(nullable: false),
                        AuthotizationDate = c.DateTime(nullable: false),
                        Authorizer_Id = c.String(maxLength: 128),
                        Creator_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Authorizer_Id)
                .ForeignKey("dbo.Users", t => t.Creator_Id)
                .Index(t => t.Authorizer_Id)
                .Index(t => t.Creator_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmployeePayDays", "PayDay_Id", "dbo.PayDays");
            DropForeignKey("dbo.PayDays", "Creator_Id", "dbo.Users");
            DropForeignKey("dbo.PayDays", "Authorizer_Id", "dbo.Users");
            DropForeignKey("dbo.EmployeePayDays", "Employee_Id", "dbo.Employees");
            DropIndex("dbo.PayDays", new[] { "Creator_Id" });
            DropIndex("dbo.PayDays", new[] { "Authorizer_Id" });
            DropIndex("dbo.EmployeePayDays", new[] { "PayDay_Id" });
            DropIndex("dbo.EmployeePayDays", new[] { "Employee_Id" });
            DropTable("dbo.PayDays");
            DropTable("dbo.EmployeePayDays");
        }
    }
}
