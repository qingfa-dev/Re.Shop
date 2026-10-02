using BuildingBlock.Kernel.Metadata;

namespace BuildingBlock.Kernel.Errors;

public partial record Error : IError, IEquatable<Error>
{
    #region Helpers

    public Error CopyWith(
        string? code = null,
        string? message = null,
        int? status = null,
        string? type = null,
        string? instance = null,
        ErrorSeverity? severity = null,
        MetadataDictionary? metadata = null)
    {
        return new Error(
            code: code ?? Code,
            message: message ?? Message,
            status: status ?? Status,
            type: type ?? Type,
            instance: instance ?? Instance,
            severity: severity ?? Severity)
        {
            Metadata = metadata ?? Metadata
        };
    }

    #endregion

    #region Metadata

    public Error WithMetadata(string key, object value)
    {
        this.SetValue(key, value);
        return this;
    }

    public Error WithMetadata(MetadataDictionary metadata)
    {
        Metadata = metadata ?? new MetadataDictionary();
        return this;
    }

    public Error WithTraceId(string traceId) => WithMetadata(ErrorConstant.Metadata.TraceId, traceId);
    public Error WithTimestamp(DateTimeOffset timestamp) => WithMetadata(ErrorConstant.Metadata.Timestamp, timestamp);
    public Error WithResource(string resource) => WithMetadata(ErrorConstant.Metadata.Resource, resource);
    public Error WithField(string field) => WithMetadata(ErrorConstant.Metadata.Field, field);
    public Error WithAttempt(int attempt) => WithMetadata(ErrorConstant.Metadata.Attempt, attempt);

    #endregion

    #region Extensions

    public Error WithCode(string code) => CopyWith(code: code);
    public Error WithMessage(string message) => CopyWith(message: message);
    public Error WithType(string type) => CopyWith(type: type);
    public Error WithInstance(string instance) => CopyWith(instance: instance);
    public Error WithStatus(int status) => CopyWith(status: status);
    public Error WithSeverity(ErrorSeverity severity) => CopyWith(severity: severity);

    #endregion

    #region Exception

    public static Error FromException(Exception exception, string code = "general.error", ErrorSeverity severity = ErrorSeverity.Critical)
    {
        ArgumentNullException.ThrowIfNull(exception);

        // Validate: runtime type FullName is never null for instantiable exception types.
        return InternalServerError(code, exception.Message)
            .WithSeverity(severity)
            .WithMetadata("exception.type", exception.GetType().FullName!);
    }

    #endregion
}