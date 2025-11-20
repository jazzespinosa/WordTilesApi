namespace WordledDictionaryApi.Models.DTOs
{
    public class GuessRequestDto
    {
        public int GameId { get; set; }
        public int PlayerId { get; set; }
        public required string Guess { get; set; }
        public int Turn { get; set; }
    }
}