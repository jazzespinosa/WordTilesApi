using WordTilesApi.Models.DTOs;
using WordTilesApi.Models.Entities;

namespace WordTilesApi.Services.Interfaces
{
  public interface IGameService
  {
    Task<GuessResponseDto> ProcessGuess(int gameId, Guid playerId, string guess);
    Task<NewGameResponseDto> CreateGame(Guid playerId, int wordLength, int maxTurns);
    Task<GetGameResponseDto?> GetGameByPlayerID(Guid playerId);
    Task<GetGameResponseDto> GetGame(int gameId, Guid playerId);
    Task<HomeStatsResponseDto> GetHomeStats(Guid playerId);
    Task<GetStatsResponseDto> GetFullStats(Guid playerId);
    Task<List<GetHistoryDto>> GetFullHistory(Guid playerId, int pageNumber, int pageSize);
    // Task<string> GetAnswer(int gameId, Guid playerId);
  }
}
