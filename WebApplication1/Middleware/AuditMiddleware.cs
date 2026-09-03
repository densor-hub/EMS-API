using System.Text;
using System.Text.Json;
using WebApplication1.Services.ControllerServices;

namespace WebApplication1.Middleware
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditMiddleware> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        // Paths to exclude from auditing
        private readonly HashSet<string> _excludedPaths = new()
        {
            "/api/health",
            "/api/status",
            "/api/ping",
            "/swagger",
            "/health"
        };

        // HTTP methods to exclude (optional)
        private readonly HashSet<string> _excludedMethods = new()
        {
            "OPTIONS",
            "HEAD"
        };

        public AuditMiddleware(
            RequestDelegate next,
            ILogger<AuditMiddleware> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip audit for excluded paths and methods
            if (ShouldSkipAudit(context))
            {
                await _next(context);
                return;
            }

            using var scope = _serviceScopeFactory.CreateScope();
            var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();

            // Capture request details
            var startTime = DateTime.UtcNow;
            var requestBody = await ReadRequestBodyAsync(context.Request);
            var requestHeaders = GetRequestHeaders(context.Request);

            // Store original response body stream
            var originalResponseBodyStream = context.Response.Body;
            using var responseBodyMemoryStream = new MemoryStream();
            context.Response.Body = responseBodyMemoryStream;

            try
            {
                // Continue processing the request
                await _next(context);

                // Read the response body
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                // Copy response body to original stream
                await responseBodyMemoryStream.CopyToAsync(originalResponseBodyStream);

                // Log the API call (asynchronously to not block response)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        using var innerScope = _serviceScopeFactory.CreateScope();
                        var innerAuditService = innerScope.ServiceProvider.GetRequiredService<IAuditService>();

                        await innerAuditService.LogCustomEventAsync(
                            "API",
                            context.Request.Path,
                            context.Request.Method,
                            $"API call {context.Response.StatusCode}",
                            new
                            {
                                Path = context.Request.Path,
                                Method = context.Request.Method,
                                Query = context.Request.QueryString.ToString(),
                                RequestBody = requestBody,
                                RequestHeaders = requestHeaders,
                                ResponseStatusCode = context.Response.StatusCode,
                                ResponseBody = ShouldCaptureResponseBody(context) ? responseBody : null,
                                Duration = DateTime.UtcNow - startTime,
                                User = context.User?.Identity?.Name ?? "Anonymous",
                                UserAgent = context.Request.Headers["User-Agent"].ToString(),
                                IpAddress = context.Connection.RemoteIpAddress?.ToString()
                            }
                        );
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to log API audit for {Path}", context.Request.Path);
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in audit middleware for {Path}", context.Request.Path);

                // Log the error
                try
                {
                    await auditService.LogCustomEventAsync(
                        "API_ERROR",
                        context.Request.Path,
                        context.Request.Method,
                        $"API call failed: {ex.Message}",
                        new
                        {
                            Path = context.Request.Path,
                            Method = context.Request.Method,
                            Error = ex.Message,
                            StackTrace = ex.StackTrace
                        }
                    );
                }
                catch (Exception auditEx)
                {
                    _logger.LogError(auditEx, "Failed to log API error audit");
                }

                throw;
            }
            finally
            {
                // Restore original response body stream
                context.Response.Body = originalResponseBodyStream;
            }
        }

        private bool ShouldSkipAudit(HttpContext context)
        {
            var path = context.Request.Path.ToString().ToLower();
            var method = context.Request.Method.ToUpper();

            // Skip excluded paths
            foreach (var excludedPath in _excludedPaths)
            {
                if (path.StartsWith(excludedPath.ToLower()))
                    return true;
            }

            // Skip excluded methods
            if (_excludedMethods.Contains(method))
                return true;

            // Skip if not API
            if (!path.StartsWith("/api"))
                return true;

            return false;
        }

        private bool ShouldCaptureResponseBody(HttpContext context)
        {
            // Only capture response bodies for specific content types
            var contentType = context.Response.ContentType?.ToLower() ?? "";

            // Skip binary content
            if (contentType.Contains("image") ||
                contentType.Contains("video") ||
                contentType.Contains("audio") ||
                contentType.Contains("octet-stream"))
                return false;

            // Only capture JSON responses
            return contentType.Contains("json") ||
                   contentType.Contains("text") ||
                   string.IsNullOrEmpty(contentType);
        }

        private async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            try
            {
                // Skip reading for GET, DELETE, etc.
                if (request.Method == "GET" ||
                    request.Method == "DELETE" ||
                    request.Method == "HEAD" ||
                    request.Method == "OPTIONS")
                {
                    return null;
                }

                // Check content type
                var contentType = request.ContentType?.ToLower() ?? "";
                if (!contentType.Contains("json") && !contentType.Contains("text"))
                {
                    return "[Binary/Non-text content]";
                }

                // Enable buffering to allow multiple reads
                request.EnableBuffering();

                using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();

                // Reset position for further processing
                request.Body.Position = 0;

                // Truncate large bodies
                if (body.Length > 10000)
                {
                    return body.Substring(0, 10000) + "... [truncated]";
                }

                return body;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error reading request body");
                return "[Error reading body]";
            }
        }

        private Dictionary<string, string> GetRequestHeaders(HttpRequest request)
        {
            var headers = new Dictionary<string, string>();

            // Important headers to capture
            var headersToCapture = new[]
            {
                "User-Agent",
                "Referer",
                "Accept",
                "Accept-Language",
                "Authorization",
                "Content-Type",
                "Content-Length",
                "X-Request-ID",
                "X-Forwarded-For",
                "X-Real-IP"
            };

            foreach (var headerName in headersToCapture)
            {
                if (request.Headers.TryGetValue(headerName, out var value))
                {
                    // Mask Authorization header
                    if (headerName == "Authorization" && !string.IsNullOrEmpty(value))
                    {
                        headers[headerName] = "*** REDACTED ***";
                    }
                    else
                    {
                        headers[headerName] = value.ToString();
                    }
                }
            }

            return headers;
        }
    }
}