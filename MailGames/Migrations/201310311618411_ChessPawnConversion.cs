namespace MailGames.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChessPawnConversion : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PawnConversions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ConvertTo = c.Int(nullable: false),
                        ChessMove_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChessMoves", t => t.ChessMove_Id)
                .Index(t => t.ChessMove_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PawnConversions", "ChessMove_Id", "dbo.ChessMoves");
            DropIndex("dbo.PawnConversions", new[] { "ChessMove_Id" });
            DropTable("dbo.PawnConversions");
        }
    }
}
