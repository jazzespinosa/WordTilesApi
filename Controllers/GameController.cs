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
        private readonly IGameService _service;

        public GameController(GameContext gameContext, IGameService service)
        {
            _gameContext = gameContext;
            _service = service;
        }

        [HttpPost("newgame")]
        public async Task<IActionResult> NewGame([FromBody] NewGameDto newGameDto)
        {
            var words = await _gameContext.ValidWords
                .Where(w => w.Word.Length == newGameDto.WordLength)
                .ToListAsync();

            if (words.Count == 0)
                return NotFound($"No words of length {newGameDto.WordLength} found.");

            var randomIndex = new Random().Next(words.Count);
            var gameWord = words[randomIndex];

            var gameData = new GameData
            {
                Word = gameWord.Word,
                PlayerId = newGameDto.PlayerId,
                MaxTurns = newGameDto.MaxTurns
            };

            _gameContext.GamesData.Add(gameData);
            await _gameContext.SaveChangesAsync();

            var response = new
            {
                GameId = gameData.GameId,
                PlayerId = gameData.PlayerId,
                MaxTurns = gameData.MaxTurns
            };

            return Ok(response);

        }

        [HttpPost("guess")]
        public async Task<IActionResult> Guess([FromBody] GuessRequestDto guessRequestDto)
        {
            //check if game exists
            var game = await _service.GetGame(guessRequestDto.GameId, guessRequestDto.PlayerId);
            if (game == null)
                return NotFound($"Game with ID '{guessRequestDto.GameId}' for Player '{guessRequestDto.PlayerId}' was not found.");

            //check if turn is valid
            var maxTurn = game.MaxTurns;
            var currentTurn = await _service.GetCurrentTurn(game.GameId);

            if (currentTurn <= 0 || currentTurn > maxTurn)
                return BadRequest("Turn number must be greater than zero and less than or equal to the maximum number of turns.");
            /// need to validate turn logic more thoroughly
            /// eg. prevent duplicate turns, ensure turns are sequential, etc.

            //check if guess is valid
            var guess = guessRequestDto.Guess;
            if (string.IsNullOrWhiteSpace(guess))
                return BadRequest("Guess cannot be empty.");

            var entry = await _gameContext.ValidWords
                .FirstOrDefaultAsync(w => w.Word.Value == guess);
            if (entry == null)
                return BadRequest($"Invalid guess word - '{guess}'.");

            //prepare guess log
            var guessLog = new GuessLog
            {
                GameId = guessRequestDto.GameId,
                Guess = guess,
                GuessTime = DateTime.UtcNow,
                IsCorrect = false,
                Turn = currentTurn
            };

            //prepare result
            var result = new GuessResponseDto
            {
                GameId = guessRequestDto.GameId,
                PlayerId = guessRequestDto.PlayerId,
                Guess = guessRequestDto.Guess,
                Turn = currentTurn,
                IsCorrect = false
            };

            //check if word matches game word
            var gameWord = game.Word.Value;
            if (guess == gameWord)
            {
                result.IsCorrect = true;
                guessLog.IsCorrect = true;
                _gameContext.GuessLogs.Add(guessLog);
                await _gameContext.SaveChangesAsync();

                return Ok(result);
            }

            // return BadRequest( $"The word '{guess}' does not match the game word '{gameWord}'.");
            _gameContext.GuessLogs.Add(guessLog);
            await _gameContext.SaveChangesAsync();
            return Ok(result);
        }

    }
}