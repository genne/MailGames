namespace MailGames.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChessWinnerr : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChessBoards", "Winner", c => c.Int());
            AddColumn("dbo.ChessBoards", "RunningState", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChessBoards", "RunningState");
            DropColumn("dbo.ChessBoards", "Winner");
        }
    }
}
