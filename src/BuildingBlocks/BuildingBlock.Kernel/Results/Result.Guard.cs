using BuildingBlock.Kernel.Errors;

namespace BuildingBlock.Kernel.Results;

public static class ResultGuard
{
    #region Status

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

    #endregion

    #region Errors

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
