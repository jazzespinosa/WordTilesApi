using WordTilesApi.Models.Entities;

namespace WordTilesApi.Models.DTOs
{
  public class GetHistoryDto
  {
    public int GameId { get; set; }
    public DateTime Date { get; set; }
    public required string Word { get; set; }
    public Status Result { get; set; }
    public int TurnsSolved { get; set; }
    public int MaxTurns { get; set; }
  }
}
