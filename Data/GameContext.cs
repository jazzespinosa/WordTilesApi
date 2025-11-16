using Microsoft.EntityFrameworkCore;
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
        public DbSet<DictionaryEntry> ValidWords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GamesData).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DictionaryEntry).Assembly);

            // Configure any additional properties or customize the configuration as needed
        }

    }
}