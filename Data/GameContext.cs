using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WordledDictionaryApi.Models.Entities;

namespace WordledDictionaryApi.Data
{
    public class GameContext : DbContext
    {
        public GameContext(DbContextOptions<GameContext> options)
            : base(options)
        {
        }

        public DbSet<GamesData> GamesData { get; set; }
        public DbSet<ValidWord> ValidWords { get; set; }
        public DbSet<GuessLogs> GuessLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GamesData).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ValidWord).Assembly);

            modelBuilder.Entity<GuessLogs>(entity =>
            {
                entity.HasKey(e => e.TransactionId);
            });


            // Configure any additional properties or customize the configuration as needed
        }

    }
}