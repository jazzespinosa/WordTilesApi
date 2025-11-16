using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WordledDictionaryApi.Data;
using WordledDictionaryApi.Models.DTOs;
using WordledDictionaryApi.Models.Entities;

namespace WordledDictionaryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly GameContext _gameContext;

        public GameController(GameContext gameContext)
        {
            _gameContext = gameContext;
        }

        [HttpPost]
        public async Task<IActionResult> NewGame([FromBody] NewGameDto newGameDto)
        {
            var words = await _gameContext.ValidWords
                .Where(e => e.Word.Length == newGameDto.WordLength)
                .ToListAsync();

            if (words.Count == 0)
                return NotFound($"No words of length {newGameDto.WordLength} found.");

            var randomIndex = new Random().Next(words.Count);
            var randomWord = words[randomIndex];

            var gameData = new GamesData
            {
                GameId = Guid.NewGuid(),
                Word = randomWord.Word,
                Username = newGameDto.Username,
                MaxTurns = newGameDto.MaxTurns
            };

            _gameContext.GamesData.Add(gameData);
            await _gameContext.SaveChangesAsync();

            var response = new
            {
                GameId = gameData.GameId,
                Username = gameData.Username,
                MaxTurns = gameData.MaxTurns
            };

            return Ok(response);

        }
    }
}