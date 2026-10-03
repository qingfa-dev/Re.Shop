using BuildingBlock.Kernel.Errors;

namespace BuildingBlock.Kernel.Results;

public partial record Result<TValue>
{
    #region Factory

    #region Success

    /// <summary>Creates a successful result with the specified value and status code.</summary>
    /// <param name="value">The success value.</param>
    /// <param name="statusCode">The HTTP status code (default 200).</param>
    /// <returns>A successful result.</returns>
    public static Result<TValue> Ok(
        TValue value,
        int statusCode = ResultConstant.StatusCode.Ok)
        => new(
            isSuccess: true,
            value: value,
            statusCode: statusCode);

    /// <summary>Alias for <see cref="Ok(TValue, int)"/>.</summary>
    /// <param name="value">The success value.</param>
    /// <param name="statusCode">The HTTP status code (default 200).</param>
    /// <returns>A successful result.</returns>
    public static Result<TValue> Success(
        TValue value,
        int statusCode = ResultConstant.StatusCode.Ok)
        => Ok(value, statusCode);

    /// <summary>Creates a 201 Created result.</summary>
    /// <param name="value">The success value.</param>
    /// <returns>A successful result with status 201.</returns>
    public static Result<TValue> Created(TValue value)
        => Ok(value, ResultConstant.StatusCode.Created);

    /// <summary>Creates a 202 Accepted result.</summary>
    /// <param name="value">The success value.</param>
    /// <returns>A successful result with status 202.</returns>
    public static Result<TValue> Accepted(TValue value)
        => Ok(value, ResultConstant.StatusCode.Accepted);

    #endregion

    #region Failure

    /// <summary>Alias for <see cref="Fail(IEnumerable{Error})"/>.</summary>
    /// <param name="errors">The errors.</param>
    /// <returns>A failed result.</returns>
    public static new Result<TValue> Failure(IEnumerable<Error> errors)
        => Fail(errors);

    /// <summary>Creates a failed result with an explicit status code.</summary>
    /// <param name="errors">The errors.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <returns>A failed result.</returns>
    public static new Result<TValue> Failure(IEnumerable<Error> errors, int statusCode)
        => Fail(errors, statusCode);

    /// <summary>Creates a failed result from params errors.</summary>
    /// <param name="errors">The errors.</param>
    /// <returns>A failed result.</returns>
    public static new Result<TValue> Fail(params Error[] errors)
        => Fail((IEnumerable<Error>)errors);

    /// <summary>Creates a failed result from an error collection, resolving status from errors.</summary>
    /// <param name="errors">The errors.</param>
    /// <returns>A failed result.</returns>
    public static new Result<TValue> Fail(IEnumerable<Error> errors)
        => CreateFailure(errors, statusCode: null);

    /// <summary>Creates a failed result with an explicit status code.</summary>
    /// <param name="errors">The errors.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <returns>A failed result.</returns>
    public static new Result<TValue> Fail(IEnumerable<Error> errors, int statusCode)
        => CreateFailure(errors, statusCode);

    private static Result<TValue> CreateFailure(IEnumerable<Error> errors, int? statusCode)
    {
        ArgumentNullException.ThrowIfNull(errors);

        var errorList = errors.ToList();
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
