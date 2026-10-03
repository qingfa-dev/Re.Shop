namespace BuildingBlocks.Domain.Concerns.Content.MediaAttachable;

public static class MediaAttachableValidator
{
    /// <summary>
    /// Validates the media that <see cref="MediaAttachableExtensions.AddMedia{TValue, TMedia}"/>
    /// is about to attach, without mutating the entity.
    /// </summary>
    public static Result<TValue> ValidateAddMedia<TValue, TMedia>(
        TValue auditable,
        TMedia media)
        where TValue : IMediaAttachable<TMedia>
    {
        if (auditable is null)
        {
            return Result<TValue>.Fail(
                MediaAttachableResult.Failure.EntityRequired);
        }

        return Result<TValue>.Success(auditable)
            .Ensure(
                predicate: entity => !entity.Media.Contains(media),
                errorValue: MediaAttachableResult.Failure.MediaDuplicate);
    }

    /// <summary>
    /// Validates that the media that
    /// <see cref="MediaAttachableExtensions.RemoveMedia{TValue, TMedia}"/>
    /// targets is attached, without mutating the entity.
    /// </summary>
    public static Result<TValue> ValidateRemoveMedia<TValue, TMedia>(
        TValue auditable,
        TMedia media)
        where TValue : IMediaAttachable<TMedia>
    {
        if (auditable is null)
        {
            return Result<TValue>.Fail(
                MediaAttachableResult.Failure.EntityRequired);
        }

        return Result<TValue>.Success(auditable)
            .Ensure(
                predicate: entity => entity.Media.Contains(media),
                errorValue: MediaAttachableResult.Failure.MediaNotFound);
    }
}
