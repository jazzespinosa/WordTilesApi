using System.ComponentModel.DataAnnotations.Schema;

namespace WordledDictionaryApi.Models.Entities
{
    public class WordData
    {
        public WordData(string word)
        {
            Value = word;
            Length = word.Length;
        }

        public WordData()
        {
            Value = string.Empty;
            Length = 0;
        }

        [Column("word_value")]
        public string Value { get; }
        [Column("word_length")]
        public int Length { get; private set; }
    }
}
