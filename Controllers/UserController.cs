using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WordTilesApi.Models.DTOs;
using WordTilesApi.Services.Interfaces;

namespace WordTilesApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class UserController : ControllerBase
  {
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
      _userService = userService;
    }

    [HttpPost("signup")]
    [Authorize]
    public async Task<IActionResult> SignUp(
      [FromBody] UserSignUpRequestDto userSignUpRequestDto
    )
    {
      var firebaseUid = GetFirebaseUid();
      var response = await _userService.SignUp(
        userSignUpRequestDto,
        firebaseUid
      );
      return CreatedAtAction(nameof(SignUp), response);
    }

    [HttpPost("login")]
    [Authorize]
    public async Task<IActionResult> Login(
      [FromBody] UserLoginRequestDto userLoginRequestDto
    )
    {
      var firebaseUid = GetFirebaseUid();
      var response = await _userService.Login(userLoginRequestDto, firebaseUid);
      return Ok(response);
    }

    [HttpPost("guest")]
    [Authorize]
    public async Task<IActionResult> LoginAsGuest()
    {
      var firebaseUid = GetFirebaseUid();
      var response = await _userService.Login(
        new UserLoginRequestDto { Email = "guest" },
        firebaseUid
      );
      return Ok(response);
    }

    private string GetFirebaseUid() =>
      User.FindFirst("user_id")?.Value ?? string.Empty;
  }
}
