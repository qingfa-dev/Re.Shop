namespace BuildingBlocks.Domain.Concerns.Content.Localizable;

public interface ILocalizable
{
    ICollection<LocalizedValue> LocalizedValues { get; }
}
