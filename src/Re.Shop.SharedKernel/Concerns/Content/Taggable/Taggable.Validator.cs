namespace SharedKernel.Concerns.Content.Taggable;

public static class TaggableValidator
{
    /// <summary>
    /// Validates the tag that <see cref="TaggableExtensions.AddTag{TValue}"/>
    /// is about to add, without mutating the entity.
    /// </summary>
    public static Result<TValue, Error> ValidateAddTag<TValue>(
        TValue auditable,
        string? tag)
        where TValue : ITaggable
    {
        if (auditable is null)
        {
            return Result<TValue, Error>.Failure(
                TaggableResult.Failure.EntityRequired);
        }

        return Result<TValue, Error>.Success(auditable)
            .Ensure(
                predicate: _ => !string.IsNullOrWhiteSpace(tag),
                error: TaggableResult.Failure.TagRequired)

            .Ensure(
                predicate: _ =>
                    tag!.Length <= TaggableConstant.Constraints.MaxTagLength,
                error: TaggableResult.Failure.TagTooLong)

            .Ensure(
                predicate: entity => !entity.Tags.Contains(tag!),
                error: TaggableResult.Failure.TagDuplicate);
    }

    /// <summary>
    /// Validates that the tag that
    /// <see cref="TaggableExtensions.RemoveTag{TValue}"/> is about to remove
    /// is present, without mutating the entity.
    /// </summary>
    public static Result<TValue, Error> ValidateRemoveTag<TValue>(
        TValue auditable,
        string? tag)
        where TValue : ITaggable
    {
        if (auditable is null)
        {
            return Result<TValue, Error>.Failure(
                TaggableResult.Failure.EntityRequired);
        }

        return Result<TValue, Error>.Success(auditable)
            .Ensure(
                predicate: entity => entity.Tags.Contains(tag!),
                error: TaggableResult.Failure.TagNotFound);
    }
}
