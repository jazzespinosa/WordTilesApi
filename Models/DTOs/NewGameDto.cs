namespace WordledDictionaryApi.Models.DTOs
{
    public class NewGameDto
    {
        public Guid PlayerId { get; set; }
        public int WordLength { get; set; }
        public int MaxTurns { get; set; }
    }
}
