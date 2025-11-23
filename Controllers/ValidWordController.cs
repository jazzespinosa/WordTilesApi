using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WordledDictionaryApi.Data;
using WordledDictionaryApi.Models.DTOs;
using WordledDictionaryApi.Models.Entities;

namespace WordledDictionaryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValidWordController : ControllerBase
    {
        private readonly GameContext _context;

        public ValidWordController(GameContext context)
        {
            _context = context;
        }

        // GET: api/dictionary/word/cat
        [HttpGet("word/{word}")]
        public async Task<IActionResult> GetWord(string word)
        {
            var entry = await _context.ValidWords
                .FirstOrDefaultAsync(w => w.Word.Value.ToLower() == word.ToLower());

            if (entry == null)
                return NotFound($"The word '{word}' was not found.");

            return Ok(entry);
        }

        // GET: api/dictionary/length/3
        // [HttpGet("length/{wordLength}")]
        // public async Task<IActionResult> GetRandomWordByLength(int wordLength)
        // {
        //     var words = await _context.Entries
        //         .Where(e => e.Word.Length == wordLength)
        //         .ToListAsync();

        //     if (words.Count == 0)
        //         return NotFound($"No words of length {wordLength} found.");

        //     var randomIndex = new Random().Next(words.Count);
        //     var randomWord = words[randomIndex];
        //     var output = new
        //     {
        //         Word = randomWord.Word.Value,
        //         Length = randomWord.Word.Length
        //     };

        //     return Ok(output);
        // }

        // // GET: api/dictionary/search?term=ca
        // [HttpGet("search")]
        // public async Task<IActionResult> Search([FromQuery] string term)
        // {
        //     var results = await _context.Entries
        //         .Where(e => e.Word.Value.ToLower().StartsWith(term.ToLower()))
        //         .ToListAsync();

        //     return Ok(results);
        // }

        // POST: api/dictionary
        [HttpPost]
        public async Task<IActionResult> AddWord([FromBody] WordDto[] entries)
        {
            if (entries == null || entries.Length == 0)
            {
                return BadRequest("No words to add.");
            }
            else
            {
                foreach (var item in entries)
                {
                    var entry = new ValidWord
                    {
                        Id = 0,
                        Word = new WordData(item.Word)
                    };

                    _context.ValidWords.Add(entry);
                }
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetWord), new { word = entries }, entries);
            }
        }
    }
}
