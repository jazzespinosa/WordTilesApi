using WordledDictionaryApi.Models.Entities;

public interface IGameService
{
    bool IsValidWord(string guess);
    int GetScore(string guess);
    Task<GameData?> GetGame(int gameId, Guid playerId);
    Task<int> GetCurrentTurn(int gameId);
}