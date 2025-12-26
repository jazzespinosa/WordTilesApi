using System.ComponentModel.DataAnnotations.Schema;

namespace WordTilesApi.Models.Entities
{
    public class GuessLog
    {
        [Column("transaction_id")]
        public long TransactionId { get; set; } = GenerateTransactionId();
        [Column("game_id")]
        public int GameId { get; set; }
        [Column("guess")]
        public required string Guess { get; set; }
        [Column("guess_time")]
        public required DateTime GuessTime { get; set; }
        [Column("is_correct")]
        public required bool IsCorrect { get; set; }
        [Column("turn")]
        public int Turn { get; set; }
        [Column("max_turns")]
        public int MaxTurns { get; set; }

        public GameData GameData { get; set; }     // Navigation

        private static int _sequence = 0;
        private static readonly object _lock = new object();

        public static long GenerateTransactionId()
        {
            lock (_lock)
            {
                _sequence = (_sequence + 1) % 100000; // keep within 5 digits
                return long.Parse(
                    DateTime.UtcNow.ToString("yyyyMMddHHmmss") +
                    _sequence.ToString("D5")
                );
            }
        }
    }

}