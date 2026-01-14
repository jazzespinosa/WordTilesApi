using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WordTilesApi.Data;
using WordTilesApi.Models.DTOs;
using WordTilesApi.Models.Entities;
using WordTilesApi.Services.Interfaces;

namespace WordTilesApi.Services.Implementations
{
  public class UserService : IUserService
  {
    private readonly GameContext _gameContext;
    private readonly string[] _adjectives;
    private readonly string[] _nouns;

    public UserService(GameContext gameContext, IConfiguration configuration)
    {
      _gameContext = gameContext;

      // Load words from configuration on startup
      _adjectives = configuration.GetSection("GeneratorSettings:Adjectives").Get<string[]>() ?? Array.Empty<string>();
      _nouns = configuration.GetSection("GeneratorSettings:Nouns").Get<string[]>() ?? Array.Empty<string>();
    }

    public async Task<UserSignUpResponseDto> SignUp(UserSignUpRequestDto userSignUpRequestDto, string firebaseUid)
    {
      var userData = await CreateUser(userSignUpRequestDto, firebaseUid);

      var response = new UserSignUpResponseDto
      {
        Email = userData.Email,
        Name = userData.Name
      };

      return response;
    }

    private async Task<UserData> CreateUser(UserSignUpRequestDto userSignUpRequestDto, string firebaseUid)
    {
      var userData = new UserData
      {
        PlayerId = Guid.NewGuid(),
        Email = userSignUpRequestDto.Email,
        Name = userSignUpRequestDto.Name,
        FirebaseUid = firebaseUid,
      };

      _gameContext.UserData.Add(userData);
      await _gameContext.SaveChangesAsync();

      return userData;
    }

    public async Task<UserLoginResponseDto> Login(UserLoginRequestDto userLoginRequestDto, string firebaseUid)
    {
      var userData = await _gameContext.UserData
          .FirstOrDefaultAsync(u => u.Email == userLoginRequestDto.Email && u.FirebaseUid == firebaseUid);

      if (userData == null)
      {
        var userName = userLoginRequestDto.Email == "guest" ? GenerateGuestName() : userLoginRequestDto.Email[..userLoginRequestDto.Email.IndexOf('@')];
        var userDataCreated = await CreateUser(new UserSignUpRequestDto
        {
          Email = userLoginRequestDto.Email,
          Name = userName,
        }, firebaseUid);

        return new UserLoginResponseDto
        {
          Email = userDataCreated.Email,
          Name = userDataCreated.Name
        };
      }

      var response = new UserLoginResponseDto
      {
        Email = userData.Email,
        Name = userData.Name
      };

      return response;
    }

    public async Task<Guid> GetPlayerId(string firebaseUid)
    {
      var userData = await _gameContext.UserData
          .Where(u => u.FirebaseUid == firebaseUid)
          .FirstOrDefaultAsync();

      userData = userData ?? throw new Exception("User not found.");

      return userData.PlayerId;
    }

    private string GenerateGuestName()
    {
      int randomSuffix = new Random().Next(10000, 99999);
      if (!_adjectives.Any() || !_nouns.Any()) return $"UnknownUser{randomSuffix}";

      // Random.Shared is the high-performance, thread-safe way to get random values
      var adj = _adjectives[Random.Shared.Next(_adjectives.Length)];
      var noun = _nouns[Random.Shared.Next(_nouns.Length)];

      return $"{adj}{noun}{randomSuffix}";
    }
  }
}
