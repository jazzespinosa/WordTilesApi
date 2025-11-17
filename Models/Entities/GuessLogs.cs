namespace WordledDictionaryApi.Models.Entities
{
    public class GuessLogs
    {
        public Guid TransactionId { get; set; }
        public Guid GameId { get; set; }
        public required string Guess { get; set; }
        public required DateTime GuessTime { get; set; }
        public required bool IsCorrect { get; set; }
        public int Turn { get; set; }

    }

}