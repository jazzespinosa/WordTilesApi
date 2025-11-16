using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WordledDictionaryApi.Models.Entities;

public class WordsConfiguration : IEntityTypeConfiguration<DictionaryEntry>
{
    public void Configure(EntityTypeBuilder<DictionaryEntry> builder)
    {
        builder.HasKey(e => e.Id);
        builder.OwnsOne(e => e.Word, word =>
        {
            word.Property(w => w.Value).HasColumnType("string").IsRequired();
            word.Property(w => w.Length).HasColumnType("int").IsRequired();
        });
    }
}