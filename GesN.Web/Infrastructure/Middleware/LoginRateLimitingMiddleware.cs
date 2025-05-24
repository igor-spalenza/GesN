using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace GesN.Web.Infrastructure.Middleware
{
    public class LoginRateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly int _maxAttempts = 5;
        private readonly TimeSpan _lockoutTime = TimeSpan.FromMinutes(15);

        public LoginRateLimitingMiddleware(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/Identity/Account/Login") && 
                HttpMethods.IsPost(context.Request.Method))
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var cacheKey = $"login_attempts_{ipAddress}";

                var attempts = _cache.GetOrCreate(cacheKey, entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = _lockoutTime;
                    return new LoginAttemptInfo { Count = 0, LastAttempt = DateTime.UtcNow };
                });

                if (attempts.Count >= _maxAttempts)
                {
                    var timeSinceLastAttempt = DateTime.UtcNow - attempts.LastAttempt;
                    if (timeSinceLastAttempt < _lockoutTime)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            error = "Too many login attempts",
                            retryAfter = (_lockoutTime - timeSinceLastAttempt).TotalSeconds
                        });
                        return;
                    }
                    else
                    {
                        _cache.Remove(cacheKey);
                    }
                }

                attempts.Count++;
                attempts.LastAttempt = DateTime.UtcNow;
                _cache.Set(cacheKey, attempts, _lockoutTime);
            }

            await _next(context);
        }

        private class LoginAttemptInfo
        {
            public int Count { get; set; }
            public DateTime LastAttempt { get; set; }
        }
    }

    public static class LoginRateLimitingMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoginRateLimiting(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoginRateLimitingMiddleware>();
        }
    }
} 