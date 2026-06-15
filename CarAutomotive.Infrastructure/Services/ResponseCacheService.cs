using StackExchange.Redis;

namespace CarAutomotive.Infrastructure.Services
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public ResponseCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _database = redis.GetDatabase();
        }

        public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
        {
            if (response is null) return;

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var serializeResponse = JsonSerializer.Serialize(response, serializeOptions);
            await _database.StringSetAsync(cacheKey, serializeResponse, timeToLive);
        }

        public async Task<string?> GetCachedResponseAsync(string cacheKey)
        {
            var cachedResponse = await _database.StringGetAsync(cacheKey);

            if (cachedResponse.IsNullOrEmpty) return null;

            return cachedResponse;
        }

        public async Task RemoveCacheResponseAsync(string pattern)
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());

            var keys = server.Keys(database: _database.Database, pattern: $"{pattern}*").ToArray();

            if (!keys.Any())
                return;

            await _database.KeyDeleteAsync(keys);
        }
    }
