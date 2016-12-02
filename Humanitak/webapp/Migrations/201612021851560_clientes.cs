namespace SmartAdminMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class clientes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Payday1Start = c.Int(nullable: false),
                        Payday1End = c.Int(nullable: false),
                        Payday2Start = c.Int(nullable: false),
                        Payday2End = c.Int(nullable: false),
                        UsesPunchClock = c.Boolean(nullable: false),
                        Commission = c.Int(nullable: false),
                        Vat = c.Double(nullable: false),
                        State = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        Operations = c.String(),
                        City = c.String(),
                        LastPayday = c.DateTime(nullable: false),
                        Header_Id = c.Long(),
                        Logo_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EnterpriseImages", t => t.Header_Id)
                .ForeignKey("dbo.EnterpriseImages", t => t.Logo_Id)
                .Index(t => t.Header_Id)
                .Index(t => t.Logo_Id);
            
            CreateTable(
                "dbo.EnterpriseClients",
                c => new
                    {
                        Enterprise_Id = c.Long(nullable: false),
                        Client_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.Enterprise_Id, t.Client_Id })
                .ForeignKey("dbo.Enterprises", t => t.Enterprise_Id, cascadeDelete: true)
                .ForeignKey("dbo.Clients", t => t.Client_Id, cascadeDelete: true)
                .Index(t => t.Enterprise_Id)
                .Index(t => t.Client_Id);
            
            AddColumn("dbo.Departments", "Client_Id", c => c.Long());
            AddColumn("dbo.Employees", "Client_Id", c => c.Long());
            AddColumn("dbo.Groups", "Client_Id", c => c.Long());
            AddColumn("dbo.SpecialPerceptions", "Client_Id", c => c.Long());
            AddColumn("dbo.Perceptions", "Client_Id", c => c.Long());
            AddColumn("dbo.Positions", "Client_Id", c => c.Long());
            CreateIndex("dbo.Departments", "Client_Id");
            CreateIndex("dbo.Employees", "Client_Id");
            CreateIndex("dbo.Groups", "Client_Id");
            CreateIndex("dbo.SpecialPerceptions", "Client_Id");
            CreateIndex("dbo.Perceptions", "Client_Id");
            CreateIndex("dbo.Positions", "Client_Id");
            AddForeignKey("dbo.Departments", "Client_Id", "dbo.Clients", "Id");
            AddForeignKey("dbo.Employees", "Client_Id", "dbo.Clients", "Id");
            AddForeignKey("dbo.Groups", "Client_Id", "dbo.Clients", "Id");
            AddForeignKey("dbo.Perceptions", "Client_Id", "dbo.Clients", "Id");
            AddForeignKey("dbo.Positions", "Client_Id", "dbo.Clients", "Id");
            AddForeignKey("dbo.SpecialPerceptions", "Client_Id", "dbo.Clients", "Id");
            DropColumn("dbo.Enterprises", "Commission");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Enterprises", "Commission", c => c.Int(nullable: false));
            DropForeignKey("dbo.SpecialPerceptions", "Client_Id", "dbo.Clients");
            DropForeignKey("dbo.Positions", "Client_Id", "dbo.Clients");
            DropForeignKey("dbo.Perceptions", "Client_Id", "dbo.Clients");
            DropForeignKey("dbo.Clients", "Logo_Id", "dbo.EnterpriseImages");
            DropForeignKey("dbo.Clients", "Header_Id", "dbo.EnterpriseImages");
            DropForeignKey("dbo.Groups", "Client_Id", "dbo.Clients");
            DropForeignKey("dbo.Employees", "Client_Id", "dbo.Clients");
            DropForeignKey("dbo.Departments", "Client_Id", "dbo.Clients");
            DropForeignKey("dbo.EnterpriseClients", "Client_Id", "dbo.Clients");
            DropForeignKey("dbo.EnterpriseClients", "Enterprise_Id", "dbo.Enterprises");
            DropIndex("dbo.EnterpriseClients", new[] { "Client_Id" });
            DropIndex("dbo.EnterpriseClients", new[] { "Enterprise_Id" });
            DropIndex("dbo.Positions", new[] { "Client_Id" });
            DropIndex("dbo.Perceptions", new[] { "Client_Id" });
            DropIndex("dbo.SpecialPerceptions", new[] { "Client_Id" });
            DropIndex("dbo.Groups", new[] { "Client_Id" });
            DropIndex("dbo.Employees", new[] { "Client_Id" });
            DropIndex("dbo.Departments", new[] { "Client_Id" });
            DropIndex("dbo.Clients", new[] { "Logo_Id" });
            DropIndex("dbo.Clients", new[] { "Header_Id" });
            DropColumn("dbo.Positions", "Client_Id");
            DropColumn("dbo.Perceptions", "Client_Id");
            DropColumn("dbo.SpecialPerceptions", "Client_Id");
            DropColumn("dbo.Groups", "Client_Id");
            DropColumn("dbo.Employees", "Client_Id");
            DropColumn("dbo.Departments", "Client_Id");
            DropTable("dbo.EnterpriseClients");
            DropTable("dbo.Clients");
        }
    }
}
