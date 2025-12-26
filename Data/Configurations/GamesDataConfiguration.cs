using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WordTilesApi.Models.Entities;

public class GamesDataConfiguration : IEntityTypeConfiguration<GameData>
{
    public void Configure(EntityTypeBuilder<GameData> builder)
    {
        builder.HasKey(g => g.GameId);
        builder.OwnsOne(g => g.Word, word =>
        {
            word.Property(w => w.Value)
                .HasColumnType("string")
                .IsRequired();
            word.Property(w => w.Length)
                .HasColumnType("int")
                .IsRequired();
        });
        builder.Property(g => g.GameStatus)
            .HasConversion<string>()
            .HasColumnType("string")
            .IsRequired();
        builder.HasOne(g => g.UserData)
            .WithMany(u => u.GameData)
            .HasForeignKey(g => g.PlayerId)
            .HasPrincipalKey(u => u.PlayerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

    }
}