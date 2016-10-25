namespace SmartAdminMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "LastAccess", c => c.DateTime(nullable: false));
            AddColumn("dbo.Users", "UserType", c => c.String());
            AddColumn("dbo.Users", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "IsActive");
            DropColumn("dbo.Users", "UserType");
            DropColumn("dbo.Users", "LastAccess");
        }
    }
}
