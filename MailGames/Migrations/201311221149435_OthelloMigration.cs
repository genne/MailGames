namespace MailGames.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OthelloMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OthelloBoards",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        LastReminded = c.DateTime(),
                        WinnerState = c.Int(),
                        FirstPlayer_Id = c.Int(),
                        SecondPlayer_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Players", t => t.FirstPlayer_Id)
                .ForeignKey("dbo.Players", t => t.SecondPlayer_Id)
                .Index(t => t.FirstPlayer_Id)
                .Index(t => t.SecondPlayer_Id);
            
            CreateTable(
                "dbo.OthelloMoves",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Position = c.Int(nullable: false),
                        DateTime = c.DateTime(nullable: false),
                        OthelloBoard_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OthelloBoards", t => t.OthelloBoard_Id)
                .Index(t => t.OthelloBoard_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OthelloBoards", "SecondPlayer_Id", "dbo.Players");
            DropForeignKey("dbo.OthelloMoves", "OthelloBoard_Id", "dbo.OthelloBoards");
            DropForeignKey("dbo.OthelloBoards", "FirstPlayer_Id", "dbo.Players");
            DropIndex("dbo.OthelloBoards", new[] { "SecondPlayer_Id" });
            DropIndex("dbo.OthelloMoves", new[] { "OthelloBoard_Id" });
            DropIndex("dbo.OthelloBoards", new[] { "FirstPlayer_Id" });
            DropTable("dbo.OthelloMoves");
            DropTable("dbo.OthelloBoards");
        }
    }
}
