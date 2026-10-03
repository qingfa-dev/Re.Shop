using BuildingBlock.Kernel.Errors;

namespace BuildingBlock.Kernel.Results;

/// <summary>Provides validation guards for result construction and mutation.</summary>
public static class ResultGuard
{
    /// <summary>Validates that a status code falls within the accepted HTTP range.</summary>
    /// <param name="statusCode">The status code to validate.</param>
    /// <param name="isSuccess"><see langword="true"/> if the result is a success; otherwise <see langword="false"/>.</param>
    /// <returns>An error when the status code is invalid; otherwise <see langword="null"/>.</returns>
    public static Error? ValidateStatusCode(int statusCode, bool isSuccess)
    {

        if (statusCode is < ResultConstant.Constraint.Status.Min or > ResultConstant.Constraint.Status.Max)
        {
            return ResultError.Status.InvalidStatusCode(statusCode);
        }

        if (isSuccess)
        {
            return statusCode is < ResultConstant.Constraint.Status.Success.Min or > ResultConstant.Constraint.Status.Success.Max
                ? ResultError.Status.InvalidSuccessStatus(statusCode)
                : null;
        }

        return statusCode is < ResultConstant.Constraint.Status.Failure.Min or > ResultConstant.Constraint.Status.Failure.Max
            ? ResultError.Status.InvalidFailureStatus(statusCode)
            : null;
    }

    #region Errors

    /// <summary>Validates the error collection for count and emptiness rules.</summary>
    /// <param name="errors">The error list to validate.</param>
    /// <param name="isSuccess"><see langword="true"/> if the result is a success; otherwise <see langword="false"/>.</param>
    /// <returns>An error when validation fails; otherwise <see langword="null"/>.</returns>
    public static Error? ValidateErrors(List<Error> errors, bool isSuccess)
    {
        ArgumentNullException.ThrowIfNull(errors);
        var count = errors.Count;

        if (!isSuccess && count == 0)
            return ResultError.Errors.Empty();

        if (count > ResultConstant.Constraint.Errors.MaxCount)
            return ResultError.Errors.ExceedsMaxCount();

        return null;
    }

    /// <summary>Validates value presence based on the result state.</summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="isSuccess"><see langword="true"/> if the result is a success; otherwise <see langword="false"/>.</param>
    /// <param name="value">The value to validate.</param>
    /// <returns>An error when the value violates state rules; otherwise <see langword="null"/>.</returns>
    public static Error? ValidateValue<TValue>(bool isSuccess, TValue value)
    {
        if (isSuccess && value is null)
        {
            return ResultError.Value.Required();
        }

        if (!isSuccess && value is not null && !typeof(TValue).IsValueType)
        {
            return ResultError.Value.NotAllowed();
        }

        return null;
    }

    #endregion

    #region Result

    /// <summary>Validates that all errors in a failure share the same status code.</summary>
    /// <param name="isSuccess"><see langword="true"/> if the result is a success; otherwise <see langword="false"/>.</param>
    /// <param name="errors">The error list to validate.</param>
    /// <returns>An error when the failure has no errors or mixed status codes; otherwise <see langword="null"/>.</returns>
    public static Error? ValidateResultConsistency(
        bool isSuccess,
        List<Error> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        using var enumerator = errors.GetEnumerator();

        // A failure must contain at least one error.
        if (!isSuccess && !enumerator.MoveNext())
        {
            return ResultError.Result.FailureWithoutErrors();
        }

        // A successful result can have no errors.
        if (isSuccess)
        {
            return null;
        }

        // We already know a failure has at least one error.
        var status = enumerator.Current.Status;

        while (enumerator.MoveNext())
        {
            if (enumerator.Current.Status != status)
            {
                return ResultError.Result.MixedStatusCodes();
            }
        }

        return null;
    }

    #endregion
}
