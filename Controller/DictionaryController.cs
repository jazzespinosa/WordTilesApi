using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WordledDictionaryApi.Data;
using WordledDictionaryApi.Models;

namespace WordledDictionaryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DictionaryController : ControllerBase
    {
        private readonly DictionaryContext _context;

        public DictionaryController(DictionaryContext context)
        {
            _context = context;
        }

        // GET: api/dictionary/word/cat
        [HttpGet("word/{word}")]
        public async Task<IActionResult> GetWord(string word)
        {
            var entry = await _context.Entries
                .FirstOrDefaultAsync(e => e.Word.ToLower() == word.ToLower());

            if (entry == null)
                return NotFound($"The word '{word}' was not found.");

            return Ok(entry);
        }

        // GET: api/dictionary/search?term=ca
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string term)
        {
            var results = await _context.Entries
                .Where(e => e.Word.ToLower().StartsWith(term.ToLower()))
                .ToListAsync();

            return Ok(results);
        }

        // POST: api/dictionary
        [HttpPost]
        public async Task<IActionResult> AddWord([FromBody] DictionaryEntry entry)
        {
            _context.Entries.Add(entry);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetWord), new { word = entry.Word }, entry);
        }
    }
}
