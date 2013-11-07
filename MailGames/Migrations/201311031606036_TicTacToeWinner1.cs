namespace MailGames.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TicTacToeWinner1 : DbMigration
    {
        public override void Up()
        {
            Sql("update tictactoeboards set winner = 0 where winner is null");
            AlterColumn("dbo.TicTacToeBoards", "Winner", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TicTacToeBoards", "Winner", c => c.Int());
        }
    }
}
