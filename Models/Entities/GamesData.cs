namespace WordledDictionaryApi.Models.Entities
{
    public class GamesData
    {
        public Guid GameId { get; set; }
        public required Word Word { get; set; }
        public required string Username { get; set; }
        public int MaxTurns { get; set; }

    }

}