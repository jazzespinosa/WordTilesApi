using System.ComponentModel.DataAnnotations.Schema;

namespace WordTilesApi.Models.Entities
{
  public class UserData
  {
    [Column("id")]
    public int Id { get; set; }
    [Column("player_id")]
    public Guid PlayerId { get; set; }
    [Column("email")]
    public required string Email { get; set; }
    [Column("name")]
    public required string Name { get; set; }
    [Column("firebase_uid")]
    public string FirebaseUid { get; set; } = string.Empty;

    public ICollection<GameData> GameData { get; set; } = new List<GameData>();
  }
}
