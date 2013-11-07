namespace MailGames.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MovesDateTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChessMoves", "DateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.TicTacToeMoves", "DateTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TicTacToeMoves", "DateTime");
            DropColumn("dbo.ChessMoves", "DateTime");
        }
    }
}
