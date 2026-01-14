using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WordTilesApi.Data;
using WordTilesApi.Models.DTOs;
using WordTilesApi.Services.Interfaces;
using static Google.Apis.Requests.BatchRequest;

namespace WordTilesApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class GameController : ControllerBase
  {
    private readonly IGameService _gameService;
    private readonly IUserService _userService;

    public GameController(IGameService gameService, IUserService userService)
    {
      _gameService = gameService;
      _userService = userService;
    }

    [HttpPost("newgame")]
    [Authorize]
    public async Task<IActionResult> NewGame([FromBody] NewGameRequestDto newGameRequestDto)
    {
      var playerId = await GetAuthorizedPlayerId();
      var newGameResult = await _gameService.CreateGame(
          playerId,
          newGameRequestDto.WordLength,
          newGameRequestDto.MaxTurns
      );

      return CreatedAtAction(nameof(NewGame), newGameResult);
    }

    [HttpPost("guess")]
    [Authorize]
    public async Task<IActionResult> Guess([FromBody] GuessRequestDto guessRequestDto)
    {
      var playerId = await GetAuthorizedPlayerId();
      var guessResult = await _gameService.ProcessGuess(
          guessRequestDto.GameId,
          playerId,
          guessRequestDto.Guess
      );

      return Ok(guessResult);
    }

    [HttpGet("get-game")]
    [Authorize]
    public async Task<IActionResult> GetActiveGame()
    {
      var playerId = await GetAuthorizedPlayerId();
      var response = await _gameService.GetGameByPlayerID(playerId);

      return Ok(response);
    }

    [HttpGet("get-homestats")]
    [Authorize]
    public async Task<IActionResult> GetHomeStats()
    {
      var playerId = await GetAuthorizedPlayerId();
      var response = await _gameService.GetHomeStats(playerId);

      return Ok(response);
    }

    [HttpGet("get-stats")]
    [Authorize]
    public async Task<IActionResult> GetStats()
    {
      var playerId = await GetAuthorizedPlayerId();
      var response = await _gameService.GetFullStats(playerId);

      return Ok(response);
    }

    [HttpGet("get-history")]
    [Authorize]
    public async Task<IActionResult> GetHistory(int pageNumber = 1, int pageSize = 20)
    {
      var playerId = await GetAuthorizedPlayerId();
      var response = await _gameService.GetFullHistory(playerId, pageNumber, pageSize);

      return Ok(response);
    }

    [HttpGet("game-id/{gameId}")]
    [Authorize]
    public async Task<IActionResult> GetGame(int gameId)
    {
      var playerId = await GetAuthorizedPlayerId();
      var response = await _gameService.GetGame(gameId, playerId);

      return Ok(response);
    }

    private async Task<Guid> GetAuthorizedPlayerId()
    {
      var firebaseUid = User.FindFirst("user_id")?.Value ?? string.Empty;

      //return Guid.Parse("6A137820-519E-4C4D-8543-43D8AC73BF32");
      return await _userService.GetPlayerId(firebaseUid);
    }
  }
}
