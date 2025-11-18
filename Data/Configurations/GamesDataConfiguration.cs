using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WordledDictionaryApi.Models.Entities;

public class GamesDataConfiguration : IEntityTypeConfiguration<GameData>
{
    public void Configure(EntityTypeBuilder<GameData> builder)
    {
        builder.HasKey(e => e.GameId);
        builder.OwnsOne(e => e.Word, word =>
        {
            word.Property(w => w.Value).HasColumnType("string").IsRequired();
            word.Property(w => w.Length).HasColumnType("int").IsRequired();
        });
    }
}