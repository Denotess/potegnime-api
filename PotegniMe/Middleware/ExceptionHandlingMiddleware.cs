using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Data;
using PotegniMe.DTOs.Error;
using Microsoft.EntityFrameworkCore;

namespace PotegniMe.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (status, code, clientMessage) = MapException(ex);

        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
        _logger.LogError(ex, "Unhandled exception. Path: {Path}, TraceId: {TraceId}, User: {User}",
            context.Request.Path, traceId, context.User?.Identity?.Name);

        var response = new ErrorResponseDto
        {
            ErrorCode = code,
            Message = clientMessage,
            TraceId = traceId
        };

        context.Response.StatusCode = (int)status;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static (HttpStatusCode status, int code, string clientMessage) MapException(Exception ex) =>
        ex switch
        {
            // user exceptions - shows detailed message
            ArgumentException => (HttpStatusCode.BadRequest, (int)HttpStatusCode.BadRequest, ex.Message),
            ConflictExceptionDto => (HttpStatusCode.Conflict, (int)HttpStatusCode.Conflict, ex.Message),
            NotFoundException => (HttpStatusCode.NotFound, (int)HttpStatusCode.NotFound, ex.Message),
            InvalidTokenException => (HttpStatusCode.BadRequest, (int)HttpStatusCode.BadRequest, ex.Message),
            ExpiredTokenException => (HttpStatusCode.BadRequest, (int)HttpStatusCode.BadRequest, ex.Message),
            SendGridLimitException => (HttpStatusCode.TooManyRequests, (int)HttpStatusCode.TooManyRequests, ex.Message),
            TorrentScraperException => (HttpStatusCode.ServiceUnavailable, (int)HttpStatusCode.ServiceUnavailable, ex.Message),
            
            // system exceptions - show generic message -> details logged server side
            DbUpdateException => (HttpStatusCode.InternalServerError, (int)HttpStatusCode.InternalServerError, "An error occurred while processing your request"),
            DataException => (HttpStatusCode.InternalServerError, (int)HttpStatusCode.InternalServerError, "An error occurred while processing your request"),
            InvalidOperationException when ex.Message.Contains("connection") || ex.Message.Contains("transient") => (HttpStatusCode.ServiceUnavailable, (int)HttpStatusCode.ServiceUnavailable, "Service temporarily unavailable"),
            // default
            _ => (HttpStatusCode.InternalServerError, (int)HttpStatusCode.InternalServerError, "An error occurred while processing your request")
        };
}
