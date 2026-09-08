using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace MiApi.Api.Middlewares;

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
            _logger.LogError(ex, "Excepción no controlada durante la petición: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var problemDetails = exception switch
        {
            // Errores de validación de FluentValidation (400 Bad Request)
            ValidationException validationException => new ProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Error de validación",
                Detail = "Uno o más errores de validación han ocurrido.",
                Extensions =
                {
                    ["errors"] = validationException.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        )
                }
            },

            // Registro duplicado o violación de FK en la Base de Datos (409 Conflict)
            Microsoft.EntityFrameworkCore.DbUpdateException dbUpdateEx => new ProblemDetails
            {
                Status = (int)HttpStatusCode.Conflict,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                Title = "Conflicto en la base de datos",
                Detail = dbUpdateEx.InnerException?.Message ?? dbUpdateEx.Message
            },

            // Registro no encontrado (404 Not Found)
            KeyNotFoundException => new ProblemDetails
            {
                Status = (int)HttpStatusCode.NotFound,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Recurso no encontrado",
                Detail = exception.Message
            },

            // Cualquier otro error interno no controlado (500 Internal Server Error)
            _ => new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "Error interno del servidor",
                Detail = exception.Message
            }
        };

        context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

        var jsonOptions = new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            };

        return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, jsonOptions));
    }
      
    
}