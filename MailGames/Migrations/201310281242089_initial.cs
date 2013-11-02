namespace MailGames.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChessBoards",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        BlackPlayer_Id = c.Int(nullable: false),
                        WhitePlayer_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Players", t => t.BlackPlayer_Id)
                .ForeignKey("dbo.Players", t => t.WhitePlayer_Id)
                .Index(t => t.BlackPlayer_Id)
                .Index(t => t.WhitePlayer_Id);
            
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Guid = c.Guid(nullable: false),
                        Mail = c.String(),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ChessMoves",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        From = c.Int(nullable: false),
                        To = c.Int(nullable: false),
                        ChessBoard_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChessBoards", t => t.ChessBoard_Id)
                .Index(t => t.ChessBoard_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChessBoards", "WhitePlayer_Id", "dbo.Players");
            DropForeignKey("dbo.ChessMoves", "ChessBoard_Id", "dbo.ChessBoards");
            DropForeignKey("dbo.ChessBoards", "BlackPlayer_Id", "dbo.Players");
            DropIndex("dbo.ChessBoards", new[] { "WhitePlayer_Id" });
            DropIndex("dbo.ChessMoves", new[] { "ChessBoard_Id" });
            DropIndex("dbo.ChessBoards", new[] { "BlackPlayer_Id" });
            DropTable("dbo.ChessMoves");
            DropTable("dbo.Players");
            DropTable("dbo.ChessBoards");
        }
    }
}
