namespace CarAutomotive.Application.Services
{
    public interface IResponseCacheService
    {
        Task CacheResponseAsync(string cacheKey,object response,TimeSpan timeToLive); //method to set cached response by key with a specified time to live
        Task<string?> GetCachedResponseAsync(string cacheKey); //method to get cached response by key, returns null if not found
    }
}