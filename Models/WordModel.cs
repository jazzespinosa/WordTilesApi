namespace WordledDictionaryApi.Models
{
    public class Word
    {
        public Word(string word)
        {
            Value = word;
            Length = word.Length;
        }

        public Word()
        {
            Value = string.Empty;
            Length = 0;
        }

        public string Value { get; }
        public int Length { get; private set; }
    }
}
