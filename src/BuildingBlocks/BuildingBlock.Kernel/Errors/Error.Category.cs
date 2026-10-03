namespace BuildingBlock.Kernel.Errors;

internal sealed record ErrorCategory(
    string Type,
    int Status,
    ErrorSeverity Severity)
{
    #region 4xx — Client Errors

    public static readonly ErrorCategory BadRequest =
        new(
            "https://errors.kernel/bad-request",
            ErrorConstant.StatusCode.BadRequest,
            ErrorSeverity.Warning);

    public static readonly ErrorCategory Unauthorized =
        new(
            "https://errors.kernel/unauthorized",
            ErrorConstant.StatusCode.Unauthorized,
            ErrorSeverity.Error);

    public static readonly ErrorCategory Forbidden =
        new(
            "https://errors.kernel/forbidden",
            ErrorConstant.StatusCode.Forbidden,
            ErrorSeverity.Error);

    public static readonly ErrorCategory NotFound =
        new(
            "https://errors.kernel/not-found",
            ErrorConstant.StatusCode.NotFound,
            ErrorSeverity.Warning);

    public static readonly ErrorCategory Conflict =
        new(
            "https://errors.kernel/conflict",
            ErrorConstant.StatusCode.Conflict,
            ErrorSeverity.Warning);

    public static readonly ErrorCategory UnprocessableEntity =
        new(
            "https://errors.kernel/unprocessable-entity",
            ErrorConstant.StatusCode.UnprocessableEntity,
            ErrorSeverity.Warning);

    #endregion

    #region 5xx — Server Errors

    public static readonly ErrorCategory InternalServerError =
        new(
            "https://errors.kernel/internal-server-error",
            ErrorConstant.StatusCode.InternalServerError,
            ErrorSeverity.Critical);

    public static readonly ErrorCategory ServiceUnavailable =
        new(
            "https://errors.kernel/service-unavailable",
            ErrorConstant.StatusCode.ServiceUnavailable,
            ErrorSeverity.Error);

    #endregion
}