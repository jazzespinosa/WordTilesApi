using System.ComponentModel.DataAnnotations.Schema;

namespace WordTilesApi.Models.Entities
{
    public class UserData
    {
        public int Id { get; set; }
        [Column("player_id")]
        public Guid PlayerId { get; set; }
        public required string Email { get; set; }
        public required string Name { get; set; }
        [Column("firebase_uid")]
        public required string FirebaseUid { get; set; }

        public ICollection<GameData> GameData { get; set; } = new List<GameData>();
    }
}