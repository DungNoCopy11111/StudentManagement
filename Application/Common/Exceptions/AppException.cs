using FluentValidation.Results;

namespace StudentMgmt.Application.Common.Exceptions;

public static class ErrorCodes
{
    public const string Validation = "validation_error";
    public const string NotFound = "not_found";
    public const string BadRequest = "bad_request";
    public const string Unauthorized = "unauthorized";
    public const string Forbidden = "forbidden";
    public const string System = "system_error";
}

// Base Exception
public abstract class AppException : Exception
{
    public string ErrorCode { get; }

    protected AppException(string message, string errorCode = ErrorCodes.System, Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}

// Validation Exception
public class ValidationException : AppException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base("One or more validation failures have occurred.", ErrorCodes.Validation)
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.ToArray());
    }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation failures have occurred.", ErrorCodes.Validation)
    {
        Errors = errors;
    }
}

// Bad Request Exception
public class BadRequestException : AppException
{
    public IEnumerable<string> Errors { get; }

    public BadRequestException(string message)
        : base(message, ErrorCodes.BadRequest)
    {
        Errors = new[] { message };
    }

    public BadRequestException(IEnumerable<string> errors)
        : base("Bad request", ErrorCodes.BadRequest)
    {
        Errors = errors;
    }
}

// Not Found Exception
public class NotFoundException : AppException
{
    public NotFoundException(string message)
        : base(message, ErrorCodes.NotFound)
    {
    }

    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.", ErrorCodes.NotFound)
    {
    }
}

// Unauthorized Exception
public class UnauthorizedException : AppException
{
    public UnauthorizedException()
        : base("Unauthorized access.", ErrorCodes.Unauthorized)
    {
    }
}

// Forbidden Exception
public class ForbiddenException : AppException
{
    public ForbiddenException()
        : base("Forbidden access.", ErrorCodes.Forbidden)
    {
    }
}
