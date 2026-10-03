namespace BuildingBlocks.Domain.Concerns.Content.Taggable;

public interface ITaggable
{
    ICollection<string> Tags { get; }
}
