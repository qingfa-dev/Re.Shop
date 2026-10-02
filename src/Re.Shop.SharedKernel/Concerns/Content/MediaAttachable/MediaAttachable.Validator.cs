namespace SharedKernel.Concerns.Content.MediaAttachable;

public static class MediaAttachableValidator
{
    /// <summary>
    /// Validates the media that <see cref="MediaAttachableExtensions.AddMedia{TValue, TMedia}"/>
    /// is about to attach, without mutating the entity.
    /// </summary>
    public static Result<TValue, Error> ValidateAddMedia<TValue, TMedia>(
        TValue auditable,
        TMedia media)
        where TValue : IMediaAttachable<TMedia>
    {
        if (auditable is null)
        {
            return Result<TValue, Error>.Failure(
                MediaAttachableResult.Failure.EntityRequired);
        }

        return Result<TValue, Error>.Success(auditable)
            .Ensure(
                predicate: entity => !entity.Media.Contains(media),
                error: MediaAttachableResult.Failure.MediaDuplicate);
    }

    /// <summary>
    /// Validates that the media that
    /// <see cref="MediaAttachableExtensions.RemoveMedia{TValue, TMedia}"/>
    /// targets is attached, without mutating the entity.
    /// </summary>
    public static Result<TValue, Error> ValidateRemoveMedia<TValue, TMedia>(
        TValue auditable,
        TMedia media)
        where TValue : IMediaAttachable<TMedia>
    {
        if (auditable is null)
        {
            return Result<TValue, Error>.Failure(
                MediaAttachableResult.Failure.EntityRequired);
        }

        return Result<TValue, Error>.Success(auditable)
            .Ensure(
                predicate: entity => entity.Media.Contains(media),
                error: MediaAttachableResult.Failure.MediaNotFound);
    }
}
