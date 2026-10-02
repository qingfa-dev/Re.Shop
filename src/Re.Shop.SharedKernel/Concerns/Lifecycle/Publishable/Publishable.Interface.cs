namespace SharedKernel.Concerns.Lifecycle.Publishable;

public interface IPublishable
{
    bool IsPublished { get; set; }
    DateTimeOffset? PublishedAtUtc { get; set; }
    string? PublishedBy { get; set; }
}
