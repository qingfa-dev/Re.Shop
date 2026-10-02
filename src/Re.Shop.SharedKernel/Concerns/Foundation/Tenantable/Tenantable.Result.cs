namespace SharedKernel.Concerns.Foundation.Tenantable;

public static class TenantableResult
{
    public static class Failure
    {
        #region Validation

        public static Error EntityRequired => Error.Validation(
            code: "tenantable.entity.required",
            message: "Tenantable entity is required.");

        public static Error TenantIdRequired => Error.Validation(
            code: "tenantable.tenant_id.required",
            message: "Tenant identifier must be specified.");

        #endregion
    }
}
