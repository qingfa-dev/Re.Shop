namespace SharedKernel.Concerns.Content.MediaAttachable;

public interface IMediaAttachable<TMedia>
{
    ICollection<TMedia> Media { get; }
}
