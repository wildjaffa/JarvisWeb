using JarvisWeb.Services.Models;

namespace JarvisWeb.Services.Interfaces
{
    public interface INewsService
    {
        Task<ServiceResponseModel<IEnumerable<NewsStory>>> GetNewsAsync(DateTime startDate, DateTime endDate, int limit);
    }
}
