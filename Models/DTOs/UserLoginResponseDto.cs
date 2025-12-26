namespace WordTilesApi.Models.DTOs
{
    public class UserLoginResponseDto
    {
        public int email { get; set; }
        public required string name { get; set; }
    }
}