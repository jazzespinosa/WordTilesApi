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

        public string Value { get; }
        public int Length { get; private set; }
    }
}
