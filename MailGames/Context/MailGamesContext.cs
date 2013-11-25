using System.Data.Entity;

namespace MailGames.Context
{
    public class MailGamesContext : DbContext
    {
        public IDbSet<ChessBoard> ChessBoards { get; set; }

        public IDbSet<Player> Players { get; set; }

        public IDbSet<TicTacToeBoard> TicTacToeBoards { get; set; }

        public IDbSet<OthelloBoard> OthelloBoards { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChessBoard>().HasRequired(b => b.FirstPlayer).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<ChessBoard>().HasRequired(b => b.SecondPlayer).WithMany().WillCascadeOnDelete(false);
            base.OnModelCreating(modelBuilder);
        }
    }
}