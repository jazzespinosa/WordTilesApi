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

        public DbSet<GameData> GamesData { get; set; }
        public DbSet<ValidWord> ValidWords { get; set; }
        public DbSet<GuessLog> GuessLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GameData).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ValidWord).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GuessLog).Assembly);

            base.OnModelCreating(modelBuilder);
        }

    }
}