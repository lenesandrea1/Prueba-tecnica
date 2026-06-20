using EventosVivos.Application.Common;
using EventosVivos.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

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

                var (statusCode, title) = exception switch
                {
                    InvalidEventScheduleException => (StatusCodes.Status400BadRequest, "Invalid event schedule."),
                    NotFoundException => (StatusCodes.Status404NotFound, "Resource not found."),
                    ConflictException => (StatusCodes.Status409Conflict, "Business rule conflict."),
                    _ => (StatusCodes.Status500InternalServerError, "Unexpected server error.")
                };

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/problem+json";

                var problem = new ProblemDetails
                {
                    Status = statusCode,
                    Title = title,
                    Detail = exception.Message,
                    Instance = context.Request.Path
                };

                await context.Response.WriteAsJsonAsync(problem);
            });
        });
    }
}
