namespace SmartAdminMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmpresasInternos1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Perceptions", "Perception_Id", c => c.Long());
            AddColumn("dbo.Perceptions", "Department_Id", c => c.Long());
            CreateIndex("dbo.Perceptions", "Perception_Id");
            CreateIndex("dbo.Perceptions", "Department_Id");
            AddForeignKey("dbo.Perceptions", "Perception_Id", "dbo.Perceptions", "Id");
            AddForeignKey("dbo.Perceptions", "Department_Id", "dbo.Departments", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Perceptions", "Department_Id", "dbo.Departments");
            DropForeignKey("dbo.Perceptions", "Perception_Id", "dbo.Perceptions");
            DropIndex("dbo.Perceptions", new[] { "Department_Id" });
            DropIndex("dbo.Perceptions", new[] { "Perception_Id" });
            DropColumn("dbo.Perceptions", "Department_Id");
            DropColumn("dbo.Perceptions", "Perception_Id");
        }
    }
}
