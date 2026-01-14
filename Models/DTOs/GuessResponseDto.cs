namespace WordTilesApi.Models.DTOs
{
  public class GuessResponseDto
  {
    public int GameId { get; set; }
    public required string Guess { get; set; }
    public int Turn { get; set; }
    public bool IsGuessCorrect { get; set; } = false;
    public string Answer { get; set; } = string.Empty;
    public List<LetterState> LetterStates { get; set; } = new List<LetterState>();
  }

  public enum LetterState
  {
    Correct,
    Present,
    Incorrect,
    Default
  }
}
