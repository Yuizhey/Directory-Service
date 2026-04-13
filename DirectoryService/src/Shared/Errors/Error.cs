namespace Shared.Errors;

public record class Error
{
    public string Code { get; set; }
    public string Message { get; set; }
    public ErrorType Type { get; set; }

    private Error(string code, string message, ErrorType type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    public static Error BadRequest(string message, string code = "BAD_REQUEST") => new(code, message, ErrorType.BAD_REQUEST);
    public static Error NotFound(string message, string code = "NOT_FOUND") => new(code, message, ErrorType.NOT_FOUND);
    public static Error Conflict(string message, string code = "CONFLICT") => new(code, message, ErrorType.CONFLICT);
    public static Error InternalServerError(string message, string code = "INTERNAL_SERVER_ERROR") => new(code, message, ErrorType.INTERNAL_SERVER_ERROR);
    public static Error ServiceUnavailable(string message, string code = "SERVICE_UNAVAILABLE") => new(code, message, ErrorType.SERVICE_UNAVAILABLE);
}
