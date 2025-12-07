using WordledDictionaryApi.Models.DTOs;

namespace WordledDictionaryApi.Services.Interfaces
{
    public interface IGameService
    {
        Task<GuessResponseDto> ProcessGuess(int gameId, Guid playerId, string guess);
        Task<NewGameResponseDto> CreateGame(Guid playerId, int wordLength, int maxTurns);
    }
}   