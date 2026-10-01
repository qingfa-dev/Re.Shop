using SharedKernel.Errors;

namespace SharedKernel.Results;

public static class ResultValidator
{
    #region ValidateErrors

    public static Error? ValidateErrors<TError>(
        List<TError>? errors,
        bool isFailure = true)
        where TError : IError
    {
        if (errors is null || errors.Count == 0)
        {
            return isFailure ? ResultResult.EmptyErrors : null;
        }

        if (errors.Count > ResultConstant.Constraint.Errors.MaxCount)
        {
            return ResultResult.ExceedsMaxErrors;
        }

        var status = errors[0].Status;
        return errors.Any(error => error.Status != status)
            ? ResultResult.MixedStatusCodes
            : null;
    }

    #endregion

    #region ValidateFailureHasErrors

    public static Error? ValidateFailureHasErrors<TError>(
        bool isSuccess,
        List<TError>? errors)
        where TError : IError
    {
        if (isSuccess)
        {
            return null;
        }

        if (errors is null || errors.Count == 0)
        {
            return ResultResult.FailureWithoutErrors;
        }

        return ValidateErrors(errors);
    }

    #endregion

    #region ValidateValueForSuccess

    public static Error? ValidateValueForSuccess<TValue>(TValue? value, bool isSuccess)
    {
        return isSuccess && value is null
            ? ResultResult.Failure.ValueRequired
            : null;
    }

    #endregion

    #region ValidateValueForFailure

    public static Error? ValidateValueForFailure<TValue>(TValue? value, bool isSuccess)
    {
        return !isSuccess && !EqualityComparer<TValue>.Default.Equals(value!, default!)
            ? ResultResult.Failure.ValueNotAllowed
            : null;
    }

    #endregion

    #region ValidateStatusCode

    public static Error? ValidateStatusCode(int status)
    {
        return status is < ResultConstant.Constraint.Status.Min or > ResultConstant.Constraint.Status.Max
            ? ResultResult.Failure.InvalidStatusCode(status)
            : null;
    }

    #endregion

    #region ValidateSuccessStatus

    public static Error? ValidateSuccessStatus(int status, bool isSuccess)
    {
        return isSuccess
            && status is not (
                >= ResultConstant.Constraint.Status.Success.Min
                and <= ResultConstant.Constraint.Status.Success.Max)
            ? ResultResult.Failure.InvalidSuccessStatus(status)
            : null;
    }

    #endregion

    #region ValidateFailureStatus

    public static Error? ValidateFailureStatus(int status, bool isSuccess)
    {
        return !isSuccess
            && status is not (
                >= ResultConstant.Constraint.Status.Failure.Min
                and <= ResultConstant.Constraint.Status.Failure.Max)
            ? ResultResult.Failure.InvalidFailureStatus(status)
            : null;
    }

    #endregion

    #region ValidateResult

    public static Error? ValidateResult<TValue>(TValue? value, bool isSuccess, int status)
    {
        return ValidateStatusCode(status)
            ?? ValidateValueForSuccess(value, isSuccess)
            ?? ValidateValueForFailure(value, isSuccess)
            ?? ValidateSuccessStatus(status, isSuccess)
            ?? ValidateFailureStatus(status, isSuccess);
    }

    #endregion
}
