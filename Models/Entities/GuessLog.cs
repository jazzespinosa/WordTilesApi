using System.ComponentModel.DataAnnotations.Schema;

namespace WordledDictionaryApi.Models.Entities
{
    public class GuessLog
    {
        [Column("transaction_id")]
        public int TransactionId { get; set; }
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

    }

}