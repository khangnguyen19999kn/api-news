using be_tabloidnews.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.RegularExpressions;


namespace be_tabloidnews.Services
{
    public class NewsService : INewsService

    {
        private readonly IMongoCollection<News> _news;

        public NewsService(INewsDatabaseSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _news = database.GetCollection<News>(settings.NewsConnectionName);
        }

        public async Task<News> CreateNews(News news)
        {
            try
            {
                await _news.InsertOneAsync(news);
                return news;
            }
            catch (Exception ex)
            {
                // Có thể log lỗi hoặc xử lý lỗi ở đây
                throw new Exception($"Error creating news: {ex.Message}");
            }
        }

        public void DeleteNews(string id)
        {
            _news.DeleteOne(n => n.id == id);
        }

        public List<News> GetAllNews()
        {
            var allNews = _news.Find(n => true).SortByDescending(n => n.createdAt).ToList();
            return allNews;
        }
        public async Task<News> GetDetailNewsById(string id)
        {
            try
            {
                var news = await _news.Find(n => n.id == id).FirstOrDefaultAsync();
                return news;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving news details: {ex.Message}");
            }
        }

        public async Task<List<News>> GetDetailNews(string slug)
        {
            try
            {
                var newsList = await _news
                    .Find(n => n.slug == slug)
                    .SortByDescending(n => n.createdAt) // Sắp xếp theo thời gian mới nhất
                    .ToListAsync();

                foreach (var news in newsList)
                {
                    news.viewCount++;
                    await _news.ReplaceOneAsync(n => n.id == news.id, news);
                }

                return newsList;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving news details: {ex.Message}");
            }
        }


        public void UpdateNews(string id, News news)
        {
            _news.ReplaceOne(n => n.id == id, news);
        }

        public async Task<List<News>> GetNewest(int count)
        {
            try
            {
                var totalNewsCount = await _news.CountDocumentsAsync(_ => true);

                if (totalNewsCount < count)
                {
                    // Nếu tổng số bài viết ít hơn số bài viết yêu cầu,
                    // thực hiện lấy tất cả bài viết mới nhất có thể.
                    count = (int)totalNewsCount;
                }
                var newest = await _news
                    .Find(_ => true) // Lấy tất cả các bản ghi
                    .SortByDescending(n => n.createdAt) // Sắp xếp theo ngày tạo giảm dần
                    .Limit(count) // Giới hạn count bản ghi
                    .ToListAsync();

                return newest;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                throw new Exception($"Error getting latest news: {ex.Message}");
            }
        }
        public async Task<List<News>> GetBySlugTopic(string slugTopic, int count)
        {
            try
            {
                var filter = Builders<News>.Filter.Regex("slugTopic", new BsonRegularExpression(slugTopic, "i")); // "i" để không phân biệt chữ hoa, chữ thường

                var totalNewsCount = await _news.CountDocumentsAsync(filter);

                if (totalNewsCount < count)
                {
                    count = (int)totalNewsCount;
                }

                var newsBySimilarSlugTopic = await _news
                    .Find(filter)
                    .SortByDescending(n => n.createdAt)
                    .Limit(count)
                    .ToListAsync();

                return newsBySimilarSlugTopic;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting news by similar slugTopic: {ex.Message}");
            }
        }

        public async Task<List<News>> SearchByTitle(string searchTerm)
        {
            try
            {
                // Use a case-insensitive search for titles containing the specified term
                var searchFilter = Builders<News>.Filter.Regex(n => n.title, new BsonRegularExpression(new Regex(searchTerm, RegexOptions.IgnoreCase)));
                var searchResults = await _news.Find(searchFilter).ToListAsync();

                return searchResults;
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                throw new Exception($"Error searching by title: {ex.Message}");
            }
        }

        public async Task<List<News>> GetNewsByTopicPaginated(string slugTopic, int page, int pageSize)
        {
            try
            {
                var filter = Builders<News>.Filter.Regex("slugTopic", new BsonRegularExpression(slugTopic, "i"));
                var totalNewsCount = await _news.CountDocumentsAsync(filter);

                if (totalNewsCount == 0)
                {
                    return new List<News>(); // Return an empty list if no news found
                }

                var totalPages = (int)Math.Ceiling((double)totalNewsCount / pageSize);
                var skip = (page - 1) * pageSize;

                var newsByTopic = await _news
                    .Find(filter)
                    .SortByDescending(n => n.createdAt)
                    .Skip(skip)
                    .Limit(pageSize)
                    .ToListAsync();

                return newsByTopic;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting paginated news by topic: {ex.Message}");
            }
        }


    }
}
