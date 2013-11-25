namespace MailGames.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TicTacToeVariants : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TicTacToeBoards", "Variant", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TicTacToeBoards", "Variant");
        }
    }
}
