namespace BuildingBlocks.Domain.Concerns.Foundation.Tenantable;

public static class TenantableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.UnprocessableEntity(
            code: "Tenantable.Entity.Required",
            message: "Tenantable entity is required.");

        public static Error TenantIdRequired => Error.UnprocessableEntity(
            code: "Tenantable.TenantId.Required",
            message: "Tenant identifier must be specified.");

        #endregion
    }
}
