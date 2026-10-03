using BuildingBlock.Kernel.Errors;

namespace BuildingBlock.Kernel.Results;

public static class ResultError
{
    #region Status

    public static class Status
    {
        public static Error InvalidStatusCode(int statusCode)
            => Error.InternalServerError(
                code: ResultConstant.Failure.Status.InvalidStatusCode.Code,
                message: string.Format(
                    ResultConstant.Failure.Status.InvalidStatusCode.Message,
                    ResultConstant.Constraint.Status.Min,
                    ResultConstant.Constraint.Status.Max,
                    statusCode));

        public static Error InvalidSuccessStatus(int statusCode)
            => Error.InternalServerError(
                code: ResultConstant.Failure.Status.InvalidSuccessStatus.Code,
                message: string.Format(
                    ResultConstant.Failure.Status.InvalidSuccessStatus.Message,
                    ResultConstant.Constraint.Status.Success.Min,
                    ResultConstant.Constraint.Status.Success.Max,
                    statusCode));

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

    public static class Result
    {
        public static Error MixedStatusCodes()
            => Error.InternalServerError(
                code: ResultConstant.Failure.Result.MixedStatusCodes.Code,
                message: ResultConstant.Failure.Result.MixedStatusCodes.Message);

        public static Error FailureWithoutErrors()
            => Error.InternalServerError(
                code: ResultConstant.Failure.Result.FailureWithoutErrors.Code,
                message: ResultConstant.Failure.Result.FailureWithoutErrors.Message);
    }

    #endregion

    #region Errors

    public static class Errors
    {
        public static Error Empty()
            => Error.InternalServerError(
                code: ResultConstant.Failure.Errors.Empty.Code,
                message: ResultConstant.Failure.Errors.Empty.Message);

        public static Error ExceedsMaxCount()
            => Error.InternalServerError(
                code: ResultConstant.Failure.Errors.ExceedsMaxCount.Code,
                message: string.Format(
                    ResultConstant.Failure.Errors.ExceedsMaxCount.Message,
                    ResultConstant.Constraint.Errors.MaxCount));
    }

    #endregion

    #region Metadata

    public static class Metadata
    {
        public static Error EmptyKey()
            => Error.InternalServerError(
                code: ResultConstant.Failure.Metadata.EmptyKey.Code,
                message: ResultConstant.Failure.Metadata.EmptyKey.Message);

        public static Error NullValue(string key)
            => Error.InternalServerError(
                code: ResultConstant.Failure.Metadata.NullValue.Code,
                message: string.Format(
                    ResultConstant.Failure.Metadata.NullValue.Message,
                    key));

        public static Error ExceedsMaxEntries()
            => Error.InternalServerError(
                code: ResultConstant.Failure.Metadata.ExceedsMaxEntries.Code,
                message: string.Format(
                    ResultConstant.Failure.Metadata.ExceedsMaxEntries.Message,
                    ResultConstant.Constraint.Metadata.MaxEntries));
    }

    #endregion

    #region Value

    public static class Value
    {
        public static Error Required()
            => Error.InternalServerError(
                code: ResultConstant.Failure.Value.Required.Code,
                message: ResultConstant.Failure.Value.Required.Message);

        public static Error NotAllowed()
            => Error.InternalServerError(
                code: ResultConstant.Failure.Value.NotAllowed.Code,
                message: ResultConstant.Failure.Value.NotAllowed.Message);
    }

    #endregion

    #region Flow

    public static class Flow
    {
        public static Error CallbackMissing(string operation)
            => Error.InternalServerError(
                code: ResultConstant.Failure.Flow.CallbackMissing.Code,
                message: string.Format(ResultConstant.Failure.Flow.CallbackMissing.Message, operation));

        public static Error ErrorMissing(string operation)
            => Error.InternalServerError(
                code: ResultConstant.Failure.Flow.ErrorMissing.Code,
                message: string.Format(ResultConstant.Failure.Flow.ErrorMissing.Message, operation));

        public static Error ResultMissing(string operation)
            => Error.InternalServerError(
                code: ResultConstant.Failure.Flow.ResultMissing.Code,
                message: string.Format(ResultConstant.Failure.Flow.ResultMissing.Message, operation));
    }

    #endregion
}
