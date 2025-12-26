using Microsoft.EntityFrameworkCore;
using WordTilesApi.Data;
using WordTilesApi.Models.DTOs;
using WordTilesApi.Models.Entities;
using WordTilesApi.Services.Interfaces;

namespace WordTilesApi.Services.Implementations
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
            var solutionsCount = await _gameContext.ValidWords
                .Where(w => w.Word.Length == wordLength && w.IsSolution)
                .CountAsync();

            if (solutionsCount == 0)
                throw new ArgumentException($"No words of length {wordLength} found.");

            var randomIndex = new Random().Next(solutionsCount);
            var gameWord = await _gameContext.ValidWords
                .Where(w => w.Word.Length == wordLength && w.IsSolution)
                .Skip(randomIndex)
                .Take(1)
                .FirstOrDefaultAsync();

            var gameData = new GameData
            {
                PlayerId = playerId,
                Word = gameWord.Word,
                GameStatus = Status.InProgress,
                MaxTurns = maxTurns
            };

            var inProgressGames = await _gameContext.GamesData
                .Where(gd => gd.GameStatus == Status.InProgress && gd.PlayerId == playerId)
                .ToArrayAsync();

            foreach (var game in inProgressGames)
            {
                game.GameStatus = Status.Lost;
                await _gameContext.SaveChangesAsync();
            }

            _gameContext.GamesData.Add(gameData);
            await _gameContext.SaveChangesAsync();

            var guessLog = new GuessLog
            {
                GameId = gameData.GameId,
                Guess = "NEW_GAME",
                GuessTime = DateTime.UtcNow,
                IsCorrect = false,
                Turn = 0,
                MaxTurns = gameData.MaxTurns
            };

            _gameContext.GuessLogs.Add(guessLog);
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
                Guess = guess.Trim().ToUpper(),
                GuessTime = DateTime.UtcNow,
                IsCorrect = false,
                Turn = currentTurn,
                MaxTurns = maxTurn
            };

            //prepare result
            var result = new GuessResponseDto
            {
                GameId = game.GameId,
                PlayerId = game.PlayerId,
                Guess = guess,
                Turn = currentTurn,
                IsGuessCorrect = false,
                Answer = string.Empty,
                LetterStates = new LetterState[guess.Length]
            };

            //check if guess is correct
            var isGuessCorrect = await IsGuessCorrect(game.GameId, game.PlayerId, guess);
            result.LetterStates = isGuessCorrect.LetterResults;
            if (isGuessCorrect.IsCorrect)
            {
                guessLog.IsCorrect = true;
                result.IsGuessCorrect = true;
                result.Answer = game.Word.Value;

                //update game status
                await UpdateStatus(Status.Won, game);
            }
            else
            {
                guessLog.IsCorrect = false;
                result.IsGuessCorrect = false;

                if (currentTurn == maxTurn)
                {
                    result.Answer = game.Word.Value;

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

        public async Task<GetGameResponseDto?> GetGame(Guid playerId)
        {
            var game = await _gameContext.GamesData
                .OrderByDescending(g => g.GameId)
                .FirstOrDefaultAsync(g => g.PlayerId == playerId && g.GameStatus == Status.InProgress);

            if (game == null)
                return null;

            var guesses = await _gameContext.GuessLogs
                    .Where(g => g.GameId == game.GameId && g.Turn > 0)
                    .OrderBy(g => g.Turn)
                    .Select(g => g.Guess)
                    .ToArrayAsync();

            GuessesDto[] guessesDto = [];
            foreach (var guess in guesses)
            {
                var letterStates = await GetLetterStates(game.Word.Value, guess);
                var guessDto = new GuessesDto
                {
                    Guess = guess,
                    LetterStates = letterStates
                };
                guessesDto = guessesDto.Append(guessDto).ToArray();
            }

            var response = new GetGameResponseDto
            {
                GameId = game.GameId,
                PlayerId = game.PlayerId,
                CurrentTurn = await GetCurrentTurn(game.GameId),
                GuessLength = game.Word.Length,
                MaxTurns = game.MaxTurns,
                Guesses = guessesDto
            };

            return response;
        }

        public async Task<int> GetCurrentTurn(int gameId)
        {
            var turnCount = await _gameContext.GuessLogs
                .CountAsync(g => g.GameId == gameId);

            return turnCount;
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

        public async Task<(bool IsCorrect, LetterState[] LetterResults)> IsGuessCorrect(int gameId, Guid playerId, string guess)
        {
            var game = await _gameContext.GamesData
                .FirstOrDefaultAsync(g => g.GameId == gameId && g.PlayerId == playerId);

            if (game == null)
                return (false, Array.Empty<LetterState>());

            var gameWord = game.Word.Value.Trim().ToUpper();
            var guessWord = guess.Trim().ToUpper();

            var letterResults = await GetLetterStates(gameWord, guessWord);

            return (gameWord == guessWord, letterResults);
        }

        public async Task<LetterState[]> GetLetterStates(string gameWord, string guessWord)
        {
            var results = new LetterState[guessWord.Length];

            for (int i = 0; i < guessWord.Length; i++)
            {
                // check state of letter
                LetterState state = LetterState.Default;
                bool isPresent = false;

                for (int j = 0; j < gameWord.Length; j++)
                {
                    if (guessWord[i] == gameWord[j] && i == j)
                    {
                        state = LetterState.Correct;
                        break;
                    }
                    else if (guessWord[i] == gameWord[j])
                    {
                        isPresent = true;
                    }

                    if (isPresent)
                    {
                        state = LetterState.Present;
                    }
                    else
                    {
                        state = LetterState.Incorrect;
                    }
                }

                results[i] = state;
            }

            return results;
        }

        public async Task UpdateStatus(Status status, GameData game)
        {
            game.GameStatus = status;
            _gameContext.GamesData.Update(game);
            await _gameContext.SaveChangesAsync();
        }

        public async Task<Status?> GetGameState(int gameId, Guid playerId)
        {
            return await _gameContext.GamesData
            .Where(g => g.GameId == gameId && g.PlayerId == playerId)
            .Select(g => g.GameStatus).FirstOrDefaultAsync();
        }

        public async Task<string> GetAnswer(int gameId, Guid playerId)
        {
            var gameState = await GetGameState(gameId, playerId);
            if (gameState == null || gameState == Status.Unknown)
            {
                throw new ArgumentException("Game not found.");
            }
            else if (gameState == Status.InProgress)
            {
                throw new ArgumentException("Game is still in progress.");
            }
            else
            {
                return await _gameContext.GamesData
                .Where(g => g.GameId == gameId && g.PlayerId == playerId)
                .Select(g => g.Word.Value)
                .FirstOrDefaultAsync() ?? throw new ArgumentException("Game not found.");
            }
        }

        public async Task<ValidWord?> GetWord(string word)
        {
            return await _gameContext.ValidWords
                .FirstOrDefaultAsync(w => w.Word.Value.ToUpper() == word.ToUpper());
        }

        public async Task<int> AddWords(WordDto[] entries)
        {
            const int batchSize = 1000;
            int count = 0;
            var batch = new List<ValidWord>(batchSize);

            foreach (var item in entries)
            {
                batch.Add(new ValidWord
                {
                    Word = new WordData(item.Word.ToUpper()),
                    IsSolution = item.IsSolution
                });

                if (batch.Count == batchSize)
                {
                    await _gameContext.ValidWords.AddRangeAsync(batch);
                    await _gameContext.SaveChangesAsync();
                    batch.Clear();
                }
                count++;
            }

            // Save remaining items
            if (batch.Count > 0)
            {
                await _gameContext.ValidWords.AddRangeAsync(batch);
                await _gameContext.SaveChangesAsync();
            }
            return count;
        }

        public async Task<int> ImportWordsFromCsv(IFormFile file)
        {
            const int batchSize = 1000;
            int count = 0;
            var batch = new List<ValidWord>(batchSize);

            _gameContext.ChangeTracker.AutoDetectChangesEnabled = false;

            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);

            // Skip header line
            await reader.ReadLineAsync();

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(',');

                if (parts.Length < 2)
                    continue;

                batch.Add(new ValidWord
                {
                    Word = new WordData(parts[0].Trim().ToUpper()),
                    IsSolution = bool.Parse(parts[1])
                });

                if (batch.Count == batchSize)
                {
                    await _gameContext.ValidWords.AddRangeAsync(batch);
                    await _gameContext.SaveChangesAsync();

                    count += batch.Count;
                    batch.Clear();
                }
            }

            // Insert remaining rows
            if (batch.Count > 0)
            {
                await _gameContext.ValidWords.AddRangeAsync(batch);
                await _gameContext.SaveChangesAsync();
                count += batch.Count;
            }
            _gameContext.ChangeTracker.AutoDetectChangesEnabled = true;

            return count;
        }

    }
}

