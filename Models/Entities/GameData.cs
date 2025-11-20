using System.ComponentModel.DataAnnotations.Schema;

namespace WordledDictionaryApi.Models.Entities
{
    public class GameData
    {
        [Column("game_id")]
        public int GameId { get; set; }
        public required WordData Word { get; set; }
        [Column("player_id")]
        public int PlayerId { get; set; }
        [Column("max_turns")]
        public int MaxTurns { get; set; }
    }
}