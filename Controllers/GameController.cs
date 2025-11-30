using Microsoft.AspNetCore.Mvc;
using WordledDictionaryApi.Data;
using WordledDictionaryApi.Models.DTOs;

namespace WordledDictionaryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly GameContext _gameContext;
        private readonly IGameService _service;

        public GameController(GameContext gameContext, IGameService service)
        {
            _gameContext = gameContext;
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

    }
}