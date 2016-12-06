namespace SmartAdminMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datosFiscales : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FiscalInformations",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        StreetAddress = c.String(),
                        OuterNumeral = c.String(),
                        InnerNumeral = c.String(),
                        Area = c.String(),
                        ZipCode = c.String(),
                        Town = c.String(),
                        Rfc = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Clients", "FiscalInformation_Id", c => c.Long());
            AlterColumn("dbo.Clients", "Commission", c => c.Double(nullable: false));
            CreateIndex("dbo.Clients", "FiscalInformation_Id");
            AddForeignKey("dbo.Clients", "FiscalInformation_Id", "dbo.FiscalInformations", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Clients", "FiscalInformation_Id", "dbo.FiscalInformations");
            DropIndex("dbo.Clients", new[] { "FiscalInformation_Id" });
            AlterColumn("dbo.Clients", "Commission", c => c.Int(nullable: false));
            DropColumn("dbo.Clients", "FiscalInformation_Id");
            DropTable("dbo.FiscalInformations");
        }
    }
}
