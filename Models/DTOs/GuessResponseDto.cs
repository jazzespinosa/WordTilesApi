namespace WordledDictionaryApi.Models.DTOs
{
    public class GuessResponseDto
    {
        public int GameId { get; set; }
        public Guid PlayerId { get; set; }
        public required string Guess { get; set; }
        public int Turn { get; set; }
        public bool IsCorrect { get; set; } = false;
    }
}