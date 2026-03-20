namespace Shared.Errors;

public enum ErrorType
{
    /// <summary>
    /// The request was invalid or cannot be served. The exact error should be explained in the error payload.
    /// </summary>
    BAD_REQUEST,

    /// <summary>
    /// The requested resource was not found.
    /// </summary>
    NOT_FOUND,

    /// <summary>
    /// The request could not be completed due to a conflict with the current state of the target resource
    /// </summary>
    CONFLICT,

    /// <summary>
    /// The server encountered an unexpected condition that prevented it from fulfilling the request.
    /// </summary>
    INTERNAL_SERVER_ERROR,
}