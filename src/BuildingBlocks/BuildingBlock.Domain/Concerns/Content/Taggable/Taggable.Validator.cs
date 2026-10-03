namespace BuildingBlocks.Domain.Concerns.Content.Taggable;

public static class TaggableValidator
{
    /// <summary>
    /// Validates the tag that <see cref="TaggableExtensions.AddTag{TValue}"/>
    /// is about to add, without mutating the entity.
    /// </summary>
    public static Result<TValue> ValidateAddTag<TValue>(
        TValue auditable,
        string? tag)
        where TValue : ITaggable
    {
        if (auditable is null)
        {
            return Result<TValue>.Fail(
                TaggableResult.Failure.EntityRequired);
        }

        return Result<TValue>.Success(auditable)
            .Ensure(
                predicate: _ => !string.IsNullOrWhiteSpace(tag),
                errorValue: TaggableResult.Failure.TagRequired)

            .Ensure(
                predicate: _ =>
                    tag!.Length <= TaggableConstant.Constraints.MaxTagLength,
                errorValue: TaggableResult.Failure.TagTooLong)

            .Ensure(
                predicate: entity => !entity.Tags.Contains(tag!),
                errorValue: TaggableResult.Failure.TagDuplicate);
    }

    /// <summary>
    /// Validates that the tag that
    /// <see cref="TaggableExtensions.RemoveTag{TValue}"/> is about to remove
    /// is present, without mutating the entity.
    /// </summary>
    public static Result<TValue> ValidateRemoveTag<TValue>(
        TValue auditable,
        string? tag)
        where TValue : ITaggable
    {
        if (auditable is null)
        {
            return Result<TValue>.Fail(
                TaggableResult.Failure.EntityRequired);
        }

        return Result<TValue>.Success(auditable)
            .Ensure(
                predicate: entity => entity.Tags.Contains(tag!),
                errorValue: TaggableResult.Failure.TagNotFound);
    }
}
