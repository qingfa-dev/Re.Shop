using SharedKernel.Errors;
using SharedKernel.Meta;

namespace SharedKernel.Results;

public partial record Result<TValue, TError>
    where TError : IError
{
    #region Map

    public Result<TNewValue, TError> Map<TNewValue>(Func<TValue, TNewValue>? mapper)
    {
        if (mapper is null)
        {
            return Result<TNewValue, TError>.Failure(Errors);
        }

        if (IsFailure)
        {
            return Result<TNewValue, TError>.Failure(Errors).WithAdditionalMetadata(Metadata);
        }

        return Result<TNewValue, TError>.Success(mapper(Value)).WithAdditionalMetadata(Metadata);
    }

    #endregion

    #region Bind

    public Result<TNewValue, TError> Bind<TNewValue>(
        Func<TValue, Result<TNewValue, TError>>? binder)
    {
        if (binder is null)
        {
            return Result<TNewValue, TError>.Failure(Errors);
        }

        if (IsFailure)
        {
            return Result<TNewValue, TError>.Failure(Errors).WithAdditionalMetadata(Metadata);
        }

        return binder(Value);
    }

    public Result<TValue, TError> Bind(Func<Result<TValue, TError>>? next)
    {
        if (IsFailure)
        {
            return this;
        }

        if (next is null)
        {
            return CreateUnsafe(
                default,
                false,
                ResultConstant.Default.Status);
        }

        return next();
    }

    #endregion

    #region Match

    public TResult Match<TResult>(
        Func<TValue, TResult>? onSuccess,
        Func<IReadOnlyList<TError>, TResult>? onFailure)
    {
        if (onSuccess is null || onFailure is null)
        {
            return default!;
        }

        return IsSuccess ? onSuccess(Value) : onFailure(Errors);
    }

    public TResult? Match<TResult>(
        Func<TResult>? onSuccess,
        Func<List<TError>, TResult>? onFailure)
    {
        if (onSuccess is null || onFailure is null)
        {
            return default;
        }

        return IsSuccess ? onSuccess() : onFailure(Errors);
    }

    #endregion

    #region Tap

    public Result<TValue, TError> Tap(Action<TValue>? onSuccess)
    {
        if (onSuccess is not null && HasValue)
        {
            onSuccess(Value);
        }

        return this;
    }

    public Result<TValue, TError> Tap(Action? onSuccess)
    {
        if (onSuccess is not null && IsSuccess)
        {
            onSuccess();
        }

        return this;
    }

    #endregion

    #region TapErrors

    public Result<TValue, TError> TapErrors(Action<List<TError>>? onErrors)
    {
        if (onErrors is not null && IsFailure)
        {
            onErrors(Errors);
        }

        return this;
    }

    #endregion

    #region TapError

    public Result<TValue, TError> TapError(Action<TError>? onError)
    {
        if (onError is not null && IsFailure && Errors.Count > 0)
        {
            onError(Errors[0]);
        }

        return this;
    }

    #endregion

    #region GetValueOrDefault

    public TValue GetValueOrDefault(TValue defaultValue = default!)
    {
        return HasValue ? Value : defaultValue;
    }

    #endregion

    #region GetValueOrThrow

    public TValue GetValueOrThrow()
    {
        if (IsFailure)
        {
            var errorSummary = string.Join("; ", Errors.Select(error => $"{error.Code}: {error.Message}"));
            throw new InvalidOperationException($"Errors({Errors.Count}): [{errorSummary}]");
        }

        return Value;
    }

    #endregion

    #region ThrowIfFailure

    public Result<TValue, TError> ThrowIfFailure()
    {
        if (IsFailure)
        {
            var errorSummary = string.Join("; ", Errors.Select(error => $"{error.Code}: {error.Message}"));
            var message = $"Errors({Errors.Count}): [{errorSummary}]";

            if (typeof(TValue) == typeof(Unit))
            {
                var targets = Errors
                    .Select(error => error.Metadata is not null
                        && error.Metadata.TryGetValue(MetadataConstant.MetadataKey.Target, out var target)
                            ? target?.ToString()
                            : null)
                    .Where(target => target is not null)
                    .ToList();

                if (targets.Count > 0)
                {
                    throw new ArgumentException(message, string.Join(", ", targets));
                }
            }

            throw new InvalidOperationException(message);
        }

        return this;
    }

    #endregion

    #region MapError

    public Result<TValue, TNewError> MapError<TNewError>(Func<TError, TNewError>? mapper)
        where TNewError : IError
    {
        if (mapper is null)
        {
            if (IsFailure)
            {
                return Result<TValue, TNewError>.CreateUnsafe(
                    default,
                    false,
                    Status,
                    [])
                    .WithAdditionalMetadata(Metadata);
            }

            if (typeof(TValue) == typeof(Unit))
            {
                return Result<TValue, TNewError>.CreateUnsafe(Value, true, Status, [])
                    .WithAdditionalMetadata(Metadata);
            }

            return Result<TValue, TNewError>.Failure(Array.Empty<TNewError>());
        }

        if (IsSuccess)
        {
            if (typeof(TValue) == typeof(Unit))
            {
                return Result<TValue, TNewError>.CreateUnsafe(Value, true, Status, [])
                    .WithAdditionalMetadata(Metadata);
            }

            return Result<TValue, TNewError>.Success(Value).WithAdditionalMetadata(Metadata);
        }

        var mappedErrors = Errors.Select(mapper).ToList();
        return Result<TValue, TNewError>.Failure(mappedErrors).WithAdditionalMetadata(Metadata);
    }

    #endregion

    #region Recover

    public Result<TValue, TError> Recover(Func<IReadOnlyList<TError>, TValue>? recovery)
    {
        if (recovery is null || IsSuccess)
        {
            return this;
        }

        return Success(recovery(Errors)).WithAdditionalMetadata(Metadata);
    }

    public Result<TValue, TError> Recover(
        Func<IReadOnlyList<TError>, Result<TValue, TError>>? recovery)
    {
        if (recovery is null || IsSuccess)
        {
            return this;
        }

        return recovery(Errors).WithAdditionalMetadata(Metadata);
    }

    public Result<TValue, TError> Recover(
        Func<List<TError>, Result<TValue, TError>>? recovery)
    {
        if (recovery is null || IsSuccess)
        {
            return this;
        }

        return recovery(Errors).WithAdditionalMetadata(Metadata);
    }

    #endregion

    #region FailIf

    public Result<TValue, TError> FailIf(Func<TValue, bool>? predicate, TError? error)
    {
        if (predicate is null || error is null || IsFailure)
        {
            return this;
        }

        return predicate(Value) ? Failure(error) : this;
    }

    public Result<TValue, TError> FailIf(
        Func<TValue, bool>? predicate,
        Func<TValue, TError>? errorFactory)
    {
        if (predicate is null || errorFactory is null || IsFailure)
        {
            return this;
        }

        return predicate(Value) ? Failure(errorFactory(Value)) : this;
    }

    public Result<TValue, TError> FailIf(Func<bool>? predicate, TError? error)
    {
        if (predicate is null || error is null || IsFailure)
        {
            return this;
        }

        return predicate() ? Failure(error) : this;
    }

    public Result<TValue, TError> FailIf(Func<bool>? predicate, Func<TError>? errorFactory)
    {
        if (predicate is null || errorFactory is null || IsFailure)
        {
            return this;
        }

        return predicate() ? Failure(errorFactory()) : this;
    }

    #endregion

    #region Ensure

    public Result<TValue, TError> Ensure(Func<TValue, bool>? predicate, TError? error)
    {
        if (predicate is null || error is null || IsFailure)
        {
            return this;
        }

        return predicate(Value) ? this : Failure(error);
    }

    public Result<TValue, TError> Ensure(
        Func<TValue, Result<TValue, TError>>? validator)
    {
        if (validator is null || IsFailure)
        {
            return this;
        }

        var result = validator(Value);
        return result.IsFailure ? result : this;
    }

    public Result<TValue, TError> Ensure(Func<bool>? predicate, TError? error)
    {
        if (predicate is null || error is null || IsFailure)
        {
            return this;
        }

        return predicate() ? this : Failure(error);
    }

    public Result<TValue, TError> Ensure(Func<Result<TValue, TError>>? validator)
    {
        if (validator is null || IsFailure)
        {
            return this;
        }

        var result = validator();
        return result.IsFailure ? result : this;
    }

    #endregion

    #region Switch

    public void Switch(
        Action<TValue>? onSuccess,
        Action<IReadOnlyList<TError>>? onFailure)
    {
        if (IsFailure)
        {
            onFailure?.Invoke(Errors);
        }
        else if (HasValue)
        {
            onSuccess?.Invoke(Value);
        }
    }

    public void Switch(Action? onSuccess, Action<List<TError>>? onFailure)
    {
        if (IsSuccess)
        {
            onSuccess?.Invoke();
        }
        else
        {
            onFailure?.Invoke(Errors);
        }
    }

    #endregion
}
