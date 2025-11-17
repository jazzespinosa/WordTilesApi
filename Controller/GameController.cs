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
            var randomWord = words[randomIndex];

            var gameData = new GamesData
            {
                GameId = Guid.NewGuid(),
                Word = randomWord.Word,
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


        // GET: api/guess/{word}?turn=1
        [HttpPost("guess")]
        public async Task<IActionResult> GetWord([FromBody] GuessDto guessDto)
        {
            //check if turn is valid
            var maxTurn = _gameContext.GamesData
                .Where(g => g.PlayerId == guessDto.PlayerId && g.GameId == guessDto.GameId)
                .Select(g => g.MaxTurns)
                .FirstOrDefault();
            if (guessDto.Turn <= 0 || guessDto.Turn > maxTurn)
            {
                return BadRequest("Turn number must be greater than zero.");
            }

            //check if guess is valid
            var guess = guessDto.Guess;
            if (string.IsNullOrWhiteSpace(guess))
            {
                return BadRequest("Guess cannot be empty.");
            }
            var entry = await _gameContext.ValidWords
                .FirstOrDefaultAsync(w => w.Word.Value.ToLower() == guess.ToLower());

            if (entry == null)
                return BadRequest($"Invalid guess word - '{guess}'.");

            //prepare guess log
            var guessLog = new GuessLogs
            {
                TransactionId = Guid.NewGuid(),
                GameId = guessDto.GameId,
                Guess = guess.ToUpper(),
                GuessTime = DateTime.UtcNow,
                IsCorrect = false,
                Turn = guessDto.Turn
            };

            //prepare result
            var result = new GuessDto
            {
                GameId = guessDto.GameId,
                PlayerId = guessDto.PlayerId,
                Guess = guessDto.Guess.ToUpper(),
                Turn = guessDto.Turn + 1,
                IsCorrect = false
            };

            //check if word matches game word
            var gameWord = _gameContext.GamesData
                .Where(g => g.PlayerId == guessDto.PlayerId && g.GameId == guessDto.GameId)
                .Select(g => g.Word.Value)
                .FirstOrDefault();

            if (guess.ToUpper() != gameWord?.ToUpper())
            {
                // return BadRequest( $"The word '{guess}' does not match the game word '{gameWord}'.");
                _gameContext.GuessLogs.Add(guessLog);
                await _gameContext.SaveChangesAsync();
                return Ok(result);
            }

            result.IsCorrect = true;
            guessLog.IsCorrect = true;
            _gameContext.GuessLogs.Add(guessLog);
            await _gameContext.SaveChangesAsync();

            return Ok(result);
        }

    }
}