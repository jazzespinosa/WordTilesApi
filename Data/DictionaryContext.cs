using Microsoft.EntityFrameworkCore;
using WordledDictionaryApi.Models;

namespace WordledDictionaryApi.Data
{
    public class DictionaryContext : DbContext
    {
        public DictionaryContext(DbContextOptions<DictionaryContext> options)
            : base(options)
        {
        }

        // public DbSet<DictionaryEntry> Entries => Set<DictionaryEntry>();
        public DbSet<DictionaryEntry> Entries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DictionaryEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.OwnsOne(e => e.Word, word =>
                {
                    word.Property(w => w.Value).HasColumnType("string").IsRequired();
                    word.Property(w => w.Length).HasColumnType("int").IsRequired();
                });
            });

            // Configure any additional properties or customize the configuration as needed
        }
    }
}