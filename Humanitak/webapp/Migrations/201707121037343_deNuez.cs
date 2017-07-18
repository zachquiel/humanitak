using System.Data.Entity.Migrations;

namespace SmartAdminMvc.Migrations {
    public partial class deNuez : DbMigration {
        public override void Up() {
            CreateTable(
                    "dbo.CatalogEntries",
                    c => new {
                        Id = c.Long(nullable: false, identity: true),
                        Key = c.String(),
                        Name = c.String(),
                        Type = c.String()
                    })
                .PrimaryKey(t => t.Id);
        }

        public override void Down() {
            DropTable("dbo.CatalogEntries");
        }
    }
}