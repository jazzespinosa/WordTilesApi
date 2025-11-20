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
            var game = await _gameContext.GamesData
                .FirstOrDefaultAsync(g => g.PlayerId == guessRequestDto.PlayerId && g.GameId == guessRequestDto.GameId);
            if (game == null)
                return NotFound($"Game with ID '{guessRequestDto.GameId}' for Player '{guessRequestDto.PlayerId}' was not found.");

            //check if turn is valid
            var maxTurn = game.MaxTurns;

            // var currentTurn = await _gameContext.GuessLogs
            //     .Where(gl => gl.GameId == game.GameId)
            //     .CountAsync() + 1;
            // if (currentTurn >= maxTurn)
            //     return BadRequest("Maximum number of turns exceeded for this game.");

            if (guessRequestDto.Turn <= 0 || guessRequestDto.Turn > maxTurn)
                return BadRequest("Turn number must be greater than zero.");
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
                Turn = guessRequestDto.Turn
            };

            //prepare result
            var result = new GuessResponseDto
            {
                GameId = guessRequestDto.GameId,
                PlayerId = guessRequestDto.PlayerId,
                Guess = guessRequestDto.Guess,
                Turn = guessRequestDto.Turn + 1, // increment turn for next guess ???
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