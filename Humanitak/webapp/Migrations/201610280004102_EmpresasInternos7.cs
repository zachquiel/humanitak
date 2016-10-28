namespace SmartAdminMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmpresasInternos7 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PerceptionDepartments", "Perception_Id", "dbo.Perceptions");
            DropForeignKey("dbo.PerceptionDepartments", "Department_Id", "dbo.Departments");
            DropIndex("dbo.PerceptionDepartments", new[] { "Perception_Id" });
            DropIndex("dbo.PerceptionDepartments", new[] { "Department_Id" });
            AddColumn("dbo.SpecialPerceptions", "Department_Id", c => c.Long());
            CreateIndex("dbo.SpecialPerceptions", "Department_Id");
            AddForeignKey("dbo.SpecialPerceptions", "Department_Id", "dbo.Departments", "Id");
            DropTable("dbo.PerceptionDepartments");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PerceptionDepartments",
                c => new
                    {
                        Perception_Id = c.Long(nullable: false),
                        Department_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.Perception_Id, t.Department_Id });
            
            DropForeignKey("dbo.SpecialPerceptions", "Department_Id", "dbo.Departments");
            DropIndex("dbo.SpecialPerceptions", new[] { "Department_Id" });
            DropColumn("dbo.SpecialPerceptions", "Department_Id");
            CreateIndex("dbo.PerceptionDepartments", "Department_Id");
            CreateIndex("dbo.PerceptionDepartments", "Perception_Id");
            AddForeignKey("dbo.PerceptionDepartments", "Department_Id", "dbo.Departments", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PerceptionDepartments", "Perception_Id", "dbo.Perceptions", "Id", cascadeDelete: true);
        }
    }
}
