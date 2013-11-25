namespace MailGames.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GameBoardGeneralization : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChessBoards", "LastReminded", c => c.DateTime());
            AddColumn("dbo.ChessBoards", "Check", c => c.Boolean(nullable: false));
            AddColumn("dbo.ChessBoards", "WinnerState", c => c.Int());
            AddColumn("dbo.TicTacToeBoards", "LastReminded", c => c.DateTime());
            AddColumn("dbo.TicTacToeBoards", "WinnerState", c => c.Int());

            Sql("update chessboards set winnerstate=winner");
            Sql("update chessboards set \"check\"=1 where runningstate=1");
            Sql("update tictactoeboards set winnerstate=winner - 1 where not winner is null");
            Sql("update tictactoeboards set winnerstate=2 where winnerstate=-1");

            DropColumn("dbo.ChessBoards", "Winner");
            DropColumn("dbo.ChessBoards", "RunningState");
            DropColumn("dbo.TicTacToeBoards", "Winner");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TicTacToeBoards", "Winner", c => c.Int(nullable: false));
            AddColumn("dbo.ChessBoards", "RunningState", c => c.Int(nullable: false));
            AddColumn("dbo.ChessBoards", "Winner", c => c.Int());
            DropColumn("dbo.TicTacToeBoards", "WinnerState");
            DropColumn("dbo.TicTacToeBoards", "LastReminded");
            DropColumn("dbo.ChessBoards", "WinnerState");
            DropColumn("dbo.ChessBoards", "Check");
            DropColumn("dbo.ChessBoards", "LastReminded");
        }
    }
}
