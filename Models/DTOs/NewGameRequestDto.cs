namespace WordTilesApi.Models.DTOs
{
    public class NewGameRequestDto
    {
        public Guid PlayerId { get; set; }
        public int WordLength { get; set; }
        public int MaxTurns { get; set; }
        public NewGameRequestDto()
        {
            WordLength = 5; //default word length
            MaxTurns = 6;  //default max turns
        }
    }
}
