namespace SharedKernel.Concerns.Organization.Categorizable;

public static class CategorizableValidator
{
    /// <summary>
    /// Validates the category that
    /// <see cref="CategorizableExtensions.AddCategory{TValue, TCategory}"/>
    /// is about to add, without mutating the entity.
    /// </summary>
    public static Result<TValue, Error> ValidateAddCategory<TValue, TCategory>(
        TValue auditable,
        TCategory category)
        where TValue : ICategorizable<TCategory>
    {
        if (auditable is null)
        {
            return Result<TValue, Error>.Failure(
                CategorizableResult.Failure.EntityRequired);
        }

        return Result<TValue, Error>.Success(auditable)
            .Ensure(
                predicate: entity => !entity.Categories.Contains(category),
                error: CategorizableResult.Failure.CategoryDuplicate);
    }

    /// <summary>
    /// Validates that the category that
    /// <see cref="CategorizableExtensions.RemoveCategory{TValue, TCategory}"/>
    /// targets is assigned, without mutating the entity.
    /// </summary>
    public static Result<TValue, Error> ValidateRemoveCategory<
        TValue,
        TCategory>(
        TValue auditable,
        TCategory category)
        where TValue : ICategorizable<TCategory>
    {
        if (auditable is null)
        {
            return Result<TValue, Error>.Failure(
                CategorizableResult.Failure.EntityRequired);
        }

        return Result<TValue, Error>.Success(auditable)
            .Ensure(
                predicate: entity => entity.Categories.Contains(category),
                error: CategorizableResult.Failure.CategoryNotFound);
    }
}
