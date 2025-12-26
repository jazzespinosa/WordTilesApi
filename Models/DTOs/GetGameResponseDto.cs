namespace WordTilesApi.Models.DTOs
{
    public class GetGameResponseDto
    {
        public int GameId { get; set; }
        public Guid PlayerId { get; set; }
        public int CurrentTurn { get; set; } = 0;
        public int GuessLength { get; set; }
        public int MaxTurns { get; set; }
        public GuessesDto[] Guesses { get; set; } = [];
    }

    public class GuessesDto
    {
        public string Guess { get; set; } = string.Empty;
        public LetterState[] LetterStates { get; set; } = [];
    }
}