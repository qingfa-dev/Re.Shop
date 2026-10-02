namespace SharedKernel.Concerns.Lifecycle.Activatable;

public interface IActivatable
{
    bool IsActive { get; set; }
    DateTimeOffset? ActivatedAtUtc { get; set; }
    string? ActivatedBy { get; set; }
}
