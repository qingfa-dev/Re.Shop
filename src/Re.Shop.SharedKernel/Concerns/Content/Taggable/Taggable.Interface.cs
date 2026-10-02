namespace SharedKernel.Concerns.Content.Taggable;

public interface ITaggable
{
    ICollection<string> Tags { get; }
}
