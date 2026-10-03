using BuildingBlock.Kernel.Metadata;

namespace BuildingBlock.Kernel.Errors;

/// <summary>Error contract. Primary surface is <see cref="Code"/> + <see cref="Message"/>.</summary>
/// <remarks>
/// The remaining members carry the ProblemDetails shape, pre-set by the category factories on <see cref="Error"/>.
/// </remarks>
public interface IError : IMetadata
{
    /// <summary>The machine-readable error code.</summary>
    string Code { get; }
    /// <summary>The human-readable error message.</summary>
    string Message { get; }
    /// <summary>The error type category (e.g. validation, business, system).</summary>
    string? Type { get; }
    /// <summary>A unique instance identifier for traceability.</summary>
    string? Instance { get; }
    /// <summary>The HTTP status code, when applicable.</summary>
    int? Status { get; }
    /// <summary>The severity level of this error.</summary>
    ErrorSeverity Severity { get; }
}