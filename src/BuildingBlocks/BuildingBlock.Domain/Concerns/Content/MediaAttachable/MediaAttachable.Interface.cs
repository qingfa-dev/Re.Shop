namespace BuildingBlocks.Domain.Concerns.Content.MediaAttachable;

public interface IMediaAttachable<TMedia>
{
    ICollection<TMedia> Media { get; }
}
