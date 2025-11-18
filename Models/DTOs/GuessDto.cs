namespace WordledDictionaryApi.Models.DTOs
{
    public class GuessDto
    {
        public Guid GameId { get; set; }
        public Guid PlayerId { get; set; }
        public required string Guess { get; set; }
        public int Turn { get; set; }
        public bool IsCorrect { get; set; } = false;
    }

    /// need to separate GuessDto for request and response if more properties are needed in response
}