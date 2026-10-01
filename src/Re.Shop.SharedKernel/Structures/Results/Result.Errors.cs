namespace SharedKernel.Structures.Results;

public static class ResultResult
{
    public static Error EmptyErrors =>
        Error.Failure(
            ResultConstant.Failure.Errors.Empty.Code,
            ResultConstant.Failure.Errors.Empty.Message);

    public static Error ExceedsMaxErrors =>
        Error.Failure(
            ResultConstant.Failure.Errors.ExceedsMaxRange.Code,
            string.Format(
                ResultConstant.Failure.Errors.ExceedsMaxRange.Message,
                ResultConstant.Constraint.Errors.MaxCount));

    public static Error MixedStatusCodes =>
        Error.Failure(
            ResultConstant.Failure.Result.MixedStatusCodes.Code,
            ResultConstant.Failure.Result.MixedStatusCodes.Message);

    public static Error FailureWithoutErrors =>
        Error.Failure(
            ResultConstant.Failure.Result.FailureWithoutErrors.Code,
            ResultConstant.Failure.Result.FailureWithoutErrors.Message);

    public static Error MetadataEmptyKey =>
        Error.Failure(
            ResultConstant.Failure.Metadata.EmptyKey.Code,
            ResultConstant.Failure.Metadata.EmptyKey.Message);

    public static Error MetadataNullValue(string key) =>
        Error.Failure(
            ResultConstant.Failure.Metadata.NullValue.Code,
            string.Format(ResultConstant.Failure.Metadata.NullValue.Message, key));

    public static class Failure
    {
        public static Error ValueRequired =>
            Error.Validation(
                ResultConstant.Failure.Value.Required.Code,
                ResultConstant.Failure.Value.Required.Message);

        public static Error ValueNotAllowed =>
            Error.Validation(
                ResultConstant.Failure.Value.NotAllowed.Code,
                ResultConstant.Failure.Value.NotAllowed.Message);

        public static Error ExceedsMaxMetadataEntries =>
            Error.Validation(
                ResultConstant.Failure.Value.ExceedsMaxMetadataEntries.Code,
                string.Format(
                    ResultConstant.Failure.Value.ExceedsMaxMetadataEntries.Message,
                    ResultConstant.Constraint.Metadata.MaxEntries));

        public static Error InvalidStatusCode(int status) =>
            Error.Validation(
                ResultConstant.Failure.Status.InvalidStatusCode.Code,
                string.Format(
                    ResultConstant.Failure.Status.InvalidStatusCode.Message,
                    ResultConstant.Constraint.Status.Min,
                    ResultConstant.Constraint.Status.Max,
                    status));

        public static Error InvalidSuccessStatus(int status) =>
            Error.Validation(
                ResultConstant.Failure.Status.InvalidSuccessStatus.Code,
                string.Format(
                    ResultConstant.Failure.Status.InvalidSuccessStatus.Message,
                    ResultConstant.Constraint.Status.Success.Min,
                    ResultConstant.Constraint.Status.Success.Max,
                    status));

        public static Error InvalidFailureStatus(int status) =>
            Error.Validation(
                ResultConstant.Failure.Status.InvalidFailureStatus.Code,
                string.Format(
                    ResultConstant.Failure.Status.InvalidFailureStatus.Message,
                    ResultConstant.Constraint.Status.Failure.Min,
                    ResultConstant.Constraint.Status.Failure.Max,
                    status));
    }
}
