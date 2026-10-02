namespace SharedKernel.Concerns.Content.Taggable;

public static class TaggableExtensions
{
    /// <summary>
    /// Adds a tag to the entity.
    /// Validation runs before any mutation, so a failure leaves the entity untouched.
    /// </summary>
    public static Result<TValue, Error> AddTag<TValue>(
        this Result<TValue, Error> result,
        string? tag)
        where TValue : ITaggable
    {
        return result
            .Bind(entity => TaggableValidator.ValidateAddTag(entity, tag))
            .Tap(entity =>
            {
                entity.Tags.Add(tag!);
            });
    }

    /// <summary>
    /// Removes a tag from the entity.
    /// Validation runs before any mutation, so a failure leaves the entity untouched.
    /// </summary>
    public static Result<TValue, Error> RemoveTag<TValue>(
        this Result<TValue, Error> result,
        string? tag)
        where TValue : ITaggable
    {
        return result
            .Bind(entity => TaggableValidator.ValidateRemoveTag(entity, tag))
            .Tap(entity =>
            {
                entity.Tags.Remove(tag!);
            });
    }
}
