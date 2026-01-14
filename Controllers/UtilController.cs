using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WordTilesApi.Models.DTOs;
using WordTilesApi.Services.Interfaces;

namespace WordTilesApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class UtilController : ControllerBase
  {
    private readonly IUserService _userService;
    private readonly IUtilService _utilService;

    public UtilController(IUserService userService, IUtilService utilService)
    {
      _userService = userService;
      _utilService = utilService;
    }

    [HttpPost("feedback")]
    [Authorize]
    public async Task<IActionResult> SendEmail([FromBody] FeedbackRequestDto feedbackRequestDto)
    {
      var playerId = await GetAuthorizedPlayerId();
      var response = await _utilService.TriggerEmailSend(feedbackRequestDto, playerId);
      return Ok(response);
    }

    [HttpGet("online")]
    public async Task<IActionResult> OnlineCheck()
    {
      return Ok(new { status = "online" });
    }

    private async Task<Guid> GetAuthorizedPlayerId()
    {
      var firebaseUid = User.FindFirst("user_id")?.Value ?? string.Empty;

      return await _userService.GetPlayerId(firebaseUid);
    }
  }
}
