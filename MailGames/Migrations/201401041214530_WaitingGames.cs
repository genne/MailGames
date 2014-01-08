namespace MailGames.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WaitingGames : DbMigration
    {
        public override void Up()
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Players", t => t.Player_Id)
                .Index(t => t.Player_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WaitingGames", "Player_Id", "dbo.Players");
            DropIndex("dbo.WaitingGames", new[] { "Player_Id" });
            DropTable("dbo.WaitingGames");
        }
    }
}
