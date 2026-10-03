using BuildingBlock.Kernel.Metadata;

namespace BuildingBlock.Kernel.Errors;

/// <summary>Represents a domain or infrastructure error with code, message, and optional problem-details shape.</summary>
/// <remarks>
/// Primary surface is <see cref="Code"/> + <see cref="Message"/>.
/// The remaining members carry the ProblemDetails shape, pre-set by the category factories.
/// </remarks>
public partial record Error : IError, IEquatable<Error>
{
    #region Properties

    /// <summary>The machine-readable error code.</summary>
    public string Code { get; }
    /// <summary>The human-readable error message.</summary>
    public string Message { get; }
    /// <summary>The error type category (e.g. validation, business, system).</summary>
    public string? Type { get; }
    /// <summary>A unique instance identifier for traceability.</summary>
    public string? Instance { get; }
    /// <summary>The HTTP status code, when applicable.</summary>
    public int? Status { get; }
    /// <summary>The severity level of this error.</summary>
    public ErrorSeverity Severity { get; }
    /// <summary>Attached metadata bag (trace IDs, timestamps, etc.).</summary>
    public MetadataDictionary Metadata { get; set; } = new MetadataDictionary();

    #endregion

    #region Constructors

    /// <summary>Initializes a new error with the specified code and message.</summary>
    /// <param name="code">The machine-readable error code.</param>
    /// <param name="message">The human-readable error message.</param>
    /// <param name="status">Optional HTTP status code.</param>
    /// <param name="type">Optional error type category.</param>
    /// <param name="instance">Optional unique instance identifier.</param>
    /// <param name="severity">Optional severity; defaults to <see cref="ErrorSeverity.Error"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="code"/> or <paramref name="message"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="status"/> is outside 100-599.</exception>
    public Error(
        string code,
        string message,
        int? status = null,
        string? type = null,
        string? instance = null,
        ErrorSeverity severity = ErrorSeverity.Error)
    {
        ErrorGuard.ValidateCode(code);
        ErrorGuard.ValidateMessage(message);
        if (status.HasValue)
            ErrorGuard.ValidateStatus(status.Value);

        Code = code;
        Message = message;
        Type = type;
        Instance = instance;
        Status = status;
        Severity = severity;
    }

    #endregion

    #region Value equality

    /// <summary>Compares two <see cref="Error"/> records for value equality.</summary>
    /// <param name="other">The other error to compare against.</param>
    /// <returns><c>true</c> when all members match.</returns>
    public virtual bool Equals(Error? other)
        => Equals((IError?)other);

    /// <summary>Compares this error against any <see cref="IError"/> implementation.</summary>
    /// <param name="other">The other error to compare against.</param>
    /// <returns><c>true</c> when <see cref="Code"/>, <see cref="Message"/>, <see cref="Status"/>, <see cref="Type"/>, <see cref="Instance"/>, and <see cref="Severity"/> match.</returns>
    public bool Equals(IError? other)
        => other is not null
        && Code == other.Code
        && Message == other.Message
        && Status == other.Status
        && Type == other.Type
        && Instance == other.Instance
        && Severity == other.Severity;

    /// <summary>Computes a hash code from <see cref="Code"/>, <see cref="Message"/>, <see cref="Status"/>, <see cref="Type"/>, <see cref="Instance"/>, and <see cref="Severity"/>.</summary>
    /// <returns>The combined hash code.</returns>
    public override int GetHashCode() => HashCode.Combine(Code, Message, Status, Type, Instance, Severity);

    #endregion

    /// <summary>Returns <c>Code:Message</c>.</summary>
    /// <returns>The string representation.</returns>
    public override string ToString() => $"{Code}:{Message}";
}