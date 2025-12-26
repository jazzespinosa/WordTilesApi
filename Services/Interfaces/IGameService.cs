using WordTilesApi.Models.DTOs;
using WordTilesApi.Models.Entities;

namespace WordTilesApi.Services.Interfaces
{
    public interface IGameService
    {
        Task<GuessResponseDto> ProcessGuess(int gameId, Guid playerId, string guess);
        Task<NewGameResponseDto> CreateGame(Guid playerId, int wordLength, int maxTurns);
        Task<GetGameResponseDto?> GetGame(Guid playerId);
        Task<string> GetAnswer(int gameId, Guid playerId);
        Task<ValidWord?> GetWord(string word);
        Task<int> AddWords(WordDto[] entries);
        Task<int> ImportWordsFromCsv(IFormFile file);
    }
}