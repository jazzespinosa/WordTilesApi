namespace WordledDictionaryApi.Models.DTOs
{
    public class NewGameResponseDto
    {
        public int GameId { get; set; }
        public Guid PlayerId { get; set; }
        public int WordLength { get; set; }
        public int MaxTurns { get; set; }
    }
}