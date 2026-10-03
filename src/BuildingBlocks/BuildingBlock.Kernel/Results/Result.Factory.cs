using BuildingBlock.Kernel.Errors;

namespace BuildingBlock.Kernel.Results;

public partial record Result
{
    #region Factory

    #region Custom

    public static Result Custom(
        bool isSuccess,
        IReadOnlyList<IError>? errors = null,
        int? statusCode = null)
        => new(
            isSuccess: isSuccess,
            errors: errors ?? ResultConstant.Default.EmptyErrors,
            statusCode: statusCode
                ?? (isSuccess
                    ? ResultConstant.StatusCode.Ok
                    : ResolveStatus(errors ?? ResultConstant.Default.EmptyErrors)));

    #endregion

    #region Success

    public static Result Success(
        int statusCode = ResultConstant.StatusCode.Ok)
        => Custom(
            isSuccess: true,
            statusCode: statusCode);

    public static Result Ok()
        => Success(ResultConstant.StatusCode.Ok);

    public static Result Created()
        => Success(ResultConstant.StatusCode.Created);

    public static Result Accepted()
        => Success(ResultConstant.StatusCode.Accepted);

    public static Result NoContent()
        => Success(ResultConstant.StatusCode.NoContent);

    #endregion

    #region Failure

    public static Result Failure(
        IEnumerable<IError> errors)
        => Fail(errors);

    public static Result Failure(
        IEnumerable<IError> errors,
        int statusCode)
        => Fail(errors, statusCode);

    public static Result Fail(params IError[] errors)
        => Fail((IEnumerable<IError>)errors);

    public static Result Fail(IEnumerable<IError> errors)
        => CreateFailure(errors, statusCode: null);

    public static Result Fail(IEnumerable<IError> errors, int statusCode)
        => CreateFailure(errors, statusCode);

    private static Result CreateFailure(IEnumerable<IError> errors, int? statusCode)
    {
        ArgumentNullException.ThrowIfNull(errors);

        var errorList = errors as IReadOnlyList<IError> ?? errors.ToArray();
        var resolvedStatusCode = statusCode ?? ResolveStatus(errorList);

        return new Result(
            isSuccess: false,
            errors: errorList,
            statusCode: resolvedStatusCode);
    }

    #endregion

    #region Status Resolution

    internal static int ResolveStatus(IReadOnlyList<IError> errors)
    {
        IError? highestSeverityError = null;
        foreach (var error in errors)
        {
            if (highestSeverityError is null || error.Severity > highestSeverityError.Severity)
                highestSeverityError = error;
        }

        if (highestSeverityError?.Status is int status)
            return status;

        foreach (var error in errors)
        {
            if (error.Status is int fallbackStatus)
                return fallbackStatus;
        }

        return ResultConstant.StatusCode.InternalServerError;
    }

    #endregion

    #endregion
}