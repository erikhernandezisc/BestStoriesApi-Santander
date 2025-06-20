using LazyCache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TestSantander.Models;

namespace TestSantander.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoriesController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAppCache _cache;

        public StoriesController(IHttpClientFactory httpClientFactory, IAppCache cache)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetTopStories([FromQuery] int count = 10)
        {
            if (count <= 0 || count > 100)
                return BadRequest("Count must be between 1 and 100.");

            var client = _httpClientFactory.CreateClient();

            var storyIds = await _cache.GetOrAddAsync("beststories", async () =>
            {
                var response = await client.GetFromJsonAsync<List<int>>("https://hacker-news.firebaseio.com/v0/beststories.json");
                return response ?? new List<int>();
            }, TimeSpan.FromMinutes(10));

            if (storyIds == null || !storyIds.Any())
                return StatusCode(503, "Unable to fetch stories from Hacker News.");

            var selectedIds = storyIds.Take(count * 2);

            var storyTasks = selectedIds.Select(id => _cache.GetOrAddAsync($"story_{id}", async () =>
            {
                return await client.GetFromJsonAsync<Story>($"https://hacker-news.firebaseio.com/v0/item/{id}.json");
            }, TimeSpan.FromMinutes(10)));

            var stories = await Task.WhenAll(storyTasks);

            var result = stories
                .Where(s => s != null)
                .OrderByDescending(s => s!.Score)
                .Take(count)
                .Select(s => new
                {
                    s!.Title,
                    Uri = s!.Url,
                    PostedBy = s!.By,
                    Time = DateTimeOffset.FromUnixTimeSeconds(s.Time).ToString("yyyy-MM-ddTHH:mm:sszzz"),
                    s!.Score,
                    CommentCount = s!.Descendants
                });

            return Ok(result);
        }

    }
}
