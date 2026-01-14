using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WordTilesApi.Models.Entities;

public class GuessLogConfiguration : IEntityTypeConfiguration<GuessLog>
{
  public void Configure(EntityTypeBuilder<GuessLog> builder)
  {
    builder.HasKey(gl => gl.TransactionId);
    builder.HasOne(gl => gl.GameData)
           .WithMany(gd => gd.GuessLogs)
           .HasForeignKey(gl => gl.GameId)
           .OnDelete(DeleteBehavior.Cascade);
    builder.HasIndex(gl => gl.GameId)
           .HasDatabaseName("IX_GuessLog_GameId");
  }
}
