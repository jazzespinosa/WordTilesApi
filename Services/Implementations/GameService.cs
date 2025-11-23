using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WordledDictionaryApi.Data;
using WordledDictionaryApi.Models.Entities;

public class GameService : IGameService
{
    private readonly List<string> _validWords;
    private readonly GameContext _gameContext;

    public GameService(GameContext gameContext)
    {
        _validWords = gameContext.ValidWords
            .Select(vw => vw.Word.Value.ToLower())
            .ToList();
        _gameContext = gameContext;
    }

    public bool IsValidWord(string guess)
    {
        return _validWords.Contains(guess.ToLower());
    }

    public int GetScore(string guess)
    {
        // Simple scoring rule
        return guess.Length * 10;
    }


    public async Task<GameData?> GetGame(int gameId, Guid playerId)
    {
        var game = await _gameContext.GamesData
            .FirstOrDefaultAsync(g => g.GameId == gameId && g.PlayerId == playerId);

        return game;
    }

    public async Task<int> GetCurrentTurn(int gameId)
    {
        var turnCount = await _gameContext.GuessLogs
            .CountAsync(gl => gl.GameId == gameId);

        return turnCount + 1; // Next turn number
    }

}