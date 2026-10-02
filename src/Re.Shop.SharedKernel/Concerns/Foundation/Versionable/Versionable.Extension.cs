namespace SharedKernel.Concerns.Foundation.Versionable;

public static class VersionableExtensions
{
    /// <summary>
    /// Increments the entity's version by one.
    /// Validation runs before any mutation, so a failure leaves the entity untouched.
    /// </summary>
    public static Result<TValue, Error> BumpVersion<TValue>(
        this Result<TValue, Error> result)
        where TValue : IVersionable
    {
        return result
            .Bind(entity => VersionableValidator.ValidateBump(entity))

            .Tap(entity =>
            {
                entity.Version++;
            });
    }
}
