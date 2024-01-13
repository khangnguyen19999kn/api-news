using be_tabloidnews.Models;

namespace be_tabloidnews.Services
{
    public interface INewsService
    {
        List<News> GetAllNews();
        Task<List<News>> GetDetailNews(string slug);
        Task<News> GetDetailNewsById(string id);
        Task<News> CreateNews(News news);
        void UpdateNews(string id, News news);
        void DeleteNews(string id);
        Task<List<News>> GetNewest(int count);
        Task<List<News>> GetBySlugTopic(string slugTopic, int count);
        Task<List<News>> SearchByTitle(string searchTerm);
        Task<List<News>> GetNewsByTopicPaginated(string slugTopic, int page, int pageSize);


    }
}
