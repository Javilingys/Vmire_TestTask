using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Parser.DTOs;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Parser.Database;
using Parser.Models;

namespace Parser.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ParserController : ControllerBase
    {

        private readonly ILogger<ParserController> _logger;
        private readonly AppDbContext _dbContext;

        public ParserController(
            ILogger<ParserController> logger,
            AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetWordCount([FromQuery] string word)
        {
            var result = await _dbContext.NumberCounts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name == word.ToLower());

            if (result == null)
            {
                _logger.LogWarning("Слово {word} не нашлось.", word);
                return NotFound();
            }

            return Ok(result.Count);
        }
        
        [HttpPost]
        public async Task<IActionResult> SearchWordInUrls([FromBody] ParseUrlsDto request)
        {
            var tasks = request.Urls
                .Select(url => CountWordInUrl(url, request.SearchWord));
            var count = (await Task.WhenAll(tasks)).Sum();

            var numberCountFromDb = await _dbContext.NumberCounts
                .FirstOrDefaultAsync(x => x.Name == request.SearchWord.ToLower());

            if (numberCountFromDb == null)
            {
                _dbContext.NumberCounts.Add(new NumberCount
                {
                    Name = request.SearchWord.ToLower(),
                    Count = count
                });
            }
            else
            {
                numberCountFromDb.Count = count;
            }

            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        
        private async Task<int> CountWordInUrl(string url, string word)
        {
            HtmlWeb web = new HtmlWeb();
            var htmlDocument = await web.LoadFromWebAsync(url);

            var textNodes = htmlDocument.DocumentNode.DescendantsAndSelf()
                .Where(n => !n.HasChildNodes && !string.IsNullOrWhiteSpace(n.InnerHtml));

            var count = textNodes.Sum(node => CountOccurrences(node.InnerHtml, word));

            return count;
        }

        private int CountOccurrences(string text, string word)
        {
            var occurrences = 0;
            var index = text.IndexOf(word, StringComparison.OrdinalIgnoreCase);

            while (index != -1)
            {
                occurrences++;
                index = text.IndexOf(word, index + word.Length, StringComparison.OrdinalIgnoreCase);
            }

            return occurrences;
        }
    }
}