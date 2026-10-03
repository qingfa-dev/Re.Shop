using System.Diagnostics.CodeAnalysis;

using BuildingBlock.Kernel.Errors;
using BuildingBlock.Kernel.Metadata;

namespace BuildingBlock.Kernel.Results;

public partial record Result<TValue> : Result, IResult<TValue, Error>
{
    #region Value

    [AllowNull]
    public TValue Value { get; }

    #endregion

    #region Constructor

    public Result(
        bool isSuccess,
        TValue value,
        List<Error>? errors = null,
        int? statusCode = null)
        : base(
            isSuccess: isSuccess,
            errors: errors ?? ResultConstant.Default.EmptyErrors,
            statusCode: statusCode)
    {
        var valueError = ResultGuard.ValidateValue(isSuccess, value);
        if (valueError is not null)
            throw new ArgumentException(valueError.ToString(), nameof(value));

        Value = value;
    }

    #endregion

    #region Copy

    /// <summary>
    /// Creates a validated copy while preserving the current value when the copy
    /// remains successful. A failure copy clears the value. Use the value overload
    /// to transition a failure to success.
    /// The copy receives independent error and metadata collections.
    /// </summary>
    /// <param name="isSuccess">The replacement success state, or <see langword="null"/> to preserve it.</param>
    /// <param name="errors">Replacement errors, or <see langword="null"/> to preserve the current errors.</param>
    /// <param name="statusCode">The replacement status code, or <see langword="null"/> to preserve or resolve it.</param>
    /// <param name="metadata">Replacement metadata; when omitted, the current metadata is copied.</param>
    /// <returns>A new generic result containing the requested state.</returns>
    public new Result<TValue> CopyWith(
        bool? isSuccess = null,
        List<Error>? errors = null,
        int? statusCode = null,
        MetadataDictionary? metadata = null)
    {
        var resolvedIsSuccess = isSuccess ?? IsSuccess;
        if (resolvedIsSuccess && !IsSuccess)
        {
            throw new ArgumentException(
                ResultError.Value.Required().ToString(),
                nameof(isSuccess));
        }

        var resolvedErrors = (errors ?? Errors).ToList();
        var statusWasChanged = isSuccess.HasValue && isSuccess.Value != IsSuccess;
        var errorsWereChanged = errors is not null;
        var resolvedStatusCode = statusCode
            ?? (statusWasChanged || (errorsWereChanged && !resolvedIsSuccess)
                ? Result.ResolveStatus(resolvedErrors)
                : StatusCode);

        return new Result<TValue>(
            isSuccess: resolvedIsSuccess,
            value: resolvedIsSuccess ? Value : default!,
            errors: resolvedErrors,
            statusCode: resolvedStatusCode)
        {
            Metadata = MetadataDictionary.Create(metadata ?? Metadata)
        };
    }

    /// <summary>
    /// Creates a validated copy with a replacement value and optional state changes.
    /// The supplied value is used only when the resulting copy is successful; a failure
    /// copy always clears its value.
    /// </summary>
    /// <param name="value">The value for the copied result.</param>
    /// <param name="isSuccess">The replacement success state, or <see langword="null"/> to preserve it.</param>
    /// <param name="errors">Replacement errors, or <see langword="null"/> to preserve the current errors.</param>
    /// <param name="statusCode">The replacement status code, or <see langword="null"/> to preserve or resolve it.</param>
    /// <param name="metadata">Replacement metadata; when omitted, the current metadata is copied.</param>
    /// <returns>A new generic result containing the requested value and state.</returns>
    public Result<TValue> CopyWith(
        TValue value,
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
                    : Result.ResolveStatus(resolvedErrors)
                : errorsWereChanged && !resolvedIsSuccess
                    ? Result.ResolveStatus(resolvedErrors)
                    : StatusCode);

        return new Result<TValue>(
            isSuccess: resolvedIsSuccess,
            value: resolvedIsSuccess ? value : default!,
            errors: resolvedErrors,
            statusCode: resolvedStatusCode)
        {
            Metadata = MetadataDictionary.Create(metadata ?? Metadata)
        };
    }

    #endregion

    #region Value Access

    public bool TryGetValue([MaybeNullWhen(false)] out TValue value)
    {
        if (IsSuccess)
        {
            value = Value;
            return true;
        }

        value = default!;
        return false;
    }

    public override string ToString()
        => IsSuccess
            ? $"Success: {Value}"
            : $"Failure: {string.Join(", ", Errors.Select(e => e.ToString()))}";

    #endregion
}
