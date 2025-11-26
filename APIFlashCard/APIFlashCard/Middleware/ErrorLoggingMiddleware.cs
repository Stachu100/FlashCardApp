using APIFlashCard.Data;
using APIFlashCard.Models;

namespace APIFlashCard.Middleware
{
    public class ErrorLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorLoggingMiddleware> _logger;

        public ErrorLoggingMiddleware(RequestDelegate next, ILogger<ErrorLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, FlashCardDbContext dbContext)
        {
            try
            {
                await _next(context);

                if (context.Response.StatusCode < 400) return;

                if (context.Response.StatusCode >= 400)
                {
                    var log = new Log
                    {
                        TimeStamp = DateTimeOffset.UtcNow,
                        Level = context.Response.StatusCode >= 500 ? "Error" : "Warning",
                        Message = $"[{context.Request.Method}] {context.Request.Path} zakończyło się kodem {context.Response.StatusCode}"
                    };

                    dbContext.Logs.Add(log);
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception: {Path}", context.Request.Path);

                var log = new Log
                {
                    TimeStamp = DateTimeOffset.UtcNow,
                    Level = "Error",
                    Message = ex.Message,
                    Exception = ex.ToString()
                };

                dbContext.Logs.Add(log);
                await dbContext.SaveChangesAsync();

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { message = "Wystąpił błąd serwera." });
            }
        }
    }
}