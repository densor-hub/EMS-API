using WebApplication1.Middleware;
using WebApplication1.Services.TokenService;

namespace WebApplication1
{
    public static class Extensions
    {
        public static IApplicationBuilder UseRefreshTokenMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RefreshTokenMiddleware>();
        }
    }

        public static class HttpRequestExtensions
    {
        public static bool IsMobileClient(this HttpRequest request)
        {
            var ua = request.Headers.UserAgent.ToString();
            if (ua.Contains("MobileApp", StringComparison.OrdinalIgnoreCase))
                return true;

            return request.Headers.ContainsKey(CookieNames.ClientTypeHeaderName);
        }
    }
}