namespace BuildingBlock.Kernel.Errors;

public enum ErrorSeverity
{
    Info = 0,
    Warning = 1,
    Error = 2,
    Critical = 3,
}

internal sealed record ErrorCategory(
    string Type,
    int Status,
    ErrorSeverity Severity)
{
    #region 4xx — Client Errors

    public static readonly ErrorCategory BadRequest =
        new(
            "https://errors.kernel/bad-request",
            400,
            ErrorSeverity.Warning);

    public static readonly ErrorCategory Unauthorized =
        new(
            "https://errors.kernel/unauthorized",
            401,
            ErrorSeverity.Error);

    public static readonly ErrorCategory Forbidden =
        new(
            "https://errors.kernel/forbidden",
            403,
            ErrorSeverity.Error);

    public static readonly ErrorCategory NotFound =
        new(
            "https://errors.kernel/not-found",
            404,
            ErrorSeverity.Warning);

    public static readonly ErrorCategory Conflict =
        new(
            "https://errors.kernel/conflict",
            409,
            ErrorSeverity.Warning);

    public static readonly ErrorCategory UnprocessableEntity =
        new(
            "https://errors.kernel/unprocessable-entity",
            422,
            ErrorSeverity.Warning);

    #endregion

    #region 5xx — Server Errors

    public static readonly ErrorCategory InternalServerError =
        new(
            "https://errors.kernel/internal-server-error",
            500,
            ErrorSeverity.Critical);

    public static readonly ErrorCategory ServiceUnavailable =
        new(
            "https://errors.kernel/service-unavailable",
            503,
            ErrorSeverity.Error);

    #endregion
}
