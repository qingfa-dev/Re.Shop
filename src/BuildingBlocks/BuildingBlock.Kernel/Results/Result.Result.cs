using BuildingBlock.Kernel.Errors;

namespace BuildingBlock.Kernel.Results;

/// <summary>Factory methods for result-level error objects.</summary>
public static class ResultError
{
    #region Status

    /// <summary>Status-code validation error factories.</summary>
    public static class Status
    {
        /// <summary>Creates an error for a status code outside the 100–599 range.</summary>
        /// <param name="statusCode">The invalid status code.</param>
        /// <returns>An internal server error with the invalid-status-code code.</returns>
        public static Error InvalidStatusCode(int statusCode)
            => Error.InternalServerError(
                code: ResultConstant.Failure.Status.InvalidStatusCode.Code,
                message: string.Format(
                    ResultConstant.Failure.Status.InvalidStatusCode.Message,
                    ResultConstant.Constraint.Status.Min,
                    ResultConstant.Constraint.Status.Max,
                    statusCode));

        /// <summary>Creates an error for a success result with a non-2xx status code.</summary>
        /// <param name="statusCode">The invalid success status code.</param>
        /// <returns>An internal server error.</returns>
        public static Error InvalidSuccessStatus(int statusCode)
            => Error.InternalServerError(
                code: ResultConstant.Failure.Status.InvalidSuccessStatus.Code,
                message: string.Format(
                    ResultConstant.Failure.Status.InvalidSuccessStatus.Message,
                    ResultConstant.Constraint.Status.Success.Min,
                    ResultConstant.Constraint.Status.Success.Max,
                    statusCode));

        /// <summary>Creates an error for a failure result with a non-4xx/5xx status code.</summary>
        /// <param name="statusCode">The invalid failure status code.</param>
        /// <returns>An internal server error.</returns>
        public static Error InvalidFailureStatus(int statusCode)
            => Error.InternalServerError(
                code: ResultConstant.Failure.Status.InvalidFailureStatus.Code,
                message: string.Format(
                    ResultConstant.Failure.Status.InvalidFailureStatus.Message,
                    ResultConstant.Constraint.Status.Failure.Min,
                    ResultConstant.Constraint.Status.Failure.Max,
                    statusCode));
    }

    #endregion

    #region Result

    /// <summary>Result-level validation error factories.</summary>
    public static class Result
    {
        /// <summary>Creates an error for a result containing mixed status codes.</summary>
        /// <returns>An internal server error.</returns>
        public static Error MixedStatusCodes()
            => Error.InternalServerError(
                code: ResultConstant.Failure.Result.MixedStatusCodes.Code,
                message: ResultConstant.Failure.Result.MixedStatusCodes.Message);

        /// <summary>Creates an error for a failure result without errors.</summary>
        /// <returns>An internal server error.</returns>
        public static Error FailureWithoutErrors()
            => Error.InternalServerError(
                code: ResultConstant.Failure.Result.FailureWithoutErrors.Code,
                message: ResultConstant.Failure.Result.FailureWithoutErrors.Message);
    }

    #endregion

    #region Errors

    /// <summary>Error-collection validation error factories.</summary>
    public static class Errors
    {
        /// <summary>Creates an error when an error collection is empty.</summary>
        /// <returns>An internal server error.</returns>
        public static Error Empty()
            => Error.InternalServerError(
                code: ResultConstant.Failure.Errors.Empty.Code,
                message: ResultConstant.Failure.Errors.Empty.Message);

        /// <summary>Creates an error when an error collection exceeds the maximum count.</summary>
        /// <returns>An internal server error.</returns>
        public static Error ExceedsMaxCount()
            => Error.InternalServerError(
                code: ResultConstant.Failure.Errors.ExceedsMaxCount.Code,
                message: string.Format(
                    ResultConstant.Failure.Errors.ExceedsMaxCount.Message,
                    ResultConstant.Constraint.Errors.MaxCount));
    }

    #endregion

    #region Metadata

    /// <summary>Metadata validation error factories.</summary>
    public static class Metadata
    {
        /// <summary>Creates an error when a metadata key is null, empty, or whitespace.</summary>
        /// <returns>An internal server error.</returns>
        public static Error EmptyKey()
            => Error.InternalServerError(
                code: ResultConstant.Failure.Metadata.EmptyKey.Code,
                message: ResultConstant.Failure.Metadata.EmptyKey.Message);

        /// <summary>Creates an error when a metadata value is null.</summary>
        /// <param name="key">The metadata key.</param>
        /// <returns>An internal server error.</returns>
        public static Error NullValue(string key)
            => Error.InternalServerError(
                code: ResultConstant.Failure.Metadata.NullValue.Code,
                message: string.Format(
                    ResultConstant.Failure.Metadata.NullValue.Message,
                    key));

        /// <summary>Creates an error when metadata exceeds the maximum entry count.</summary>
        /// <returns>An internal server error.</returns>
        public static Error ExceedsMaxEntries()
            => Error.InternalServerError(
                code: ResultConstant.Failure.Metadata.ExceedsMaxEntries.Code,
                message: string.Format(
                    ResultConstant.Failure.Metadata.ExceedsMaxEntries.Message,
                    ResultConstant.Constraint.Metadata.MaxEntries));
    }

    #endregion

    #region Value

    /// <summary>Value access validation error factories.</summary>
    public static class Value
    {
        /// <summary>Creates an error when a successful result has no value.</summary>
        /// <returns>An internal server error.</returns>
        public static Error Required()
            => Error.InternalServerError(
                code: ResultConstant.Failure.Value.Required.Code,
                message: ResultConstant.Failure.Value.Required.Message);

        /// <summary>Creates an error when a failed result has a value.</summary>
        /// <returns>An internal server error.</returns>
        public static Error NotAllowed()
            => Error.InternalServerError(
                code: ResultConstant.Failure.Value.NotAllowed.Code,
                message: ResultConstant.Failure.Value.NotAllowed.Message);
    }

    #endregion

    #region Flow

    /// <summary>Flow-operation validation error factories.</summary>
    public static class Flow
    {
        /// <summary>Creates an error when a required callback is missing.</summary>
        /// <param name="operation">The operation name.</param>
        /// <returns>An internal server error.</returns>
        public static Error CallbackMissing(string operation)
            => Error.InternalServerError(
                code: ResultConstant.Failure.Flow.CallbackMissing.Code,
                message: string.Format(ResultConstant.Failure.Flow.CallbackMissing.Message, operation));

        /// <summary>Creates an error when an error callback returns null.</summary>
        /// <param name="operation">The operation name.</param>
        /// <returns>An internal server error.</returns>
        public static Error ErrorMissing(string operation)
            => Error.InternalServerError(
                code: ResultConstant.Failure.Flow.ErrorMissing.Code,
                message: string.Format(ResultConstant.Failure.Flow.ErrorMissing.Message, operation));

        /// <summary>Creates an error when a result callback returns null.</summary>
        /// <param name="operation">The operation name.</param>
        /// <returns>An internal server error.</returns>
        public static Error ResultMissing(string operation)
            => Error.InternalServerError(
                code: ResultConstant.Failure.Flow.ResultMissing.Code,
                message: string.Format(ResultConstant.Failure.Flow.ResultMissing.Message, operation));
    }

    #endregion
}
