namespace MailGames.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TicTacToeWinner : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TicTacToeBoards", "Winner", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TicTacToeBoards", "Winner");
        }
    }
}
