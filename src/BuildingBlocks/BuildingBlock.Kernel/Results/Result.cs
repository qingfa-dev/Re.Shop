using BuildingBlock.Kernel.Errors;
using BuildingBlock.Kernel.Metadata;

namespace BuildingBlock.Kernel.Results;

public partial record Result : IResult, IEquatable<Result>
{
    #region Properties
    public bool IsSuccess { get; }
    
    public IReadOnlyList<IError> Errors { get; }
    public MetadataDictionary Metadata { get; set; } = new();

    public bool HasValue => IsSuccess;
    public bool IsFailure => !IsSuccess;
    public int StatusCode { get; set; } = ResultConstant.Default.Status;

    #endregion

    #region Constructors

    public Result(
        bool isSuccess,
        IReadOnlyList<IError> errors,
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
        IReadOnlyList<IError>? errors = null,
        int? statusCode = null,
        MetadataDictionary? metadata = null)
    {
        var resolvedIsSuccess = isSuccess ?? IsSuccess;
        var resolvedErrors = errors is not null
            ? errors.ToArray()
            : resolvedIsSuccess && !IsSuccess
                ? ResultConstant.Default.EmptyErrors
                : Errors.ToArray();
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

    public virtual bool Equals(Result? other)
        => other is not null
        && IsSuccess == other.IsSuccess
        && Errors.SequenceEqual(other.Errors);

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

    public override string ToString()
        => IsSuccess
            ? "Success"
            : $"Failure: {string.Join(", ", Errors.Select(e => e.ToString()))}";
    #endregion
}
