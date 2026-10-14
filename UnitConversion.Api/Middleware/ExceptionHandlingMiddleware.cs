using System.Net;
using System.Text.Json;
using UnitConversion.Utilities.Common.Models;

namespace UnitConversion.Api.Middleware;

public partial class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            LogUnhandledException(ex);
            await WriteResponseAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
        }
    }

    private static async Task WriteResponseAsync(HttpContext context, HttpStatusCode status, string error)
    {
        context.Response.StatusCode = (int)status;
        context.Response.ContentType = "application/json";

        var response = ApiResponse.Fail<object>((int)status, error);
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Unhandled exception")]
    private partial void LogUnhandledException(Exception ex);
}
