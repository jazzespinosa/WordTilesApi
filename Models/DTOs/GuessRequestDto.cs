namespace WordTilesApi.Models.DTOs
{
  public class GuessRequestDto
  {
    public int GameId { get; set; }
    public required string Guess { get; set; }
  }
}
