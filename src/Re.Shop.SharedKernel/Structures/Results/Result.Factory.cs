namespace SharedKernel.Structures.Results;

public partial record Result<TValue, TError>
    where TError : IError
{
    #region Create

    public static Result<TValue, TError> Create(
        TValue? value,
        bool isSuccess,
        int status,
        IReadOnlyList<TError>? errors = null)
    {
        var errorList = errors?.ToList() ?? [];
        var validationError = ResultValidator.ValidateResult(value, isSuccess, status)
            ?? ResultValidator.ValidateFailureHasErrors(isSuccess, errorList)
            ?? ResultValidator.ValidateErrors(errorList, isFailure: !isSuccess);

        return validationError is not null
            ? FailureFromValidationError(validationError.Value)
            : CreateUnsafe(value, isSuccess, status, errorList);
    }

    public static Result<TValue, TError> Create(
        bool isSuccess,
        int status,
        List<TError>? errors = null)
    {
        return Create(default, isSuccess, status, errors);
    }

    internal static Result<TValue, TError> CreateUnsafe(
        TValue? value,
        bool isSuccess,
        int status,
        List<TError>? errors = null)
    {
        return new Result<TValue, TError>(value, isSuccess, status, errors);
    }

    private static Result<TValue, TError> FailureFromValidationError(Error error)
    {
        if (error is TError typedError)
        {
            return CreateUnsafe(default, false, error.Status, [typedError]);
        }

        return CreateUnsafe(default, false, error.Status);
    }

    #endregion

    #region Success

    public static Result<TValue, TError> Success(TValue? value)
    {
        return Success(value, ResultConstant.StatusCode.Success.Ok);
    }

    public static Result<TValue, TError> Success(TValue? value, int status)
    {
        return Create(value, true, status);
    }

    public static Result<TValue, TError> Ok(TValue? value)
    {
        return Success(value);
    }

    public static Result<TValue, TError> Created(TValue? value)
    {
        return Success(value, ResultConstant.StatusCode.Success.Created);
    }

    public static Result<TValue, TError> Accepted(TValue? value)
    {
        return Success(value, ResultConstant.StatusCode.Success.Accepted);
    }

    public static Result<TValue, TError> NoContent(TValue? value)
    {
        return Success(value, ResultConstant.StatusCode.Success.NoContent);
    }

    #endregion

    #region Failure

    public static Result<TValue, TError> Failure(TError? error)
    {
        if (error is null)
        {
            return CreateUnsafe(default, false, ResultConstant.Default.Status);
        }

        return Create(default, false, error.Status, [error]);
    }

    public static Result<TValue, TError> Failure(params TError[]? errors)
    {
        return Failure((IEnumerable<TError>?)errors);
    }

    public static Result<TValue, TError> Failure(IEnumerable<TError>? errors)
    {
        if (errors is null)
        {
            return CreateUnsafe(default, false, ResultConstant.Default.Status);
        }

        var errorList = errors.ToList();
        if (errorList.Count == 0)
        {
            return CreateUnsafe(default, false, ResultConstant.Default.Status);
        }

        return Create(default, false, errorList[0].Status, errorList);
    }

    #endregion

    #region From

    public static Result<TValue, TError> From(TValue? value)
    {
        return Success(value);
    }

    public static Result<TValue, TError> From(TError? error)
    {
        return Failure(error);
    }

    #endregion
}
