namespace WordledDictionaryApi.Models.Entities
{
    public class DictionaryEntry
    {
        public int Id { get; set; }
        public required Word Word { get; set; }

    }

}
