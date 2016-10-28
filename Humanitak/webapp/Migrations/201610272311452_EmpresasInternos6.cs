namespace SmartAdminMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmpresasInternos6 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SpecialPerceptionGroups", "SpecialPerception_Id", "dbo.SpecialPerceptions");
            DropForeignKey("dbo.SpecialPerceptionGroups", "Group_Id", "dbo.Groups");
            DropIndex("dbo.SpecialPerceptionGroups", new[] { "SpecialPerception_Id" });
            DropIndex("dbo.SpecialPerceptionGroups", new[] { "Group_Id" });
            AddColumn("dbo.SpecialPerceptions", "Group_Id", c => c.Long());
            CreateIndex("dbo.SpecialPerceptions", "Group_Id");
            AddForeignKey("dbo.SpecialPerceptions", "Group_Id", "dbo.Groups", "Id");
            DropTable("dbo.SpecialPerceptionGroups");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SpecialPerceptionGroups",
                c => new
                    {
                        SpecialPerception_Id = c.Long(nullable: false),
                        Group_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.SpecialPerception_Id, t.Group_Id });
            
            DropForeignKey("dbo.SpecialPerceptions", "Group_Id", "dbo.Groups");
            DropIndex("dbo.SpecialPerceptions", new[] { "Group_Id" });
            DropColumn("dbo.SpecialPerceptions", "Group_Id");
            CreateIndex("dbo.SpecialPerceptionGroups", "Group_Id");
            CreateIndex("dbo.SpecialPerceptionGroups", "SpecialPerception_Id");
            AddForeignKey("dbo.SpecialPerceptionGroups", "Group_Id", "dbo.Groups", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SpecialPerceptionGroups", "SpecialPerception_Id", "dbo.SpecialPerceptions", "Id", cascadeDelete: true);
        }
    }
}
