namespace SmartAdminMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmpresasInternos4 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Enterprise_Id = c.Long(),
                        SpecialPerception_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Enterprises", t => t.Enterprise_Id)
                .ForeignKey("dbo.SpecialPerceptions", t => t.SpecialPerception_Id)
                .Index(t => t.Enterprise_Id)
                .Index(t => t.SpecialPerception_Id);
            
            AddColumn("dbo.Employees", "Group_Id", c => c.Long());
            CreateIndex("dbo.Employees", "Group_Id");
            AddForeignKey("dbo.Employees", "Group_Id", "dbo.Groups", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Groups", "SpecialPerception_Id", "dbo.SpecialPerceptions");
            DropForeignKey("dbo.Groups", "Enterprise_Id", "dbo.Enterprises");
            DropForeignKey("dbo.Employees", "Group_Id", "dbo.Groups");
            DropIndex("dbo.Groups", new[] { "SpecialPerception_Id" });
            DropIndex("dbo.Groups", new[] { "Enterprise_Id" });
            DropIndex("dbo.Employees", new[] { "Group_Id" });
            DropColumn("dbo.Employees", "Group_Id");
            DropTable("dbo.Groups");
        }
    }
}
