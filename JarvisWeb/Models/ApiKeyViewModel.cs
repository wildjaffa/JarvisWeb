using JarvisWeb.Domain.Models;

namespace JarvisWeb.Models
{
    public class ApiKeyViewModel : ApiKey
    {
        public bool IsVisible { get; set; }
        public ApiKeyViewModel() 
        { 
        }

        public ApiKeyViewModel(ApiKey apiKey)
        {
            Id = apiKey.Id;
            Name = apiKey.Name;
            Key = apiKey.Key;
            IsActive = apiKey.IsActive;
            IsVisible = false;
        }
    }
}
