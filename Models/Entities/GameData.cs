using System.ComponentModel.DataAnnotations.Schema;

namespace WordTilesApi.Models.Entities
{
  public class GameData
  {
    [Column("game_id")]
    public int GameId { get; set; }
    [Column("start_time")]
    public DateTime StartTime { get; set; }
    [Column("end_time")]
    public DateTime? EndTime { get; set; }
    [Column("duration")] // Not mapped to database, calculated property
    public TimeSpan? Duration => EndTime.HasValue ? EndTime.Value - StartTime : null;
    public required WordData Word { get; set; }
    [Column("player_id")]
    public Guid PlayerId { get; set; }
    [Column("max_turns")]
    public int MaxTurns { get; set; }
    [Column("game_status")]
    public Status GameStatus { get; set; } = Status.Unknown;
    [Column("turns_taken")]
    public int? TurnsTaken { get; set; } = 0;

    public ICollection<GuessLog> GuessLogs { get; set; } = new List<GuessLog>();
    public UserData UserData { get; set; }
  }

  public enum Status
  {
    Unknown = 0,
    InProgress = 1,
    Won = 2,
    Lost = 3
  }
}
