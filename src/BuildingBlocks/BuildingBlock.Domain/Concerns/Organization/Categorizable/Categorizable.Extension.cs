namespace BuildingBlocks.Domain.Concerns.Organization.Categorizable;

public static class CategorizableExtensions
{
    /// <summary>
    /// Adds a category to the entity.
    /// Validation runs before any mutation, so a failure leaves the entity untouched.
    /// </summary>
    public static Result<TValue> AddCategory<TValue, TCategory>(
        this Result<TValue> result,
        TCategory category)
        where TValue : ICategorizable<TCategory>
    {
        return result
            .Bind(entity =>
                CategorizableValidator.ValidateAddCategory(entity, category))
            .Tap(entity =>
            {
                entity.Categories.Add(category);
            });
    }

    /// <summary>
    /// Removes a category from the entity.
    /// Validation runs before any mutation, so a failure leaves the entity untouched.
    /// </summary>
    public static Result<TValue> RemoveCategory<TValue, TCategory>(
        this Result<TValue> result,
        TCategory category)
        where TValue : ICategorizable<TCategory>
    {
        return result
            .Bind(entity =>
                CategorizableValidator.ValidateRemoveCategory(entity, category))
            .Tap(entity =>
            {
                entity.Categories.Remove(category);
            });
    }
}
