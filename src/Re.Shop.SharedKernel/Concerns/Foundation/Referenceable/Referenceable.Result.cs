namespace SharedKernel.Concerns.Foundation.Referenceable;

public static class ReferenceableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.Validation(
            code: "referenceable.entity.required",
            message: "Referenceable entity is required.");

        public static Error ReferenceRequired => Error.Validation(
            code: "referenceable.reference.required",
            message: "Reference must be specified.");

        public static Error ReferenceTooLong => Error.Validation(
            code: "referenceable.reference.too_long",
            message:
                $"Reference cannot exceed {ReferenceableConstant.Constraints.MaxReferenceLength} characters.");

        #endregion
    }
}
