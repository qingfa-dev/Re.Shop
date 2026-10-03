namespace BuildingBlocks.Domain.Concerns.Lifecycle.Modifiable;

public interface IModifiable
{
    DateTimeOffset? ModifiedAtUtc { get; set; }
    string? ModifiedBy { get; set; }
}
