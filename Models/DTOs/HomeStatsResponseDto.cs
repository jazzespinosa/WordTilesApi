using WordTilesApi.Models.Entities;

namespace WordTilesApi.Models.DTOs
{
  public class HomeStatsResponseDto
  {
    public bool HasExistingGame { get; set; }
    public int GamesPlayed { get; set; }
    public double WinPercentage { get; set; }
    public int CurrentStreak { get; set; }
    public List<GetHistoryDto> HomeGameHistories { get; set; } = new List<GetHistoryDto>();
  }

  //public class HomeGameHistory
  //{
  //  public int GameId { get; set; }
  //  public DateTime Date { get; set; }
  //  public required string Word { get; set; }
  //  public Status Result { get; set; }
  //  public int TurnsSolved { get; set; }
  //  public int MaxTurns { get; set; }
  //}
}
