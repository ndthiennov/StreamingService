using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace StreamingAPI.Middlewares
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly ILogger<RateLimitMiddleware> _logger;

        // Config: each IP can make a maximum of 5 requests per 10 seconds
        private readonly int _maxRequests = 5;
        private readonly TimeSpan _timeWindow = TimeSpan.FromSeconds(10);

        public RateLimitMiddleware(RequestDelegate next, IMemoryCache cache, ILogger<RateLimitMiddleware> logger)
        {
            _next = next;
            _cache = cache;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var key = GetClientIdentifier(context);

            var requestCount = _cache.GetOrCreate(key, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _timeWindow;
                return 0;
            });

            if (requestCount >= _maxRequests)
            {
                _logger.LogWarning("Too many requests from {Client}", key);
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                await context.Response.WriteAsync("Too many requests. Please try again later.");
                return;
            }

            _cache.Set(key, requestCount + 1, _timeWindow);

            await _next(context);
        }

        private string GetClientIdentifier(HttpContext context)
        {
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            // Hoặc context.User.Identity.Name nếu cần rate-limit theo User
        }
    }
}
