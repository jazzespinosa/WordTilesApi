using Microsoft.EntityFrameworkCore;
using WordledDictionaryApi.Data;
using WordledDictionaryApi.Models.DTOs;
using WordledDictionaryApi.Models.Entities;
using WordledDictionaryApi.Services.Interfaces;

namespace WordledDictionaryApi.Services.Implementations
{
    public class GameService : IGameService
    {
        private readonly GameContext _gameContext;

        public GameService(GameContext gameContext)
        {
            _gameContext = gameContext;
        }

        public async Task<NewGameResponseDto> CreateGame(Guid playerId, int wordLength, int maxTurns)
        {
            var wordsCount = await _gameContext.ValidWords
                .Where(w => w.Word.Length == wordLength)
                .CountAsync();

            if (wordsCount == 0)
                throw new ArgumentException($"No words of length {wordLength} found.");

            var randomIndex = new Random().Next(wordsCount);
            var gameWord = await _gameContext.ValidWords
                .Where(w => w.Word.Length == wordLength)
                .Skip(randomIndex)
                .Take(1)
                .FirstOrDefaultAsync();

            var gameData = new GameData
            {
                PlayerId = playerId,
                Word = gameWord.Word,
                MaxTurns = maxTurns
            };

            _gameContext.GamesData.Add(gameData);
            await _gameContext.SaveChangesAsync();

            return new NewGameResponseDto
            {
                GameId = gameData.GameId,
                PlayerId = gameData.PlayerId,
                WordLength = gameData.Word.Length,
                MaxTurns = gameData.MaxTurns
            };
        }

        public async Task<GuessResponseDto> ProcessGuess(int gameId, Guid playerId, string guess)
        {
            //check if game exists
            var game = await GetGame(gameId, playerId);
            if (game == null)
                throw new ArgumentException("Game not found.");

            //check if turn is valid
            var currentTurn = await GetCurrentTurn(game.GameId);
            var maxTurn = game.MaxTurns;

            if (currentTurn <= 0 || currentTurn > maxTurn)
                throw new ArgumentException("Turn number must be greater than zero and less than or equal to the maximum number of turns.");

            //check if guess is valid
            if (string.IsNullOrWhiteSpace(guess))
                throw new ArgumentException("Guess cannot be empty.");

            bool isGuessValid = await IsGuessValid(game.Word.Length, guess);
            if (!isGuessValid)
                throw new ArgumentException($"Invalid guess word - '{guess}'.");

            //prepare guess log
            var guessLog = new GuessLog
            {
                GameId = game.GameId,
                Guess = guess,
                GuessTime = DateTime.UtcNow,
                IsCorrect = false,
                Turn = currentTurn
            };

            //prepare result
            var result = new GuessResponseDto
            {
                GameId = game.GameId,
                PlayerId = game.PlayerId,
                Guess = guess,
                Turn = currentTurn,
                IsGuessCorrect = false
            };

            //check if guess is correct
            bool isGuessCorrect = await IsGuessCorrect(game.GameId, game.PlayerId, guess);
            if (isGuessCorrect)
            {
                guessLog.IsCorrect = true;
                result.IsGuessCorrect = true;

                //update game status
                await UpdateStatus(Status.Won, game);
            }
            else
            {
                guessLog.IsCorrect = false;
                result.IsGuessCorrect = false;

                if (currentTurn == maxTurn)
                {
                    //update game status
                    await UpdateStatus(Status.Lost, game);
                }
            }

            //save guess log
            await _gameContext.GuessLogs.AddAsync(guessLog);
            await _gameContext.SaveChangesAsync();

            return result;
        }

        public async Task<GameData?> GetGame(int gameId, Guid playerId)
        {
            var game = await _gameContext.GamesData
                .FirstOrDefaultAsync(g => g.GameId == gameId && g.PlayerId == playerId && g.GameStatus == Status.InProgress);

            return game;
        }

        public async Task<int> GetCurrentTurn(int gameId)
        {
            var turnCount = await _gameContext.GuessLogs
                .CountAsync(g => g.GameId == gameId);

            return turnCount + 1;
        }

        public async Task<bool> IsGuessValid(int gameWordLength, string guess)
        {
            if (guess.Length != gameWordLength)
                throw new ArgumentException("Guess word length does not match the game word length.");

            var word = await _gameContext.ValidWords
                .FirstOrDefaultAsync(w => w.Word.Value == guess);

            // if (guess == null)
            //     return false;

            return word != null;
        }

        public async Task<bool> IsGuessCorrect(int gameId, Guid playerId, string guess)
        {
            var game = await _gameContext.GamesData
                .FirstOrDefaultAsync(g => g.GameId == gameId && g.PlayerId == playerId);

            if (game == null)
                return false;

            return string.Equals(game.Word.Value, guess, StringComparison.OrdinalIgnoreCase);
        }

        public async Task UpdateStatus(Status status, GameData game)
        {
            game.GameStatus = status;
            _gameContext.GamesData.Update(game);
            await _gameContext.SaveChangesAsync();
        }

    }
}

