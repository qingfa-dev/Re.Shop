using BuildingBlock.Kernel.Metadata;

namespace BuildingBlock.Kernel.Errors;

public partial record Error : IError, IEquatable<Error>
{
    #region Properties
    public string Code { get; }
    public string Message { get; }
    public string? Type { get; init; }
    public string? Instance { get; init; }
    public int? Status { get; init; }
    public ErrorSeverity Severity { get; init; }
    public MetadataDictionary Metadata { get; set; } = new MetadataDictionary();
    #endregion

    #region Constructors
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

    #region  Value equality 
    public virtual bool Equals(Error? other)
        => Equals((IError?)other);

    public bool Equals(IError? other)
        => other is not null
        && Code == other.Code
        && Message == other.Message
        && Status == other.Status
        && Type == other.Type
        && Instance == other.Instance
        && Severity == other.Severity;

    public override int GetHashCode() => HashCode.Combine(Code, Message, Status, Type, Instance, Severity);
    #endregion

    public override string ToString() => $"{Code}:{Message}";
}