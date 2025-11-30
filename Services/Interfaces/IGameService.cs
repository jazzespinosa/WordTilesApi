using WordledDictionaryApi.Models.DTOs;

public interface IGameService
{
    Task<GuessResponseDto> ProcessGuess(int gameId, Guid playerId, string guess);
    Task<NewGameResponseDto> CreateGame(Guid playerId, int wordLength, int maxTurns);
}