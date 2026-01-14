using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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
        StartTime = DateTime.UtcNow,
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
        game.EndTime = DateTime.UtcNow;
        game.TurnsTaken = 0;
        _gameContext.GamesData.Update(game);
        await _gameContext.SaveChangesAsync();
      }

      _gameContext.GamesData.Add(gameData);
      await _gameContext.SaveChangesAsync();

      return new NewGameResponseDto
      {
        GameId = gameData.GameId,
        WordLength = gameData.Word.Length,
        MaxTurns = gameData.MaxTurns
      };
    }

    public async Task<GuessResponseDto> ProcessGuess(int gameId, Guid playerId, string guess)
    {
      //check if game exists
      var game = await GetActiveGame(gameId, playerId);
      if (game == null)
        throw new ArgumentException("Game not found.");

      //check if turn is valid
      var currentTurn = await GetPlayedTurn(game.GameId) + 1;
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
        Guess = guess,
        Turn = currentTurn,
        IsGuessCorrect = false,
        Answer = string.Empty,
        LetterStates = new List<LetterState>()
      };

      //check if guess is correct
      var isGuessCorrect = await IsGuessCorrect(game.GameId, game.PlayerId, guess);
      result.LetterStates.Clear();
      result.LetterStates.AddRange(isGuessCorrect.LetterResults);
      if (isGuessCorrect.IsCorrect)
      {
        guessLog.IsCorrect = true;
        result.IsGuessCorrect = true;
        result.Answer = game.Word.Value;

        //update game status
        await UpdateStatus(Status.Won, game, currentTurn);
      }
      else
      {
        guessLog.IsCorrect = false;
        result.IsGuessCorrect = false;

        if (currentTurn == maxTurn)
        {
          result.Answer = game.Word.Value;

          //update game status
          await UpdateStatus(Status.Lost, game, currentTurn);
        }
      }

      //save guess log
      await _gameContext.GuessLogs.AddAsync(guessLog);
      await _gameContext.SaveChangesAsync();

      return result;
    }

    private async Task<GameData> GetActiveGame(int gameId, Guid playerId)
    {
      var game = await _gameContext.GamesData
          .FirstOrDefaultAsync(g => g.GameId == gameId && g.PlayerId == playerId && g.GameStatus == Status.InProgress);

      if (game == null)
        throw new ArgumentException("Game not found.");

      return game;
    }

    public async Task<GetGameResponseDto> GetGame(int gameId, Guid playerId)
    {
      var game = await _gameContext.GamesData
          .FirstOrDefaultAsync(g => g.GameId == gameId && g.PlayerId == playerId);

      if (game == null)
        throw new ArgumentException("Game not found.");

      return await ProcessGetGame(game);
    }

    public async Task<GetGameResponseDto?> GetGameByPlayerID(Guid playerId)
    {
      var game = await _gameContext.GamesData
          .OrderByDescending(g => g.GameId)
          .FirstOrDefaultAsync(g => g.PlayerId == playerId && g.GameStatus == Status.InProgress);

      if (game == null)
        throw new ArgumentException("No active game found.");

      return await ProcessGetGame(game);
    }

    private async Task<GetGameResponseDto> ProcessGetGame(GameData gamedata)
    {
      var guesses = await _gameContext.GuessLogs
              .Where(g => g.GameId == gamedata.GameId && g.Turn > 0)
              .OrderBy(g => g.Turn)
              .Select(g => g.Guess)
              .ToArrayAsync();

      GuessesDto[] guessesDto = [];
      foreach (var guess in guesses)
      {
        var letterStates = await GetLetterStates(gamedata.Word.Value, guess);
        var guessDto = new GuessesDto
        {
          Guess = guess,
          LetterStates = new List<LetterState>()
        };
        guessDto.LetterStates.AddRange(letterStates);

        guessesDto = guessesDto.Append(guessDto).ToArray();
      }

      var getGameResponseDto = new GetGameResponseDto
      {
        GameId = gamedata.GameId,
        TurnsPlayed = await GetPlayedTurn(gamedata.GameId),
        GuessLength = gamedata.Word.Length,
        MaxTurns = gamedata.MaxTurns,
        Guesses = new List<GuessesDto>()
      };
      getGameResponseDto.Guesses.AddRange(guessesDto);

      return getGameResponseDto;
    }

    private async Task<int> GetPlayedTurn(int gameId)
    {
      var turnCount = await _gameContext.GuessLogs
          .CountAsync(g => g.GameId == gameId && g.Turn > 0);

      return turnCount;
    }

    private async Task<bool> IsGuessValid(int gameWordLength, string guess)
    {
      if (guess.Length != gameWordLength)
        throw new ArgumentException("Guess word length does not match the game word length.");

      var word = await _gameContext.ValidWords
          .FirstOrDefaultAsync(w => w.Word.Value == guess);

      // if (guess == null)
      //     return false;

      return word != null;
    }

    private async Task<(bool IsCorrect, LetterState[] LetterResults)> IsGuessCorrect(int gameId, Guid playerId, string guess)
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

    private async Task<LetterState[]> GetLetterStates(string gameWord, string guessWord)
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

    private async Task UpdateStatus(Status status, GameData game, int currentTurn)
    {
      game.GameStatus = status;
      game.EndTime = DateTime.UtcNow;
      game.TurnsTaken = currentTurn;
      _gameContext.GamesData.Update(game);
      await _gameContext.SaveChangesAsync();
    }

    public async Task<HomeStatsResponseDto> GetHomeStats(Guid playerId)
    {
      var stats = await _gameContext.GamesData
        .AsSplitQuery()
        .AsNoTracking()
        .Where(gd => gd.PlayerId == playerId)
        .GroupBy(gd => gd.PlayerId)
        .Select(group => new
        {
          HasExistingGame = group.Any(gd => gd.GameStatus == Status.InProgress),
          GamesPlayed = group.Count(gd => gd.GameStatus == Status.Won || gd.GameStatus == Status.Lost),
          GamesWon = group.Count(gd => gd.GameStatus == Status.Won),
          History = group.OrderByDescending(gd => gd.StartTime).Select(gd => new GetHistoryDto
          {
            GameId = gd.GameId,
            Date = gd.StartTime,
            Word = gd.GameStatus == Status.InProgress ? "???" : gd.Word.Value,
            Result = gd.GameStatus,
            TurnsSolved = _gameContext.GuessLogs.Where(gl => gl.GameId == gd.GameId && gl.Turn > 0).Count(),
            MaxTurns = gd.MaxTurns
          }).Take(10).ToList()
        })
        .FirstOrDefaultAsync();

      if (stats == null)
      {
        return new HomeStatsResponseDto();
      }

      var response = new HomeStatsResponseDto
      {
        HasExistingGame = stats.HasExistingGame,
        GamesPlayed = stats.GamesPlayed,
        WinPercentage = stats.GamesPlayed > 0 ? (double)stats.GamesWon / stats.GamesPlayed * 100 : 0,
        CurrentStreak = await GetCurrentStreak(playerId),
        HomeGameHistories = stats.History
      };

      return response;
    }

    private async Task<int> GetCurrentStreak(Guid playerId)
    {
      var lastLossTime = await _gameContext.GamesData
        .Where(g => g.PlayerId == playerId && g.GameStatus == Status.Lost)
        .OrderByDescending(g => g.StartTime)
        .Select(g => (DateTime?)g.StartTime)
        .FirstOrDefaultAsync();

      var streakQuery = _gameContext.GamesData
        .Where(g => g.PlayerId == playerId && g.GameStatus == Status.Won);

      if (lastLossTime != null)
      {
        streakQuery = streakQuery.Where(g => g.StartTime > lastLossTime);
      }

      var streakCount = await streakQuery.CountAsync();

      return streakCount;
    }

    public async Task<GetStatsResponseDto> GetFullStats(Guid playerId)
    {
      var playerGames = await _gameContext.GamesData
        .AsNoTracking()
        .Where(gd => gd.PlayerId == playerId)
        .ToListAsync();

      var stats = playerGames
        .GroupBy(gd => gd.PlayerId)
        .Select(group => new
        {
          GamesPlayed = group.Count(gd => gd.GameStatus == Status.Won || gd.GameStatus == Status.Lost),
          GamesWon = group.Count(gd => gd.GameStatus == Status.Won),
          FastestWinByTime = group.Where(gd => gd.GameStatus == Status.Won && gd.EndTime.HasValue)
            .OrderBy(gd => gd.Duration).Select(gd => new FastestWinByTimeModel
            {
              GameId = gd.GameId,
              Duration = gd.Duration ?? TimeSpan.Zero,
              Word = gd.Word.Value
            }).FirstOrDefault(),
          FastestWinByTurns = group.Where(gd => gd.GameStatus == Status.Won && gd.TurnsTaken.HasValue)
            .OrderBy(gd => (gd.TurnsTaken ?? 0)).Select(gd => new FastestWinByTurnsModel
            {
              GameId = gd.GameId,
              TurnsTaken = gd.TurnsTaken ?? 0,
              Word = gd.Word.Value
            }).FirstOrDefault(),
        })
        .FirstOrDefault();

      if (stats == null || playerGames == null)
      {
        return new GetStatsResponseDto();
      }

      var response = new GetStatsResponseDto
      {
        GamesPlayed = stats.GamesPlayed,
        WinPercentage = stats.GamesPlayed > 0 ? (double)stats.GamesWon / stats.GamesPlayed * 100 : 0,
        GamesWon = stats.GamesWon,
        GamesLost = stats.GamesPlayed - stats.GamesWon,
        AverageTurnsToWin = await GetAverageTurnsToWin(playerGames),
        CurrentStreak = await GetCurrentStreak(playerId),
        LongestStreak = await GetLongestStreak(playerGames),
        FastestWinByTime = stats.FastestWinByTime,
        FastestWinByTurns = stats.FastestWinByTurns,
        WordLengthDistribution = await GetWordLengthDistribution(playerGames),
        TurnDistribution = await GetTurnDistribution(playerGames),
        UsedWordDistribution = await GetUsedWordDistribution(playerId),
        WinsByTurnDistribution = await GetWinsByTurnDistribution(playerGames)
      };

      return response;
    }

    private async Task<int> GetLongestStreak(List<GameData> gameData)
    {
      //var gameHistory = await _gameContext.GamesData
      //  .Where(g => g.PlayerId == playerId)
      //  .OrderBy(g => g.StartTime)
      //  .Select(g => g.GameStatus == Status.Won)
      //  .ToListAsync();

      var sortedData = gameData
        .OrderBy(g => g.StartTime)
        .Select(g => g.GameStatus == Status.Won)
        .ToList();

      int longestStreak = 0;
      int counter = 0;

      foreach (bool isWin in sortedData)
      {
        if (isWin)
        {
          counter++;
          longestStreak = Math.Max(longestStreak, counter);
        }
        else
        {
          counter = 0;
        }
      }

      return longestStreak;
    }

    private async Task<List<WordLengthDistributionModel>> GetWordLengthDistribution(List<GameData> gameData)
    {
      var distributedData = new List<WordLengthDistributionModel>();
      var processedList = gameData
        .GroupBy(gd => gd.Word.Length)
        .Select(group => new
        {
          WordLength = group.Key,
          Count = group.Count()
        })
        .ToList();

      foreach (var item in processedList)
      {
        distributedData.Add(new WordLengthDistributionModel { WordLength = item.WordLength, Count = item.Count });
      }

      return distributedData;

    }

    private async Task<List<TurnDistributionModel>> GetTurnDistribution(List<GameData> gameData)
    {
      var distributionData = new List<TurnDistributionModel>();
      var processedList = gameData
        .GroupBy(gd => gd.MaxTurns)
        .Select(group => new
        {
          MaxTurns = group.Key,
          Count = group.Count()
        })
        .ToList();

      foreach (var item in processedList)
      {
        distributionData.Add(new TurnDistributionModel { Turn = item.MaxTurns, Count = item.Count });
      }

      return distributionData;
    }

    private async Task<List<UsedWordDistributionModel>> GetUsedWordDistribution(Guid playerId)
    {
      var distributionData = new List<UsedWordDistributionModel>();

      var processedList = await _gameContext.GamesData
        .AsNoTracking()
        .Where(gd => gd.PlayerId == playerId)
        .SelectMany(gd => gd.GuessLogs)
        .Where(gl => gl.Turn > 0)
        .GroupBy(gl => gl.Guess)
        .Select(group => new
        {
          Word = group.Key,
          Count = group.Count()
        })
        .OrderByDescending(group => group.Count)
        .Take(10)
        .ToListAsync();

      foreach (var item in processedList)
      {
        distributionData.Add(new UsedWordDistributionModel { Word = item.Word, Count = item.Count });
      }
      return distributionData;
    }

    private async Task<List<WinsByTurnDistributionModel>> GetWinsByTurnDistribution(List<GameData> gameData)
    {
      var distributionData = new List<WinsByTurnDistributionModel>();

      var processedList = gameData
        .GroupBy(gd => gd.Word.Length)
        .Select(group => new
        {
          WordLength = group.Key,
          TurnsToWin = group.GroupBy(subGroup => subGroup.TurnsTaken)
          .Select(subGroup => new
          {
            Turns = subGroup.Key ?? 0,
            Win = subGroup.Count(subGroup => subGroup.GameStatus == Status.Won)
          })
        })
        .ToList();

      foreach (var item in processedList)
      {
        var turnsToWinData = new List<TurnsToWinModel>();
        foreach (var turnsToWin in item.TurnsToWin)
        {
          if (turnsToWin.Win > 0)
            turnsToWinData.Add(new TurnsToWinModel { Turns = turnsToWin.Turns, WinCount = turnsToWin.Win });
        }

        if (turnsToWinData.Count > 0)
          distributionData.Add(new WinsByTurnDistributionModel { WordLength = item.WordLength, TurnsToWinDistribution = turnsToWinData });
      }

      return distributionData;
    }

    private async Task<double> GetAverageTurnsToWin(List<GameData> gameData)
    {
      var totalTurns = gameData.Where(gd => gd.GameStatus == Status.Won).Sum(gd => gd.TurnsTaken) ?? 0;
      var winCount = gameData.Count(gd => gd.GameStatus == Status.Won);
      return winCount > 0 ? (double)totalTurns / winCount : 0;
    }

    public async Task<List<GetHistoryDto>> GetFullHistory(Guid playerId, int pageNumber, int pageSize)
    {
      var response = new List<GetHistoryDto>();
      var history = await _gameContext.GamesData
        .AsSplitQuery()
        .AsNoTracking()
        .Where(gd => gd.PlayerId == playerId)
        .OrderByDescending(gd => gd.StartTime)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        //.GroupBy(gd => gd.PlayerId)
        .Select(group => new GetHistoryDto
        {
          GameId = group.GameId,
          Date = group.StartTime,
          Word = group.GameStatus == Status.InProgress ? "???" : group.Word.Value,
          Result = group.GameStatus,
          TurnsSolved = _gameContext.GuessLogs.Where(gl => gl.GameId == group.GameId && gl.Turn > 0).Count(),
          MaxTurns = group.MaxTurns
        }).ToListAsync();

      foreach (var item in history)
      {
        response.Add(item);
      }

      return response;
    }
  }
}


