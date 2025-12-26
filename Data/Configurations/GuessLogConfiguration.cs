using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WordTilesApi.Models.Entities;

public class GuessLogConfiguration : IEntityTypeConfiguration<GuessLog>
{
    public void Configure(EntityTypeBuilder<GuessLog> builder)
    {
        builder.HasKey(l => l.TransactionId);
        builder.HasOne(l => l.GameData)
               .WithMany(g => g.GuessLogs)
               .HasForeignKey(l => l.GameId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}