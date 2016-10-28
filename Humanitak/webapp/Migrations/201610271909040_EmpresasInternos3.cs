namespace SmartAdminMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmpresasInternos3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SpecialPerceptions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Amount = c.Double(nullable: false),
                        Repeat = c.Int(nullable: false),
                        Perception_Id = c.Long(),
                        Enterprise_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Perceptions", t => t.Perception_Id)
                .ForeignKey("dbo.Enterprises", t => t.Enterprise_Id)
                .Index(t => t.Perception_Id)
                .Index(t => t.Enterprise_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SpecialPerceptions", "Enterprise_Id", "dbo.Enterprises");
            DropForeignKey("dbo.SpecialPerceptions", "Perception_Id", "dbo.Perceptions");
            DropIndex("dbo.SpecialPerceptions", new[] { "Enterprise_Id" });
            DropIndex("dbo.SpecialPerceptions", new[] { "Perception_Id" });
            DropTable("dbo.SpecialPerceptions");
        }
    }
}
