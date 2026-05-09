using Microsoft.AspNetCore.Diagnostics;
using MinimalApiApplication.Dtos;
using MinimalApiApplication.Models;

namespace MinimalApiApplication.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception occurred while processing the request.");
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        
        var response = new ApiResponse<string>("An unexpected error occurred. Please try again later.");
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }
}
