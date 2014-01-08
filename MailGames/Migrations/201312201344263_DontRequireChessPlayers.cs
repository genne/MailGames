namespace MailGames.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DontRequireChessPlayers : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ChessBoards", "FirstPlayer_Id", "dbo.Players");
            DropForeignKey("dbo.ChessBoards", "SecondPlayer_Id", "dbo.Players");
            DropIndex("dbo.ChessBoards", new[] { "FirstPlayer_Id" });
            DropIndex("dbo.ChessBoards", new[] { "SecondPlayer_Id" });
            AlterColumn("dbo.ChessBoards", "FirstPlayer_Id", c => c.Int());
            AlterColumn("dbo.ChessBoards", "SecondPlayer_Id", c => c.Int());
            CreateIndex("dbo.ChessBoards", "FirstPlayer_Id");
            CreateIndex("dbo.ChessBoards", "SecondPlayer_Id");
            AddForeignKey("dbo.ChessBoards", "FirstPlayer_Id", "dbo.Players", "Id");
            AddForeignKey("dbo.ChessBoards", "SecondPlayer_Id", "dbo.Players", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChessBoards", "SecondPlayer_Id", "dbo.Players");
            DropForeignKey("dbo.ChessBoards", "FirstPlayer_Id", "dbo.Players");
            DropIndex("dbo.ChessBoards", new[] { "SecondPlayer_Id" });
            DropIndex("dbo.ChessBoards", new[] { "FirstPlayer_Id" });
            AlterColumn("dbo.ChessBoards", "SecondPlayer_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.ChessBoards", "FirstPlayer_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.ChessBoards", "SecondPlayer_Id");
            CreateIndex("dbo.ChessBoards", "FirstPlayer_Id");
            AddForeignKey("dbo.ChessBoards", "SecondPlayer_Id", "dbo.Players", "Id");
            AddForeignKey("dbo.ChessBoards", "FirstPlayer_Id", "dbo.Players", "Id");
        }
    }
}
