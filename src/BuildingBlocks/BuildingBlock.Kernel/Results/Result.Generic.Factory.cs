using BuildingBlock.Kernel.Errors;

namespace BuildingBlock.Kernel.Results;

public partial record Result<TValue>
{
    #region Factory

    #region Success

    public static Result<TValue> Ok(
        TValue value,
        int statusCode = ResultConstant.StatusCode.Ok)
        => new(
            isSuccess: true,
            value: value,
            statusCode: statusCode);

    public static Result<TValue> Success(
        TValue value,
        int statusCode = ResultConstant.StatusCode.Ok)
        => Ok(value, statusCode);

    public static Result<TValue> Created(TValue value)
        => Ok(value, ResultConstant.StatusCode.Created);

    public static Result<TValue> Accepted(TValue value)
        => Ok(value, ResultConstant.StatusCode.Accepted);
    
    #endregion

    #region Failure

    public static new Result<TValue> Failure(IEnumerable<IError> errors)
        => Fail(errors);

    public static new Result<TValue> Failure(IEnumerable<IError> errors, int statusCode)
        => Fail(errors, statusCode);

    public static new Result<TValue> Fail(params IError[] errors)
        => Fail((IEnumerable<IError>)errors);

    public static new Result<TValue> Fail(IEnumerable<IError> errors)
        => CreateFailure(errors, statusCode: null);

    public static new Result<TValue> Fail(IEnumerable<IError> errors, int statusCode)
        => CreateFailure(errors, statusCode);

    private static Result<TValue> CreateFailure(IEnumerable<IError> errors, int? statusCode)
    {
        ArgumentNullException.ThrowIfNull(errors);

        var errorList = errors as IReadOnlyList<IError> ?? errors.ToArray();
        var resolvedStatusCode = statusCode ?? Result.ResolveStatus(errorList);

        return new Result<TValue>(
            isSuccess: false,
            value: default!,
            errors: errorList,
            statusCode: resolvedStatusCode);
    }

    #endregion

    #endregion
}
