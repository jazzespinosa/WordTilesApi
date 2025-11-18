using System.ComponentModel.DataAnnotations.Schema;

namespace WordledDictionaryApi.Models.Entities
{
    public class ValidWord
    {
        [Column("id")]
        public int Id { get; set; }
        public required WordData Word { get; set; }

    }

}
