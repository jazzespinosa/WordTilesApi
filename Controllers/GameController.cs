using Microsoft.AspNetCore.Mvc;
using WordTilesApi.Data;
using WordTilesApi.Models.DTOs;
using WordTilesApi.Services.Interfaces;

namespace WordTilesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _service;

        public GameController(IGameService service)
        {
            _service = service;
        }

        [HttpPost("newgame")]
        public async Task<IActionResult> NewGame([FromBody] NewGameRequestDto newGameRequestDto)
        {
            var newGameResult = await _service.CreateGame(
                newGameRequestDto.PlayerId,
                newGameRequestDto.WordLength,
                newGameRequestDto.MaxTurns
            );

            return Ok(newGameResult);
        }

        [HttpPost("guess")]
        public async Task<IActionResult> Guess([FromBody] GuessRequestDto guessRequestDto)
        {
            var guessResult = await _service.ProcessGuess(
                guessRequestDto.GameId,
                guessRequestDto.PlayerId,
                guessRequestDto.Guess
            );

            return Ok(guessResult);
        }

        [HttpGet("get/{playerId}")]
        public async Task<IActionResult> GetActiveGame(Guid playerId)
        {
            var gameResponse = await _service.GetGame(playerId);

            if (gameResponse == null)
                return NotFound("Game not found.");

            return Ok(gameResponse);
        }

        [HttpPost("{gameId}/answer")]
        public async Task<IActionResult> GetAnswer([FromBody] Guid playerId, int gameId)
        {
            var gameAnswer = await _service.GetAnswer(gameId, playerId);

            return Ok(gameAnswer);
        }
    }
}