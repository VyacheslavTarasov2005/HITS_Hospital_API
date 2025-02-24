using System.Security.Authentication;
using HITS_API_1.Application.Exceptions;
using HITS_API_1.Domain.Exceptions;
using HITS_API_1.Entities;

namespace HITS_API_1.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        httpContext.Response.ContentType = "application/json";

        var response = new ErrorResponse("Internal Server Error", 500);

        switch (exception)
        {
            case FluentValidation.ValidationException ex:
                response.Type = "ValidationError";
                response.Status = 400;

                var validationErrors = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        name => name.Key,
                        message => message
                            .Select(e => e.ErrorMessage)
                            .ToList());

                response.Errors = validationErrors;
                break;

            case IncorrectFieldException ex:
                response.Type = "ValidationError";
                response.Status = 400;
                response.AddError(ex.FieldName, ex.Message);
                break;

            case NotFoundFieldException ex:
                response.Type = "NotFoundField";
                response.Status = 400;
                response.AddError(ex.FieldName, ex.Message);
                break;

            case DuplicateFieldException ex:
                response.Type = "DuplicateField";
                response.Status = 409;
                response.AddError(ex.FieldName, ex.Message);
                break;

            case ForbiddenOperationException ex:
                response.Type = "Forbidden";
                response.Status = 403;
                response.AddError("Forbidden", ex.Message);
                break;

            case BadHttpRequestException ex:
                response.Type = "BadRequest";
                response.Status = 400;
                response.AddError("BadRequest", ex.Message);
                break;

            case InvalidCredentialException:
                response.Type = "InvalidCredential";
                response.Status = 401;
                response.AddError("InvalidCredential", "Неверный логин или пароль");
                break;

            case UnauthorizedAccessException ex:
                response.Type = "Unauthorized";
                response.Status = 401;
                response.AddError("Unauthorized", ex.Message);
                break;

            case NotFoundObjectException ex:
                response.Type = "NotFound";
                response.Status = 404;
                response.AddError(ex.ObjectName, ex.Message);
                break;

            case KeyNotFoundException ex:
                response.Type = "NotFound";
                response.Status = 404;
                response.AddError("NotFound", ex.Message);
                break;
        }

        httpContext.Response.StatusCode = response.Status;

        return httpContext.Response.WriteAsJsonAsync(response);
    }
}