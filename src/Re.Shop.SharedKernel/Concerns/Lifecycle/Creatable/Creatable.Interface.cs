namespace SharedKernel.Concerns.Lifecycle.Creatable;

public interface ICreatable
{
    DateTimeOffset CreatedAtUtc { get; set; }
    string? CreatedBy { get; set; }
}
