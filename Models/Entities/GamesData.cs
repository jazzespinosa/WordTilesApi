namespace WordledDictionaryApi.Models.Entities
{
    public class GamesData
    {
        public Guid GameId { get; set; }
        public required WordData Word { get; set; }
        public required Guid PlayerId { get; set; }
        public int MaxTurns { get; set; }

    }

}