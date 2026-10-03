namespace BuildingBlocks.Domain.Concerns.Organization.Categorizable;

public static class CategorizableValidator
{
    /// <summary>
    /// Validates the category that
    /// <see cref="CategorizableExtensions.AddCategory{TValue, TCategory}"/>
    /// is about to add, without mutating the entity.
    /// </summary>
    public static Result<TValue> ValidateAddCategory<TValue, TCategory>(
        TValue auditable,
        TCategory category)
        where TValue : ICategorizable<TCategory>
    {
        if (auditable is null)
        {
            return Result<TValue>.Fail(
                CategorizableResult.Failure.EntityRequired);
        }

        return Result<TValue>.Success(auditable)
            .Ensure(
                predicate: entity => !entity.Categories.Contains(category),
                errorValue: CategorizableResult.Failure.CategoryDuplicate);
    }

    /// <summary>
    /// Validates that the category that
    /// <see cref="CategorizableExtensions.RemoveCategory{TValue, TCategory}"/>
    /// targets is assigned, without mutating the entity.
    /// </summary>
    public static Result<TValue> ValidateRemoveCategory<
        TValue,
        TCategory>(
        TValue auditable,
        TCategory category)
        where TValue : ICategorizable<TCategory>
    {
        if (auditable is null)
        {
            return Result<TValue>.Fail(
                CategorizableResult.Failure.EntityRequired);
        }

        return Result<TValue>.Success(auditable)
            .Ensure(
                predicate: entity => entity.Categories.Contains(category),
                errorValue: CategorizableResult.Failure.CategoryNotFound);
    }
}
