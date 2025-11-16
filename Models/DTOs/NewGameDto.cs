namespace WordledDictionaryApi.Models.DTOs
{
    public class NewGameDto
    {
        public required string Username { get; set; }
        public int WordLength { get; set; }
        public int MaxTurns { get; set; }
    }
}
