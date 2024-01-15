using Microsoft.AspNetCore.Mvc;

using be_tabloidnews.DTOs;
using be_tabloidnews.Models;
using be_tabloidnews.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace be_tabloidnews.Controllers
{
    [Route("api/news")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService newsService;

        public NewsController(INewsService newsService)
        {
            this.newsService = newsService ?? throw new ArgumentNullException(nameof(newsService)); ;
        }
        [HttpGet]
        public ActionResult<List<News>> GetAllNews()
        {
            try
            {
                var allNews = newsService.GetAllNews();

                if (allNews != null && allNews.Count > 0)
                {
                    // Sắp xếp tin tức theo thứ tự giảm dần của thuộc tính createdAt
                    allNews = allNews.OrderByDescending(n => n.createdAt).ToList();
                }

                return allNews;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ nội bộ: {ex.Message}");
            }
        }

        // GET: api/<NewsController>
        [HttpGet("detail/{id}")]
        public async Task<ActionResult<News>> GetDetailNewsById(string id)
        {
            try
            {
                var news = await newsService.GetDetailNewsById(id);

                if (news == null)
                {
                    return NotFound();
                }

                return Ok(news);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


        // GET api/<NewsController>/5
        [HttpGet("{slug}")]
        public async Task<ActionResult<List<News>>> GetNewsBySlug(string slug)
        {
            try
            {
                var newsList = await newsService.GetDetailNews(slug);

                if (newsList == null || newsList.Count == 0)
                {
                    return NotFound();
                }

                return Ok(newsList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        // POST api/<NewsController>
        [HttpPost]
        public async Task<ActionResult<News>> CreateNews([FromBody] NewsCreateDTO newsDTO)
        {
            try
            {
                var news = new News
                {
                    title = newsDTO.title,
                    topic = newsDTO.topic,
                    content = newsDTO.content,
                    author = newsDTO.author,
                    tags = newsDTO.tags,
                    bannerImg = newsDTO.bannerImg,
                    description = newsDTO.description,
                    createdAt = DateTime.UtcNow
                };

                news.slug = news.GenerateSlug(news.title);
                news.slugTopic = news.GenerateSlug(news.topic);
                var createdNews = await newsService.CreateNews(news);
                return Ok("News created successfully."); ;

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


        // PUT api/<NewsController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<News>> Put(string id, [FromBody] NewsCreateDTO newsUpdate)
            {
                try
                {
                var newsTask = newsService.GetDetailNewsById(id);
                if (newsTask == null)
                {
                    return NotFound();
                }

                // Await the task to get the actual News object
                var news = await newsTask;

                // Update properties
                news.title = newsUpdate.title;
                news.topic = newsUpdate.topic;
                news.content = newsUpdate.content;
                news.author = newsUpdate.author;
                news.tags = newsUpdate.tags;
                news.bannerImg = newsUpdate.bannerImg;
                news.description = newsUpdate.description;
                news.slug = news.GenerateSlug(news.title);
                news.slugTopic = news.GenerateSlug(news.topic);

                // Use UpdateNewsAsync if it's an asynchronous method
                newsService.UpdateNews(id, news);

                return Ok("News updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


        // DELETE api/<NewsController>/5
        [HttpDelete("{id}")]
        public ActionResult<News> Delete(string id)
        {
            try
            {
                var news = newsService.GetDetailNewsById(id);
                if (news == null)
                {
                    return NotFound();
                }
                newsService.DeleteNews(id);
                return Ok("News deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        // GET api/news/newest-8
        [HttpGet("newest/{numberPosts}")]
        public async Task<ActionResult<List<News>>> GetNewest(int numberPosts)
        {
            try
            {
                // Replace this logic with your actual data retrieval from the database
                // Here assuming there's a method like GetNewest in your service
                var newest = await newsService.GetNewest(numberPosts);
                if (newest == null || newest.Count == 0)
                {
                    return NotFound("No newest news found."); // Trả về 404 nếu không có dữ liệu mới nhất
                }

                return Ok(newest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("byslugtopic/{slugTopic}/{numberPosts}")]
        public async Task<ActionResult<List<News>>> GetBySlugTopic(string slugTopic, int numberPosts)
        {
            try
            {
                var newsBySlugTopic = await newsService.GetBySlugTopic(slugTopic, numberPosts);

                if (newsBySlugTopic == null || newsBySlugTopic.Count == 0)
                {
                    return NotFound($"No news found for slugTopic: {slugTopic}");
                }

                return Ok(newsBySlugTopic.OrderByDescending(n => n.createdAt)); // Sắp xếp theo createdAt giảm dần
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        // Add a new endpoint for searching by title
        [HttpGet("search")]
        public async Task<ActionResult<List<News>>> SearchByTitle([FromQuery] string searchTerm)
        {
            try
            {
                var searchResults = await newsService.SearchByTitle(searchTerm);

                if (searchResults == null || searchResults.Count == 0)
                {
                    return NotFound($"No news found for the search term: {searchTerm}");
                }
                searchResults = searchResults.OrderByDescending(n => n.createdAt).ToList();
                return Ok(searchResults);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("byslugtopic")]
        public async Task<ActionResult<List<News>>> GetNewsByTopic([FromQuery] string slugTopic, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var newsByTopic = await newsService.GetNewsByTopicPaginated(slugTopic, page, pageSize);

                if (newsByTopic == null || newsByTopic.Count == 0)
                {
                    return NotFound($"No news found for slugTopic: {slugTopic}");
                }
                newsByTopic = newsByTopic.OrderByDescending(n => n.createdAt).ToList();
                return Ok(newsByTopic);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}