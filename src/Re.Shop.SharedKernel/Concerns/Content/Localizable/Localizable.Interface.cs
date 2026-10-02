namespace SharedKernel.Concerns.Content.Localizable;

public interface ILocalizable
{
    ICollection<LocalizedValue> LocalizedValues { get; }
}
