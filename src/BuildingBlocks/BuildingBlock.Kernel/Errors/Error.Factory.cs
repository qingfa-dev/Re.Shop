namespace BuildingBlock.Kernel.Errors;

public partial record Error : IError, IEquatable<Error>
{
    #region Factory

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

    public static Error BadRequest(
        string code,
        string message)
        => Create(ErrorCategory.BadRequest, code, message);

    public static Error Unauthorized(
        string code,
        string message)
        => Create(ErrorCategory.Unauthorized, code, message);

    public static Error Forbidden(
        string code,
        string message)
        => Create(ErrorCategory.Forbidden, code, message);

    public static Error NotFound(
        string code,
        string message)
        => Create(ErrorCategory.NotFound, code, message);

    public static Error Conflict(
        string code,
        string message)
        => Create(ErrorCategory.Conflict, code, message);

    public static Error UnprocessableEntity(
        string code,
        string message)
        => Create(ErrorCategory.UnprocessableEntity, code, message);

    #endregion

    #region 5xx — Server Errors

    public static Error InternalServerError(
        string code,
        string message)
        => Create(ErrorCategory.InternalServerError, code, message);

    public static Error ServiceUnavailable(
        string code,
        string message)
        => Create(ErrorCategory.ServiceUnavailable, code, message);

    #endregion
}
