using BuildingBlock.Kernel.Metadata;

namespace BuildingBlock.Kernel.Errors;

/// <summary>
/// Error contract. Primary surface is <see cref="Code"/> + <see cref="Message"/>.
/// The remaining members carry the ProblemDetails shape, pre-set by the
/// category factories on <see cref="Error"/>.
/// </summary>
public interface IError : IMetadata
{
    string Code { get; }
    string Message { get; }
    string? Type { get; }
    string? Instance { get; }
    int? Status { get; }
    ErrorSeverity Severity { get; }
}