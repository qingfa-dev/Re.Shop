namespace SharedKernel.Concerns.Content.MediaAttachable;

public static class MediaAttachableExtensions
{
    /// <summary>
    /// Attaches media to the entity.
    /// Validation runs before any mutation, so a failure leaves the entity untouched.
    /// </summary>
    public static Result<TValue, Error> AddMedia<TValue, TMedia>(
        this Result<TValue, Error> result,
        TMedia media)
        where TValue : IMediaAttachable<TMedia>
    {
        return result
            .Bind(entity =>
                MediaAttachableValidator.ValidateAddMedia(entity, media))
            .Tap(entity =>
            {
                entity.Media.Add(media);
            });
    }

    /// <summary>
    /// Detaches media from the entity.
    /// Validation runs before any mutation, so a failure leaves the entity untouched.
    /// </summary>
    public static Result<TValue, Error> RemoveMedia<TValue, TMedia>(
        this Result<TValue, Error> result,
        TMedia media)
        where TValue : IMediaAttachable<TMedia>
    {
        return result
            .Bind(entity =>
                MediaAttachableValidator.ValidateRemoveMedia(entity, media))
            .Tap(entity =>
            {
                entity.Media.Remove(media);
            });
    }
}
