namespace WordTilesApi.Models.DTOs
{
    public class NewGameResponseDto
    {
        public int GameId { get; set; }
        public int WordLength { get; set; }
        public int MaxTurns { get; set; }
    }
}