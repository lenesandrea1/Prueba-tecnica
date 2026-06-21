using EventosVivos.Application.Common;
using EventosVivos.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventosVivos.Api.Infrastructure;

public static class ExceptionHandlingExtensions
{
    public static void UseGlobalExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerFeature>();
                var exception = feature?.Error;
                if (exception is null)
                {
                    return;
                }

                var (statusCode, title, detail) = exception switch
                {
                    InvalidEventScheduleException => (StatusCodes.Status400BadRequest, "Invalid event schedule.", exception.Message),
                    InvalidReservationException => (StatusCodes.Status400BadRequest, "Invalid reservation request.", exception.Message),
                    NotFoundException => (StatusCodes.Status404NotFound, "Resource not found.", exception.Message),
                    ConflictException => (StatusCodes.Status409Conflict, "Business rule conflict.", exception.Message),
                    DbUpdateException dbUpdate => (StatusCodes.Status409Conflict, "Database conflict.", dbUpdate.InnerException?.Message ?? dbUpdate.Message),
                    _ => (StatusCodes.Status500InternalServerError, "Unexpected server error.", exception.Message)
                };

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/problem+json";

                var problem = new ProblemDetails
                {
                    Status = statusCode,
                    Title = title,
                    Detail = detail,
                    Instance = context.Request.Path
                };

                await context.Response.WriteAsJsonAsync(problem);
            });
        });
    }
}
