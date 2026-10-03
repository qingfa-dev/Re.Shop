using BuildingBlock.Kernel.Errors;

namespace BuildingBlock.Kernel.Results;

/// <summary>Factory methods for creating <see cref="Result"/> instances.</summary>
public partial record Result
{
    #region Factory

    #region Custom

    /// <summary>Creates a result with explicit state, resolving the status code when omitted.</summary>
    /// <param name="isSuccess">The success state.</param>
    /// <param name="errors">Optional error list; defaults to empty.</param>
    /// <param name="statusCode">Optional status code; resolved from errors when omitted.</param>
    /// <returns>A new result.</returns>
    public static Result Custom(
        bool isSuccess,
        List<Error>? errors = null,
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

    /// <summary>Creates a successful result with the specified status code.</summary>
    /// <param name="statusCode">The HTTP status code (default 200).</param>
    /// <returns>A successful result.</returns>
    public static Result Success(
        int statusCode = ResultConstant.StatusCode.Ok)
        => Custom(
            isSuccess: true,
            statusCode: statusCode);

    /// <summary>Creates a 200 OK result.</summary>
    /// <returns>A successful result.</returns>
    public static Result Ok()
        => Success(ResultConstant.StatusCode.Ok);

    /// <summary>Creates a 201 Created result.</summary>
    /// <returns>A successful result with status 201.</returns>
    public static Result Created()
        => Success(ResultConstant.StatusCode.Created);

    /// <summary>Creates a 202 Accepted result.</summary>
    /// <returns>A successful result with status 202.</returns>
    public static Result Accepted()
        => Success(ResultConstant.StatusCode.Accepted);

    /// <summary>Creates a 204 No Content result.</summary>
    /// <returns>A successful result with status 204.</returns>
    public static Result NoContent()
        => Success(ResultConstant.StatusCode.NoContent);

    #endregion

    #region Failure

    /// <summary>Alias for <see cref="Fail(IEnumerable{Error})"/>.</summary>
    /// <param name="errors">The errors.</param>
    /// <returns>A failed result.</returns>
    public static Result Failure(
        IEnumerable<Error> errors)
        => Fail(errors);

    /// <summary>Creates a failed result with an explicit status code.</summary>
    /// <param name="errors">The errors.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <returns>A failed result.</returns>
    public static Result Failure(
        IEnumerable<Error> errors,
        int statusCode)
        => Fail(errors, statusCode);

    /// <summary>Creates a failed result from params errors.</summary>
    /// <param name="errors">The errors.</param>
    /// <returns>A failed result.</returns>
    public static Result Fail(params Error[] errors)
        => Fail((IEnumerable<Error>)errors);

    /// <summary>Creates a failed result from an error collection, resolving status from errors.</summary>
    /// <param name="errors">The errors.</param>
    /// <returns>A failed result.</returns>
    public static Result Fail(IEnumerable<Error> errors)
        => CreateFailure(errors, statusCode: null);

    /// <summary>Creates a failed result with an explicit status code.</summary>
    /// <param name="errors">The errors.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <returns>A failed result.</returns>
    public static Result Fail(IEnumerable<Error> errors, int statusCode)
        => CreateFailure(errors, statusCode);

    private static Result CreateFailure(IEnumerable<Error> errors, int? statusCode)
    {
        ArgumentNullException.ThrowIfNull(errors);

        // Compute: Materialize errors and resolve status code from errors when omitted.
        var errorList = errors.ToList();
        var resolvedStatusCode = statusCode ?? ResolveStatus(errorList);

        return new Result(
            isSuccess: false,
            errors: errorList,
            statusCode: resolvedStatusCode);
    }

    #endregion

    #region Status Resolution

    /// <summary>Resolves the highest-severity status code from the error list.</summary>
    /// <param name="errors">The error list.</param>
    /// <returns>The resolved status code; defaults to 500.</returns>
    internal static int ResolveStatus(List<Error> errors)
    {
        // Compute: Find the error with the highest severity.
        Error? highestSeverityError = null;
        foreach (var error in errors)
        {
            if (highestSeverityError is null || error.Severity > highestSeverityError.Severity)
                highestSeverityError = error;
        }

        // Return: the severity-based status if found.
        if (highestSeverityError?.Status is int status)
            return status;

        // Fallback: Return the status from the first error with a status.
        foreach (var error in errors)
        {
            if (error.Status is int fallbackStatus)
                return fallbackStatus;
        }

        // Fallback: Default to internal server error.
        return ResultConstant.StatusCode.InternalServerError;
    }

    #endregion

    #endregion
}