using System.ComponentModel.DataAnnotations.Schema;

namespace WordledDictionaryApi.Models.Entities
{
    public class GameData
    {
        [Column("game_id")]
        public int GameId { get; set; }
        public required WordData Word { get; set; }
        [Column("player_id")]
        public Guid PlayerId { get; set; }
        [Column("max_turns")]
        public int MaxTurns { get; set; }
        [Column("is_win")]
        public Status GameStatus { get; set; } = Status.InProgress;
        public ICollection<GuessLog> GuessLogs { get; set; } = new List<GuessLog>();
    }

    public enum Status
    {
        InProgress,
        Won,
        Lost
    }
}