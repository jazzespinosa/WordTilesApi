using WordTilesApi.Models.DTOs;

namespace WordTilesApi.Services.Interfaces
{
  public interface IUserService
  {
    Task<UserSignUpResponseDto> SignUp(UserSignUpRequestDto userSignUpRequestDto, string firebaseUid);
    Task<UserLoginResponseDto> Login(UserLoginRequestDto userLoginRequestDto, string firebaseUid);
    Task<Guid> GetPlayerId(string firebaseUid);
  }
}
