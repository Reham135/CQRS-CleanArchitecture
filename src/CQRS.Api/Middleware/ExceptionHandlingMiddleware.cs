using System.Text.Json;
using CQRS.Domain.Entities;
using FluentValidation;

namespace CQRS.Api.Middleware;

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

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse();

        switch (exception)
        {
            case DomainException domainEx:
                // Business rule violations
                response.StatusCode = StatusCodes.Status400BadRequest;
                errorResponse.Message = domainEx.Message;
                errorResponse.Type = "DomainError";
                _logger.LogWarning("Domain exception: {Message}", domainEx.Message);
                break;

            case ValidationException validationEx:
                // FluentValidation errors
                response.StatusCode = StatusCodes.Status400BadRequest;
                errorResponse.Message = "Validation failed";
                errorResponse.Type = "ValidationError";
                errorResponse.Errors = validationEx.Errors
                    .Select(e => new ValidationError
                    {
                        Property = e.PropertyName,
                        Message = e.ErrorMessage
                    }).ToList();
                _logger.LogWarning("Validation exception: {Errors}", string.Join(", ", validationEx.Errors.Select(e => e.ErrorMessage)));
                break;

            default:
                // Unexpected errors
                response.StatusCode = StatusCodes.Status500InternalServerError;
                errorResponse.Message = "An unexpected error occurred";
                errorResponse.Type = "ServerError";
                _logger.LogError(exception, "Unhandled exception");
                break;
        }

        var result = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(result);
    }
}

public class ErrorResponse
{
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<ValidationError>? Errors { get; set; }
}

public class ValidationError
{
    public string Property { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
