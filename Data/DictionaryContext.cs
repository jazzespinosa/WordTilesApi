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

        public DbSet<DictionaryEntry> Entries => Set<DictionaryEntry>();
    }
}