namespace WordledDictionaryApi.Models
{
    public class DictionaryEntry
    {
        public int Id { get; set; }
        public string Word { get; set; } = string.Empty;
        public string Definition { get; set; } = string.Empty;
        // public string Example { get; set; }
    }
}
