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
                GameId = Guid.NewGuid(),
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
        public async Task<IActionResult> Guess([FromBody] GuessDto guessDto)
        {
            //check if game exists
            var game = await _gameContext.GamesData
                .FirstOrDefaultAsync(g => g.PlayerId == guessDto.PlayerId && g.GameId == guessDto.GameId);
            if (game == null)
                return NotFound($"Game with ID '{guessDto.GameId}' for Player '{guessDto.PlayerId}' was not found.");

            //check if turn is valid
            var maxTurn = game.MaxTurns;
            if (guessDto.Turn <= 0 || guessDto.Turn > maxTurn)
                return BadRequest("Turn number must be greater than zero.");
            /// need to validate turn logic more thoroughly
            /// eg. prevent duplicate turns, ensure turns are sequential, etc.

            //check if guess is valid
            var guess = guessDto.Guess;
            if (string.IsNullOrWhiteSpace(guess))
                return BadRequest("Guess cannot be empty.");

            var entry = await _gameContext.ValidWords
                .FirstOrDefaultAsync(w => w.Word.Value == guess);
            if (entry == null)
                return BadRequest($"Invalid guess word - '{guess}'.");

            //prepare guess log
            var guessLog = new GuessLog
            {
                TransactionId = Guid.NewGuid(),
                GameId = guessDto.GameId,
                Guess = guess,
                GuessTime = DateTime.UtcNow,
                IsCorrect = false,
                Turn = guessDto.Turn
            };

            //prepare result
            var result = new GuessDto
            {
                GameId = guessDto.GameId,
                PlayerId = guessDto.PlayerId,
                Guess = guessDto.Guess,
                Turn = guessDto.Turn + 1, // increment turn for next guess ???
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