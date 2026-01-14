namespace WordTilesApi.Models.DTOs
{
    public class UserSignUpRequestDto
    {
        public required string Email { get; set; }
        public required string Name { get; set; }
    }
}