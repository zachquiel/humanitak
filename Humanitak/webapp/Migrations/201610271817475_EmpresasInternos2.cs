namespace SmartAdminMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmpresasInternos2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Perceptions", "Perception_Id", "dbo.Perceptions");
            DropForeignKey("dbo.Perceptions", "Department_Id", "dbo.Departments");
            DropIndex("dbo.Perceptions", new[] { "Perception_Id" });
            DropIndex("dbo.Perceptions", new[] { "Department_Id" });
            CreateTable(
                "dbo.PerceptionDepartments",
                c => new
                    {
                        Perception_Id = c.Long(nullable: false),
                        Department_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.Perception_Id, t.Department_Id })
                .ForeignKey("dbo.Perceptions", t => t.Perception_Id, cascadeDelete: true)
                .ForeignKey("dbo.Departments", t => t.Department_Id, cascadeDelete: true)
                .Index(t => t.Perception_Id)
                .Index(t => t.Department_Id);
            
            DropColumn("dbo.Perceptions", "Perception_Id");
            DropColumn("dbo.Perceptions", "Department_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Perceptions", "Department_Id", c => c.Long());
            AddColumn("dbo.Perceptions", "Perception_Id", c => c.Long());
            DropForeignKey("dbo.PerceptionDepartments", "Department_Id", "dbo.Departments");
            DropForeignKey("dbo.PerceptionDepartments", "Perception_Id", "dbo.Perceptions");
            DropIndex("dbo.PerceptionDepartments", new[] { "Department_Id" });
            DropIndex("dbo.PerceptionDepartments", new[] { "Perception_Id" });
            DropTable("dbo.PerceptionDepartments");
            CreateIndex("dbo.Perceptions", "Department_Id");
            CreateIndex("dbo.Perceptions", "Perception_Id");
            AddForeignKey("dbo.Perceptions", "Department_Id", "dbo.Departments", "Id");
            AddForeignKey("dbo.Perceptions", "Perception_Id", "dbo.Perceptions", "Id");
        }
    }
}
