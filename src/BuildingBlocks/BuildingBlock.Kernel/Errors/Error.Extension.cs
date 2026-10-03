using BuildingBlock.Kernel.Metadata;

namespace BuildingBlock.Kernel.Errors;

public partial record Error : IError, IEquatable<Error>
{
    #region Helpers

    /// <summary>Creates a copy of this error with the specified members overridden.</summary>
    /// <param name="code">Override for <see cref="Code"/>; keeps current value when <c>null</c>.</param>
    /// <param name="message">Override for <see cref="Message"/>; keeps current value when <c>null</c>.</param>
    /// <param name="status">Override for <see cref="Status"/>; keeps current value when <c>null</c>.</param>
    /// <param name="type">Override for <see cref="Type"/>; keeps current value when <c>null</c>.</param>
    /// <param name="instance">Override for <see cref="Instance"/>; keeps current value when <c>null</c>.</param>
    /// <param name="severity">Override for <see cref="Severity"/>; keeps current value when <c>null</c>.</param>
    /// <param name="metadata">Override for <see cref="Metadata"/>; keeps current value when <c>null</c>.</param>
    /// <returns>A new <see cref="Error"/> with the overridden members.</returns>
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

    /// <summary>Adds or overwrites a metadata entry on this error.</summary>
    /// <param name="key">The metadata key.</param>
    /// <param name="value">The metadata value.</param>
    /// <returns>This instance for chaining.</returns>
    public Error WithMetadata(string key, object value)
    {
        this.SetValue(key, value);
        return this;
    }

    /// <summary>Replaces the entire metadata bag.</summary>
    /// <param name="metadata">The metadata dictionary; replaced with a fresh empty one when <c>null</c>.</param>
    /// <returns>This instance for chaining.</returns>
    public Error WithMetadata(MetadataDictionary metadata)
    {
        Metadata = metadata ?? new MetadataDictionary();
        return this;
    }

    /// <summary>Attaches a trace identifier to <see cref="Metadata"/>.</summary>
    /// <param name="traceId">The trace identifier.</param>
    /// <returns>This instance for chaining.</returns>
    public Error WithTraceId(string traceId) => WithMetadata(ErrorConstant.Metadata.TraceId, traceId);
    /// <summary>Attaches a timestamp to <see cref="Metadata"/>.</summary>
    /// <param name="timestamp">The timestamp.</param>
    /// <returns>This instance for chaining.</returns>
    public Error WithTimestamp(DateTimeOffset timestamp) => WithMetadata(ErrorConstant.Metadata.Timestamp, timestamp);
    /// <summary>Attaches a resource identifier to <see cref="Metadata"/>.</summary>
    /// <param name="resource">The resource name.</param>
    /// <returns>This instance for chaining.</returns>
    public Error WithResource(string resource) => WithMetadata(ErrorConstant.Metadata.Resource, resource);
    /// <summary>Attaches a field name to <see cref="Metadata"/>.</summary>
    /// <param name="field">The field name.</param>
    /// <returns>This instance for chaining.</returns>
    public Error WithField(string field) => WithMetadata(ErrorConstant.Metadata.Field, field);
    /// <summary>Attaches an attempt count to <see cref="Metadata"/>.</summary>
    /// <param name="attempt">The attempt number.</param>
    /// <returns>This instance for chaining.</returns>
    public Error WithAttempt(int attempt) => WithMetadata(ErrorConstant.Metadata.Attempt, attempt);

    #endregion

    #region Extensions

    /// <summary>Returns a copy with <see cref="Code"/> overridden.</summary>
    /// <param name="code">The new error code.</param>
    /// <returns>A new <see cref="Error"/> with the overridden code.</returns>
    public Error WithCode(string code) => CopyWith(code: code);
    /// <summary>Returns a copy with <see cref="Message"/> overridden.</summary>
    /// <param name="message">The new error message.</param>
    /// <returns>A new <see cref="Error"/> with the overridden message.</returns>
    public Error WithMessage(string message) => CopyWith(message: message);
    /// <summary>Returns a copy with <see cref="Type"/> overridden.</summary>
    /// <param name="type">The new error type.</param>
    /// <returns>A new <see cref="Error"/> with the overridden type.</returns>
    public Error WithType(string type) => CopyWith(type: type);
    /// <summary>Returns a copy with <see cref="Instance"/> overridden.</summary>
    /// <param name="instance">The new instance identifier.</param>
    /// <returns>A new <see cref="Error"/> with the overridden instance.</returns>
    public Error WithInstance(string instance) => CopyWith(instance: instance);
    /// <summary>Returns a copy with <see cref="Status"/> overridden.</summary>
    /// <param name="status">The new HTTP status code.</param>
    /// <returns>A new <see cref="Error"/> with the overridden status.</returns>
    public Error WithStatus(int status) => CopyWith(status: status);
    /// <summary>Returns a copy with <see cref="Severity"/> overridden.</summary>
    /// <param name="severity">The new severity level.</param>
    /// <returns>A new <see cref="Error"/> with the overridden severity.</returns>
    public Error WithSeverity(ErrorSeverity severity) => CopyWith(severity: severity);

    #endregion

    #region Exception

    /// <summary>Creates an <see cref="Error"/> from a caught exception.</summary>
    /// <param name="exception">The exception that was caught.</param>
    /// <param name="code">The error code to assign; defaults to <c>"general.error"</c>.</param>
    /// <param name="severity">The severity; defaults to <see cref="ErrorSeverity.Critical"/>.</param>
    /// <returns>A new <see cref="Error"/> with the exception details attached.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="exception"/> is <c>null</c>.</exception>
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