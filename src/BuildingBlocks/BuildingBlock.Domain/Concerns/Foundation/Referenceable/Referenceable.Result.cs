namespace BuildingBlocks.Domain.Concerns.Foundation.Referenceable;

public static class ReferenceableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.UnprocessableEntity(
            code: "Referenceable.Entity.Required",
            message: "Referenceable entity is required.");

        public static Error ReferenceRequired => Error.UnprocessableEntity(
            code: "Referenceable.Reference.Required",
            message: "Reference must be specified.");

        public static Error ReferenceTooLong => Error.UnprocessableEntity(
            code: "Referenceable.Reference.TooLong",
            message:
                $"Reference cannot exceed {ReferenceableConstant.Constraints.MaxReferenceLength} characters.");

        #endregion
    }
}
