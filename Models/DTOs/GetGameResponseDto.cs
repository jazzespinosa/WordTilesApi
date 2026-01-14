namespace WordTilesApi.Models.DTOs
{
  public class GetGameResponseDto
  {
    public int GameId { get; set; }
    public int TurnsPlayed { get; set; } = 0;
    public int GuessLength { get; set; }
    public int MaxTurns { get; set; }
    public List<GuessesDto> Guesses { get; set; } = new List<GuessesDto>();
  }

  public class GuessesDto
  {
    public string Guess { get; set; } = string.Empty;
    public List<LetterState> LetterStates { get; set; } = new List<LetterState>();
  }
}
