using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WordTilesApi.Models.Entities;

namespace WordTilesApi.Data
{
    public class GameContext : DbContext
    {
        public GameContext(DbContextOptions<GameContext> options)
            : base(options)
        {
        }

        public DbSet<GameData> GamesData { get; set; }
        public DbSet<GuessLog> GuessLogs { get; set; }
        public DbSet<ValidWord> ValidWords { get; set; }
        public DbSet<UserData> UserData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GameData).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GuessLog).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ValidWord).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserData).Assembly);

            base.OnModelCreating(modelBuilder);
        }

    }
}