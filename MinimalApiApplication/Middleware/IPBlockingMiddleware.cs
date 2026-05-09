using MinimalApiApplication.Dtos;

namespace MinimalApiApplication.Middleware;

public class IPBlockingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IPBlockingMiddleware> _logger;
    private readonly List<string> _blockedIPs = new List<string>
    {
       "127.0.0.1",
    };
    public IPBlockingMiddleware(RequestDelegate next, ILogger<IPBlockingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        var remoteIp = context.Connection.RemoteIpAddress?.ToString();
        _logger.LogInformation("Request from Remote IP address: {RemoteIp}", remoteIp);
        if (remoteIp != null && _blockedIPs.Contains(remoteIp))
        {
            _logger.LogWarning("Blocked request from IP: {RemoteIp}", remoteIp);
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new ApiResponse<string>($"Forbidden: Your IP address ({remoteIp}) is blocked."));
            return;
        }
        await _next(context);
    }

}
