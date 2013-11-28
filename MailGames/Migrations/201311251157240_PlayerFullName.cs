namespace MailGames.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlayerFullName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "FullName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Players", "FullName");
        }
    }
}
