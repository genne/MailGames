namespace MailGames.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JapaneseWhistTrumfSelector : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.JapaneseWhistMoves", "Trumf", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.JapaneseWhistMoves", "Trumf");
        }
    }
}
