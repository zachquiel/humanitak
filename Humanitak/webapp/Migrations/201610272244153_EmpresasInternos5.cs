namespace SmartAdminMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmpresasInternos5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Groups", "SpecialPerception_Id", "dbo.SpecialPerceptions");
            DropIndex("dbo.Groups", new[] { "SpecialPerception_Id" });
            CreateTable(
                "dbo.SpecialPerceptionGroups",
                c => new
                    {
                        SpecialPerception_Id = c.Long(nullable: false),
                        Group_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.SpecialPerception_Id, t.Group_Id })
                .ForeignKey("dbo.SpecialPerceptions", t => t.SpecialPerception_Id, cascadeDelete: true)
                .ForeignKey("dbo.Groups", t => t.Group_Id, cascadeDelete: true)
                .Index(t => t.SpecialPerception_Id)
                .Index(t => t.Group_Id);
            
            DropColumn("dbo.Groups", "SpecialPerception_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Groups", "SpecialPerception_Id", c => c.Long());
            DropForeignKey("dbo.SpecialPerceptionGroups", "Group_Id", "dbo.Groups");
            DropForeignKey("dbo.SpecialPerceptionGroups", "SpecialPerception_Id", "dbo.SpecialPerceptions");
            DropIndex("dbo.SpecialPerceptionGroups", new[] { "Group_Id" });
            DropIndex("dbo.SpecialPerceptionGroups", new[] { "SpecialPerception_Id" });
            DropTable("dbo.SpecialPerceptionGroups");
            CreateIndex("dbo.Groups", "SpecialPerception_Id");
            AddForeignKey("dbo.Groups", "SpecialPerception_Id", "dbo.SpecialPerceptions", "Id");
        }
    }
}
