using Microsoft.AspNetCore.Mvc.Filters;

namespace CarAutomotive.API.Helpers
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveInSeconds;
        public CachedAttribute(int timeToLiveInSeconds)
        {
            _timeToLiveInSeconds = timeToLiveInSeconds;
        }

        public async Task OnActionExecutionAsync(
    ActionExecutingContext context,
    ActionExecutionDelegate next)
        {
            if (context.HttpContext.User.Identity?.IsAuthenticated == true)
            {
                await next();
                return;
            }

            var responseCacheService =
                context.HttpContext.RequestServices
                .GetRequiredService<IResponseCacheService>();


            var cacheKey =
                GenerateCacheKeyFromRequest(
                    context.HttpContext.Request);


            var response =
                await responseCacheService
                .GetCachedResponseAsync(cacheKey);


            if (!string.IsNullOrEmpty(response))
            {
                context.Result = new ContentResult
                {
                    Content = response,
                    ContentType = "application/json",
                    StatusCode = 200
                };

                return;
            }


            var executedActionContext =
                await next();


            if (executedActionContext.Result is OkObjectResult okObjectResult
                && okObjectResult.Value is not null)
            {
                await responseCacheService.CacheResponseAsync(
                    cacheKey,
                    okObjectResult.Value,
                    TimeSpan.FromSeconds(_timeToLiveInSeconds));
            }
        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();

            keyBuilder.Append(request.Path.ToString().ToLower());

            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
            {
                if (!string.IsNullOrWhiteSpace(value))
                    keyBuilder.Append($"|{key.ToLower()}-{value}");
            }

            return keyBuilder.ToString();
        }
    }
}
