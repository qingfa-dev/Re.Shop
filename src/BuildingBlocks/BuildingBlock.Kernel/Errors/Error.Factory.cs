namespace BuildingBlock.Kernel.Errors;

public partial record Error : IError, IEquatable<Error>
{
    #region Factory

    /// <summary>Creates a custom <see cref="Error"/> with full control over all members.</summary>
    /// <param name="code">The machine-readable error code.</param>
    /// <param name="message">The human-readable error message.</param>
    /// <param name="status">Optional HTTP status code.</param>
    /// <param name="type">Optional error type category.</param>
    /// <param name="instance">Optional unique instance identifier.</param>
    /// <param name="severity">Optional severity; defaults to <see cref="ErrorSeverity.Error"/>.</param>
    /// <returns>A new <see cref="Error"/> instance.</returns>
    public static Error Custom(
        string code,
        string message,
        int? status = null,
        string? type = null,
        string? instance = null,
        ErrorSeverity severity = ErrorSeverity.Error)
        => new(
            code: code,
            message: message,
            status: status,
            type: type,
            instance: instance,
            severity: severity);

    /// <summary>Creates an <see cref="Error"/> from a <see cref="ErrorCategory"/>.</summary>
    /// <param name="category">The predefined error category.</param>
    /// <param name="code">The machine-readable error code.</param>
    /// <param name="message">The human-readable error message.</param>
    /// <returns>A new <see cref="Error"/> with the category defaults applied.</returns>
    private static Error Create(
        ErrorCategory category,
        string code,
        string message)
        => new(
            code: code,
            message: message,
            status: category.Status,
            type: category.Type,
            severity: category.Severity);

    #endregion

    #region 4xx — Client Errors

    /// <summary>Creates a 400 BadRequest error.</summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A new <see cref="Error"/> with status 400.</returns>
    public static Error BadRequest(string code, string message) => Create(ErrorCategory.BadRequest, code, message);

    /// <summary>Creates a 401 Unauthorized error.</summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A new <see cref="Error"/> with status 401.</returns>
    public static Error Unauthorized(string code, string message) => Create(ErrorCategory.Unauthorized, code, message);

    /// <summary>Creates a 403 Forbidden error.</summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A new <see cref="Error"/> with status 403.</returns>
    public static Error Forbidden(string code, string message) => Create(ErrorCategory.Forbidden, code, message);

    /// <summary>Creates a 404 NotFound error.</summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A new <see cref="Error"/> with status 404.</returns>
    public static Error NotFound(string code, string message) => Create(ErrorCategory.NotFound, code, message);

    /// <summary>Creates a 409 Conflict error.</summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A new <see cref="Error"/> with status 409.</returns>
    public static Error Conflict(string code, string message) => Create(ErrorCategory.Conflict, code, message);

    /// <summary>Creates a 422 UnprocessableEntity error.</summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A new <see cref="Error"/> with status 422.</returns>
    public static Error UnprocessableEntity(string code, string message) => Create(ErrorCategory.UnprocessableEntity, code, message);

    #endregion

    #region 5xx — Server Errors

    /// <summary>Creates a 500 InternalServerError error.</summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A new <see cref="Error"/> with status 500.</returns>
    public static Error InternalServerError(string code, string message) => Create(ErrorCategory.InternalServerError, code, message);

    /// <summary>Creates a 503 ServiceUnavailable error.</summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A new <see cref="Error"/> with status 503.</returns>
    public static Error ServiceUnavailable(string code, string message) => Create(ErrorCategory.ServiceUnavailable, code, message);

    #endregion
}