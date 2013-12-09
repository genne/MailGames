namespace MailGames.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JapaneseWhistMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.JapaneseWhistBoards",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        LastReminded = c.DateTime(),
                        WinnerState = c.Int(),
                        Seed = c.Int(nullable: false),
                        FirstPlayer_Id = c.Int(),
                        SecondPlayer_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Players", t => t.FirstPlayer_Id)
                .ForeignKey("dbo.Players", t => t.SecondPlayer_Id)
                .Index(t => t.FirstPlayer_Id)
                .Index(t => t.SecondPlayer_Id);
            
            CreateTable(
                "dbo.JapaneseWhistMoves",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PlayerDeck = c.Int(nullable: false),
                        CardIndex = c.Int(nullable: false),
                        DateTime = c.DateTime(nullable: false),
                        JapaneseWhistBoard_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.JapaneseWhistBoards", t => t.JapaneseWhistBoard_Id)
                .Index(t => t.JapaneseWhistBoard_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.JapaneseWhistBoards", "SecondPlayer_Id", "dbo.Players");
            DropForeignKey("dbo.JapaneseWhistMoves", "JapaneseWhistBoard_Id", "dbo.JapaneseWhistBoards");
            DropForeignKey("dbo.JapaneseWhistBoards", "FirstPlayer_Id", "dbo.Players");
            DropIndex("dbo.JapaneseWhistBoards", new[] { "SecondPlayer_Id" });
            DropIndex("dbo.JapaneseWhistMoves", new[] { "JapaneseWhistBoard_Id" });
            DropIndex("dbo.JapaneseWhistBoards", new[] { "FirstPlayer_Id" });
            DropTable("dbo.JapaneseWhistMoves");
            DropTable("dbo.JapaneseWhistBoards");
        }
    }
}
