using WebApplication1.Middleware;

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
}