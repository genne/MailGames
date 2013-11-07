namespace MailGames.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TicTacToeMigration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ChessBoards", "BlackPlayer_Id", "dbo.Players");
            DropForeignKey("dbo.ChessBoards", "WhitePlayer_Id", "dbo.Players");
            DropIndex("dbo.ChessBoards", new[] { "BlackPlayer_Id" });
            DropIndex("dbo.ChessBoards", new[] { "WhitePlayer_Id" });
            CreateTable(
                "dbo.TicTacToeBoards",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FirstPlayer_Id = c.Int(),
                        SecondPlayer_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Players", t => t.FirstPlayer_Id)
                .ForeignKey("dbo.Players", t => t.SecondPlayer_Id)
                .Index(t => t.FirstPlayer_Id)
                .Index(t => t.SecondPlayer_Id);
            
            CreateTable(
                "dbo.TicTacToeMoves",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        X = c.Int(nullable: false),
                        Y = c.Int(nullable: false),
                        TicTacToeBoard_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TicTacToeBoards", t => t.TicTacToeBoard_Id)
                .Index(t => t.TicTacToeBoard_Id);
            
            RenameColumn("dbo.ChessBoards", "WhitePlayer_Id", "FirstPlayer_Id");
            RenameColumn("dbo.ChessBoards", "BlackPlayer_Id", "SecondPlayer_Id");
            CreateIndex("dbo.ChessBoards", "FirstPlayer_Id");
            CreateIndex("dbo.ChessBoards", "SecondPlayer_Id");
            AddForeignKey("dbo.ChessBoards", "FirstPlayer_Id", "dbo.Players", "Id");
            AddForeignKey("dbo.ChessBoards", "SecondPlayer_Id", "dbo.Players", "Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ChessBoards", "WhitePlayer_Id", c => c.Int(nullable: false));
            AddColumn("dbo.ChessBoards", "BlackPlayer_Id", c => c.Int(nullable: false));
            DropForeignKey("dbo.TicTacToeBoards", "SecondPlayer_Id", "dbo.Players");
            DropForeignKey("dbo.TicTacToeMoves", "TicTacToeBoard_Id", "dbo.TicTacToeBoards");
            DropForeignKey("dbo.TicTacToeBoards", "FirstPlayer_Id", "dbo.Players");
            DropForeignKey("dbo.ChessBoards", "SecondPlayer_Id", "dbo.Players");
            DropForeignKey("dbo.ChessBoards", "FirstPlayer_Id", "dbo.Players");
            DropIndex("dbo.TicTacToeBoards", new[] { "SecondPlayer_Id" });
            DropIndex("dbo.TicTacToeMoves", new[] { "TicTacToeBoard_Id" });
            DropIndex("dbo.TicTacToeBoards", new[] { "FirstPlayer_Id" });
            DropIndex("dbo.ChessBoards", new[] { "SecondPlayer_Id" });
            DropIndex("dbo.ChessBoards", new[] { "FirstPlayer_Id" });
            DropColumn("dbo.ChessBoards", "SecondPlayer_Id");
            DropColumn("dbo.ChessBoards", "FirstPlayer_Id");
            DropTable("dbo.TicTacToeMoves");
            DropTable("dbo.TicTacToeBoards");
            CreateIndex("dbo.ChessBoards", "WhitePlayer_Id");
            CreateIndex("dbo.ChessBoards", "BlackPlayer_Id");
            AddForeignKey("dbo.ChessBoards", "WhitePlayer_Id", "dbo.Players", "Id");
            AddForeignKey("dbo.ChessBoards", "BlackPlayer_Id", "dbo.Players", "Id");
        }
    }
}
