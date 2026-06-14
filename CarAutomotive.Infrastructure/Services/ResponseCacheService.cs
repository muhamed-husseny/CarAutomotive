using StackExchange.Redis;
namespace CarAutomotive.Infrastructure.Services
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDatabase _database;

        public ResponseCacheService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task  CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
        {
            if (response is null) return;
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var serializeResponse = JsonSerializer.Serialize(response, serializeOptions);
            await _database.StringSetAsync(cacheKey,serializeResponse, timeToLive);
        }

        public async Task<string?> GetCachedResponseAsync(string cacheKey)
        {
           var cachedResponse = await _database.StringGetAsync(cacheKey);
           if(cachedResponse.IsNullOrEmpty) return null;
           return cachedResponse;
        }

        
    }
}
