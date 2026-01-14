using WordTilesApi.Models.DTOs;
using WordTilesApi.Models.Entities;

namespace WordTilesApi.Services.Interfaces
{
  public interface IWordService
  {
    Task<ValidWord?> GetWord(string word);
    Task<int> AddWords(List<WordDto> entries);
    Task<int> ImportWordsFromCsv(IFormFile file);
  }
}
