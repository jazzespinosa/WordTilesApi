using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WordledDictionaryApi.Models.Entities;

public class GuessLogConfiguration : IEntityTypeConfiguration<GuessLog>
{
    public void Configure(EntityTypeBuilder<GuessLog> builder)
    {
        builder.HasKey(l => l.TransactionId);
        builder.HasOne(g => g.GameData)
               .WithMany(l => l.GuessLogs)
               .HasForeignKey(l => l.GameId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}