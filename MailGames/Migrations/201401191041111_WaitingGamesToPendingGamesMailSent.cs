namespace MailGames.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WaitingGamesToPendingGamesMailSent : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.WaitingGames", "Player_Id", "dbo.Players");
            DropIndex("dbo.WaitingGames", new[] { "Player_Id" });
            AddColumn("dbo.Players", "PendingGamesMailSent", c => c.DateTime());
            DropTable("dbo.WaitingGames");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.WaitingGames",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GameType = c.Int(nullable: false),
                        GameId = c.Guid(nullable: false),
                        DateTime = c.DateTime(nullable: false),
                        Player_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.Players", "PendingGamesMailSent");
            CreateIndex("dbo.WaitingGames", "Player_Id");
            AddForeignKey("dbo.WaitingGames", "Player_Id", "dbo.Players", "Id");
        }
    }
}
