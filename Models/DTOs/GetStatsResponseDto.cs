namespace WordTilesApi.Models.DTOs
{
  public class GetStatsResponseDto
  {
    public int GamesPlayed { get; set; }
    public double WinPercentage { get; set; }
    public int GamesWon { get; set; }
    public int GamesLost { get; set; }
    public double AverageTurnsToWin { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public FastestWinByTimeModel FastestWinByTime { get; set; } = new FastestWinByTimeModel();
    public FastestWinByTurnsModel FastestWinByTurns { get; set; } = new FastestWinByTurnsModel();
    public List<WordLengthDistributionModel> WordLengthDistribution { get; set; } = new List<WordLengthDistributionModel>();
    public List<TurnDistributionModel> TurnDistribution { get; set; } = new List<TurnDistributionModel>();
    public List<UsedWordDistributionModel> UsedWordDistribution { get; set; } = new List<UsedWordDistributionModel>();
    public List<WinsByTurnDistributionModel> WinsByTurnDistribution { get; set; } = new List<WinsByTurnDistributionModel>();
  }

  public class FastestWinByTimeModel
  {
    public int GameId { get; set; }
    public TimeSpan Duration { get; set; }
    public string Word { get; set; } = string.Empty;
  }

  public class FastestWinByTurnsModel
  {
    public int GameId { get; set; }
    public int TurnsTaken { get; set; }
    public string Word { get; set; } = string.Empty;
  }

  public class WordLengthDistributionModel
  {
    public int WordLength { get; set; }
    public int Count { get; set; }
  }

  public class TurnDistributionModel
  {
    public int Turn { get; set; }
    public int Count { get; set; }
  }

  public class UsedWordDistributionModel
  {
    public string Word { get; set; } = string.Empty;
    public int Count { get; set; }
  }

  public class WinsByTurnDistributionModel
  {
    public int WordLength { get; set; }
    public List<TurnsToWinModel> TurnsToWinDistribution { get; set; } = new List<TurnsToWinModel>();
  }

  public class TurnsToWinModel
  {
    public int Turns { get; set; }
    public int WinCount { get; set; }
  }
}
