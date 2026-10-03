using BuildingBlock.Kernel.Errors;
using BuildingBlock.Kernel.Metadata;

namespace BuildingBlock.Kernel.Results;

/// <summary>Represents an operation result that is either a success or a failure.</summary>
public partial record Result : IResult<Error>, IEquatable<Result>
{
    #region Properties

    /// <summary>Gets a value indicating whether the result is a success.</summary>
    public bool IsSuccess { get; }

    /// <summary>Gets the error list associated with the result.</summary>
    public List<Error> Errors { get; }
    /// <summary>Gets or sets the metadata bag attached to the result.</summary>
    public MetadataDictionary Metadata { get; set; } = new();

    /// <summary>Gets <see langword="true"/> when the result is a success; otherwise <see langword="false"/>.</summary>
    public bool HasValue => IsSuccess;
    /// <summary>Gets <see langword="true"/> when the result is a failure; otherwise <see langword="false"/>.</summary>
    public bool IsFailure => !IsSuccess;
    /// <summary>Gets or sets the HTTP status code.</summary>
    public int StatusCode { get; set; } = ResultConstant.Default.Status;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a result with explicit state, validating errors and status codes.
    /// </summary>
    /// <param name="isSuccess">The success state.</param>
    /// <param name="errors">The error list; <see langword="null"/> is treated as empty.</param>
    /// <param name="statusCode">Optional status code; defaults to 200 on success or 500 on failure.</param>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    public Result(
        bool isSuccess,
        List<Error> errors,
        int? statusCode = null)
    {
        var normalizedErrors = errors ?? ResultConstant.Default.EmptyErrors;
        var resolvedStatusCode = statusCode ?? (isSuccess ? ResultConstant.StatusCode.Ok : ResultConstant.StatusCode.InternalServerError);

        var error = ResultGuard.ValidateStatusCode(resolvedStatusCode, isSuccess)
            ?? ResultGuard.ValidateErrors(normalizedErrors, isSuccess)
            ?? ResultGuard.ValidateResultConsistency(isSuccess, normalizedErrors);

        if (error is not null)
            throw new ArgumentException(error.ToString(), nameof(errors));

        IsSuccess = isSuccess;
        Errors = normalizedErrors;
        StatusCode = resolvedStatusCode;
    }

    #endregion

    #region Copy

    /// <summary>
    /// Creates a validated copy of this result, replacing only the supplied state.
    /// The copy receives independent error and metadata collections.
    /// </summary>
    /// <param name="isSuccess">The replacement success state, or <see langword="null"/> to preserve it.</param>
    /// <param name="errors">Replacement errors, or <see langword="null"/> to preserve the current errors.</param>
    /// <param name="statusCode">The replacement status code, or <see langword="null"/> to preserve or resolve it.</param>
    /// <param name="metadata">Replacement metadata; when omitted, the current metadata is copied.</param>
    /// <returns>A new result containing the requested state.</returns>
    public Result CopyWith(
        bool? isSuccess = null,
        List<Error>? errors = null,
        int? statusCode = null,
        MetadataDictionary? metadata = null)
    {
        var resolvedIsSuccess = isSuccess ?? IsSuccess;
        var resolvedErrors = errors is not null
            ? errors.ToList()
            : resolvedIsSuccess && !IsSuccess
                ? ResultConstant.Default.EmptyErrors
                : [.. Errors];
        var statusWasChanged = isSuccess.HasValue && isSuccess.Value != IsSuccess;
        var errorsWereChanged = errors is not null;
        var resolvedStatusCode = statusCode
            ?? (statusWasChanged
                ? resolvedIsSuccess
                    ? ResultConstant.StatusCode.Ok
                    : ResolveStatus(resolvedErrors)
                : errorsWereChanged && !resolvedIsSuccess
                    ? ResolveStatus(resolvedErrors)
                    : StatusCode);

        return new Result(
            isSuccess: resolvedIsSuccess,
            errors: resolvedErrors,
            statusCode: resolvedStatusCode)
        {
            Metadata = MetadataDictionary.Create(metadata ?? Metadata)
        };
    }

    #endregion

    #region Equality

    /// <summary>Compares two results by state and error list, ignoring status code and metadata.</summary>
    /// <param name="other">The other result to compare.</param>
    /// <returns><see langword="true"/> when both results have the same state and equal errors.</returns>
    public virtual bool Equals(Result? other)
        => other is not null
        && IsSuccess == other.IsSuccess
        && Errors.SequenceEqual(other.Errors);

    /// <summary>Computes a hash code from the success state and error list.</summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(IsSuccess);
        foreach (var error in Errors)
            hash.Add(error);
        return hash.ToHashCode();
    }

    #endregion
    #region ToString

    /// <summary>Renders "Success" or "Failure: {errors}".</summary>
    /// <returns>A human-readable result representation.</returns>
    public override string ToString()
        => IsSuccess
            ? "Success"
            : $"Failure: {string.Join(", ", Errors.Select(e => e.ToString()))}";
    #endregion
}
