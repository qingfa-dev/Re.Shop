namespace BuildingBlock.Kernel.Errors;

/// <summary>Defines the severity levels for <see cref="Error"/>.</summary>
public enum ErrorSeverity
{
    /// <summary>Informational — no action required.</summary>
    Info = 0,
    /// <summary>Warning — may need attention but not critical.</summary>
    Warning = 1,
    /// <summary>Error — operation failed but can be retried or recovered.</summary>
    Error = 2,
    /// <summary>Critical — operation failed and requires immediate intervention.</summary>
    Critical = 3,
}