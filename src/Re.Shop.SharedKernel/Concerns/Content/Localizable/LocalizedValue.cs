namespace SharedKernel.Concerns.Content.Localizable;

public sealed class LocalizedValue
{
    public string Property { get; set; } = null!;
    public string Locale { get; set; } = null!;
    public string? Value { get; set; }
}
