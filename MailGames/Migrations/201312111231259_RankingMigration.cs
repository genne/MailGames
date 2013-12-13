namespace MailGames.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RankingMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PlayerGameRankings",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        GameType = c.Int(nullable: false),
                        Ranking = c.Single(nullable: false),
                        Player_Id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Players", t => t.Player_Id)
                .Index(t => t.Player_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PlayerGameRankings", "Player_Id", "dbo.Players");
            DropIndex("dbo.PlayerGameRankings", new[] { "Player_Id" });
            DropTable("dbo.PlayerGameRankings");
        }
    }
}
