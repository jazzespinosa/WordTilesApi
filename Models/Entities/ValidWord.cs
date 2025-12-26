using System.ComponentModel.DataAnnotations.Schema;

namespace WordTilesApi.Models.Entities
{
    public class ValidWord
    {
        [Column("id")]
        public int Id { get; set; }
        public required WordData Word { get; set; }
        [Column("is_solution")]
        public bool IsSolution { get; set; }
    }

}
