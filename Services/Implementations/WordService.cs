using Microsoft.EntityFrameworkCore;
using WordTilesApi.Data;
using WordTilesApi.Models.DTOs;
using WordTilesApi.Models.Entities;
using WordTilesApi.Services.Interfaces;

namespace WordTilesApi.Services.Implementations
{
  public class WordService : IWordService
  {
    private readonly GameContext _gameContext;

    public WordService(GameContext gameContext)
    {
      _gameContext = gameContext;
    }

    public async Task<ValidWord?> GetWord(string word)
    {
      return await _gameContext.ValidWords
          .FirstOrDefaultAsync(w => w.Word.Value.ToUpper() == word.ToUpper());
    }

    public async Task<int> AddWords(List<WordDto> entries)
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
