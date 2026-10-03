namespace BuildingBlocks.Domain.Concerns.Content.Metafieldable;

public sealed class Metafield
{
    public string Namespace { get; set; } = null!;
    public string Key { get; set; } = null!;
    public MetafieldType Type { get; set; }
    public string? Value { get; set; }
}
