namespace WordTilesApi.Models.DTOs
{
    public class WordDto
    {
        public int Id { get; set; }
        public string Word { get; set; } = string.Empty;
        public bool IsSolution { get; set; }
    }

}