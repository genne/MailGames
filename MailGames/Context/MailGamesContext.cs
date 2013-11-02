using System.Data.Entity;

namespace MailGames.Context
{
    public class MailGamesContext : DbContext
    {
        public IDbSet<ChessBoard> ChessBoards { get; set; }

        public IDbSet<Player> Players { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChessBoard>().HasRequired(b => b.WhitePlayer).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<ChessBoard>().HasRequired(b => b.BlackPlayer).WithMany().WillCascadeOnDelete(false);
            base.OnModelCreating(modelBuilder);
        }
    }
}