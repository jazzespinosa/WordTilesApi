using Microsoft.AspNetCore.Mvc;
using WordTilesApi.Services.Interfaces;

namespace WordTilesApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ValidWordController : ControllerBase
  {
    private readonly IWordService _wordService;

    public ValidWordController(IWordService wordService)
    {
      _wordService = wordService;
    }

    [HttpGet("word/{word}")]
    public async Task<IActionResult> GetWord(string word)
    {
      var entry = await _wordService.GetWord(word);

      if (entry == null)
        return NotFound($"The word '{word}' was not found.");

      return Ok(entry);
    }

    //[HttpPost("words")]
    //public async Task<IActionResult> AddWords([FromBody] List<WordDto> entries)
    //{
    //  if (entries == null || entries.Count == 0)
    //  {
    //    return BadRequest("No words to add.");
    //  }

    //  var added = await _wordService.AddWords(entries);

    //  return Ok(new { Added = added });
    //}

    /// <summary>
    /// Import words from a CSV file
    /// CSV file must have the following format:
    /// Word,IsSolution
    /// where Word is the word <string> and IsSolution <bolean> is true if the word is a solution
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    //[HttpPost("import-words")]
    //[Consumes("multipart/form-data")]
    //public async Task<IActionResult> ImportWords(IFormFile file)
    //{
    //  if (file == null || file.Length == 0)
    //    return BadRequest("CSV file is required.");

    //  int added = await _wordService.ImportWordsFromCsv(file);

    //  return Ok(new { Added = added });
    //}
  }
}
