using System.Text.Json;
using ClaimsModule.Application.Common.Exceptions;
using ClaimsModule.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClaimsModule.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            await WriteProblemAsync(context, StatusCodes.Status422UnprocessableEntity,
                new ValidationProblemDetailsDto { Errors = new Dictionary<string, string[]>(ex.Errors) });
        }
        catch (NotFoundException ex)
        {
            await WriteProblemAsync(context, StatusCodes.Status404NotFound,
                new ProblemDetails { Title = ex.Message, Status = 404, Type = "NotFound" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            await WriteProblemAsync(context, StatusCodes.Status500InternalServerError,
                new ProblemDetails { Title = "An unexpected error occurred.", Status = 500, Type = "InternalError" });
        }
    }

    private static async Task WriteProblemAsync(HttpContext context, int status, object problem)
    {
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
