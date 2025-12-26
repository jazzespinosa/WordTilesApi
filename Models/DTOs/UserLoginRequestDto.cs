namespace WordTilesApi.Models.DTOs
{
    public class UserLoginRequestDto
    {
        public int email { get; set; }
        public required string firebaseUid { get; set; }
    }
}