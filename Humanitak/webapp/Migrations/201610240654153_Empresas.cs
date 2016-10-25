namespace SmartAdminMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Empresas : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EnterpriseImages",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Image = c.Binary(),
                        Enterprise_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Enterprises", t => t.Enterprise_Id)
                .Index(t => t.Enterprise_Id);
            
            CreateTable(
                "dbo.Enterprises",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Payday1Start = c.Int(nullable: false),
                        Payday1End = c.Int(nullable: false),
                        Payday2Start = c.Int(nullable: false),
                        Payday2End = c.Int(nullable: false),
                        UsesPunchClock = c.Boolean(nullable: false),
                        Commision = c.Int(nullable: false),
                        Vat = c.Double(nullable: false),
                        State = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        City = c.String(),
                        LastPayday = c.DateTime(nullable: false),
                        Header_Id = c.Long(),
                        Logo_Id = c.Long(),
                        ParentEnterprise_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EnterpriseImages", t => t.Header_Id)
                .ForeignKey("dbo.EnterpriseImages", t => t.Logo_Id)
                .ForeignKey("dbo.Enterprises", t => t.ParentEnterprise_Id)
                .Index(t => t.Header_Id)
                .Index(t => t.Logo_Id)
                .Index(t => t.ParentEnterprise_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EnterpriseImages", "Enterprise_Id", "dbo.Enterprises");
            DropForeignKey("dbo.Enterprises", "ParentEnterprise_Id", "dbo.Enterprises");
            DropForeignKey("dbo.Enterprises", "Logo_Id", "dbo.EnterpriseImages");
            DropForeignKey("dbo.Enterprises", "Header_Id", "dbo.EnterpriseImages");
            DropIndex("dbo.Enterprises", new[] { "ParentEnterprise_Id" });
            DropIndex("dbo.Enterprises", new[] { "Logo_Id" });
            DropIndex("dbo.Enterprises", new[] { "Header_Id" });
            DropIndex("dbo.EnterpriseImages", new[] { "Enterprise_Id" });
            DropTable("dbo.Enterprises");
            DropTable("dbo.EnterpriseImages");
        }
    }
}
