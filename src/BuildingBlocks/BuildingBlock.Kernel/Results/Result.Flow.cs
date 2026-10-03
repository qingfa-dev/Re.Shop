using BuildingBlock.Kernel.Errors;

namespace BuildingBlock.Kernel.Results;

/// <summary>Provides synchronous and asynchronous functional operations for results.</summary>
public static partial class ResultExtension
{
    #region Match

    /// <summary>Invokes the selected callback, or returns the default value when it is omitted.</summary>
    public static TOut? Match<TOut>(
        this Result result,
        Func<TOut>? onSuccess,
        Func<IReadOnlyList<IError>, TOut>? onFailure)
    {
        ArgumentNullException.ThrowIfNull(result);
        return result.IsSuccess ? onSuccess is null ? default! : onSuccess()
            : onFailure is null ? default! : onFailure(result.Errors);
    }

    /// <summary>Invokes the selected callback, or returns the default value when it is omitted.</summary>
    public static TOut? Match<TValue, TOut>(
        this Result<TValue> result,
        Func<TValue, TOut>? onSuccess,
        Func<IReadOnlyList<IError>, TOut>? onFailure)
    {
        ArgumentNullException.ThrowIfNull(result);
        return result.IsSuccess ? onSuccess is null ? default! : onSuccess(result.Value)
            : onFailure is null ? default! : onFailure(result.Errors);
    }

    /// <summary>Asynchronously invokes the selected callback, or returns the default value when it is omitted.</summary>
    public static async Task<TOut?> MatchAsync<TOut>(
        this Result result,
        Func<CancellationToken, Task<TOut>>? onSuccess,
        Func<IReadOnlyList<IError>, CancellationToken, Task<TOut>>? onFailure,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsSuccess)
        {
            var task = onSuccess?.Invoke(cancellationToken);
            return task is null ? default! : await task.ConfigureAwait(false);
        }

        var failureTask = onFailure?.Invoke(result.Errors, cancellationToken);
        return failureTask is null
            ? default!
            : await failureTask.ConfigureAwait(false);
    }

    /// <summary>Asynchronously invokes the selected callback, or returns the default value when it is omitted.</summary>
    public static async Task<TOut?> MatchAsync<TValue, TOut>(
        this Result<TValue> result,
        Func<TValue, CancellationToken, Task<TOut>>? onSuccess,
        Func<IReadOnlyList<IError>, CancellationToken, Task<TOut>>? onFailure,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsSuccess)
        {
            var task = onSuccess?.Invoke(result.Value, cancellationToken);
            return task is null ? default! : await task.ConfigureAwait(false);
        }

        var failureTask = onFailure?.Invoke(result.Errors, cancellationToken);
        return failureTask is null
            ? default!
            : await failureTask.ConfigureAwait(false);
    }

    #endregion

    #region Map

    /// <summary>Maps a successful value-less result; a missing mapper returns a failed result.</summary>
    public static Result<TOut> Map<TOut>(this Result result, Func<TOut>? mapper)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsFailure)
            return Result<TOut>.Fail(result.Errors, result.StatusCode);
        if (mapper is null)
            return Result<TOut>.Fail(ResultError.Flow.CallbackMissing(nameof(Map)));

        return Result<TOut>.Ok(mapper(), result.StatusCode);
    }

    /// <summary>Maps a successful value; a missing mapper returns a failed result.</summary>
    public static Result<TOut> Map<TValue, TOut>(
        this Result<TValue> result,
        Func<TValue, TOut>? mapper)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsFailure)
            return Result<TOut>.Fail(result.Errors, result.StatusCode);
        if (mapper is null)
            return Result<TOut>.Fail(ResultError.Flow.CallbackMissing(nameof(Map)));

        return Result<TOut>.Ok(mapper(result.Value), result.StatusCode);
    }

    /// <summary>Asynchronously maps a successful value-less result.</summary>
    public static async Task<Result<TOut>> MapAsync<TOut>(
        this Result result,
        Func<CancellationToken, Task<TOut>>? mapper,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsFailure)
            return Result<TOut>.Fail(result.Errors, result.StatusCode);
        if (mapper is null)
            return Result<TOut>.Fail(ResultError.Flow.CallbackMissing(nameof(MapAsync)));

        var task = mapper(cancellationToken);
        if (task is null)
            return Result<TOut>.Fail(ResultError.Flow.ResultMissing(nameof(MapAsync)));
        return Result<TOut>.Ok(await task.ConfigureAwait(false), result.StatusCode);
    }

    /// <summary>Asynchronously maps a successful value.</summary>
    public static async Task<Result<TOut>> MapAsync<TValue, TOut>(
        this Result<TValue> result,
        Func<TValue, CancellationToken, Task<TOut>>? mapper,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsFailure)
            return Result<TOut>.Fail(result.Errors, result.StatusCode);
        if (mapper is null)
            return Result<TOut>.Fail(ResultError.Flow.CallbackMissing(nameof(MapAsync)));

        var task = mapper(result.Value, cancellationToken);
        if (task is null)
            return Result<TOut>.Fail(ResultError.Flow.ResultMissing(nameof(MapAsync)));
        return Result<TOut>.Ok(await task.ConfigureAwait(false), result.StatusCode);
    }

    #endregion

    #region Bind

    /// <summary>Binds a successful value to a non-generic result operation.</summary>
    public static Result Bind<TValue>(this Result<TValue> result, Func<TValue, Result>? binder)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsFailure)
            return Result.Failure(result.Errors, result.StatusCode);
        if (binder is null)
            return Result.Fail(ResultError.Flow.CallbackMissing(nameof(Bind)));

        return binder(result.Value)
            ?? Result.Fail(ResultError.Flow.ResultMissing(nameof(Bind)));
    }

    /// <summary>Binds a successful value to a generic result operation.</summary>
    public static Result<TOut> Bind<TValue, TOut>(
        this Result<TValue> result,
        Func<TValue, Result<TOut>>? binder)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsFailure)
            return Result<TOut>.Fail(result.Errors, result.StatusCode);
        if (binder is null)
            return Result<TOut>.Fail(ResultError.Flow.CallbackMissing(nameof(Bind)));

        return binder(result.Value)
            ?? Result<TOut>.Fail(ResultError.Flow.ResultMissing(nameof(Bind)));
    }

    /// <summary>Asynchronously binds a successful value to a non-generic result operation.</summary>
    public static async Task<Result> BindAsync<TValue>(
        this Result<TValue> result,
        Func<TValue, CancellationToken, Task<Result>>? binder,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsFailure)
            return Result.Failure(result.Errors, result.StatusCode);
        if (binder is null)
            return Result.Fail(ResultError.Flow.CallbackMissing(nameof(BindAsync)));

        var task = binder(result.Value, cancellationToken);
        if (task is null)
            return Result.Fail(ResultError.Flow.ResultMissing(nameof(BindAsync)));
        return await task.ConfigureAwait(false)
            ?? Result.Fail(ResultError.Flow.ResultMissing(nameof(BindAsync)));
    }

    /// <summary>Asynchronously binds a successful value to a generic result operation.</summary>
    public static async Task<Result<TOut>> BindAsync<TValue, TOut>(
        this Result<TValue> result,
        Func<TValue, CancellationToken, Task<Result<TOut>>>? binder,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsFailure)
            return Result<TOut>.Fail(result.Errors, result.StatusCode);
        if (binder is null)
            return Result<TOut>.Fail(ResultError.Flow.CallbackMissing(nameof(BindAsync)));

        var task = binder(result.Value, cancellationToken);
        if (task is null)
            return Result<TOut>.Fail(ResultError.Flow.ResultMissing(nameof(BindAsync)));
        return await task.ConfigureAwait(false)
            ?? Result<TOut>.Fail(ResultError.Flow.ResultMissing(nameof(BindAsync)));
    }

    #endregion

    #region Ensure

    /// <summary>Ensures the successful value satisfies a predicate, using a supplied error on violation.</summary>
    public static Result<TValue> Ensure<TValue>(
        this Result<TValue> result,
        Func<TValue, bool>? predicate,
        IError? errorValue)
        => EnsureCore(result, predicate, errorValue is null ? null : _ => errorValue);

    /// <summary>Ensures the successful value satisfies a predicate, using an error factory on violation.</summary>
    public static Result<TValue> Ensure<TValue>(
        this Result<TValue> result,
        Func<TValue, bool>? predicate,
        Func<IError>? error)
        => EnsureCore(result, predicate, error is null ? null : _ => error());

    /// <summary>Ensures the successful value satisfies a predicate, using a value-aware error factory on violation.</summary>
    public static Result<TValue> Ensure<TValue>(
        this Result<TValue> result,
        Func<TValue, bool>? predicate,
        Func<TValue, IError>? onViolation)
        => EnsureCore(result, predicate, onViolation);

    /// <summary>Asynchronously ensures the successful value satisfies a predicate.</summary>
    public static Task<Result<TValue>> EnsureAsync<TValue>(
        this Result<TValue> result,
        Func<TValue, CancellationToken, Task<bool>>? predicate,
        IError? errorValue,
        CancellationToken cancellationToken = default)
        => EnsureAsyncCore(
            result,
            predicate,
            errorValue is null ? null : (_, _) => Task.FromResult(errorValue),
            cancellationToken);

    /// <summary>Asynchronously ensures the successful value satisfies a predicate, using an error factory on violation.</summary>
    public static Task<Result<TValue>> EnsureAsync<TValue>(
        this Result<TValue> result,
        Func<TValue, CancellationToken, Task<bool>>? predicate,
        Func<IError>? error,
        CancellationToken cancellationToken = default)
        => EnsureAsyncCore(
            result,
            predicate,
            error is null ? null : (_, _) => Task.FromResult(error()),
            cancellationToken);

    /// <summary>Asynchronously ensures the successful value satisfies a predicate, using a synchronous value-aware error factory.</summary>
    public static Task<Result<TValue>> EnsureAsync<TValue>(
        this Result<TValue> result,
        Func<TValue, CancellationToken, Task<bool>>? predicate,
        Func<TValue, IError>? onViolation,
        CancellationToken cancellationToken = default)
        => EnsureAsyncCore(
            result,
            predicate,
            onViolation is null ? null : (value, _) => Task.FromResult(onViolation(value)),
            cancellationToken);

    /// <summary>Asynchronously ensures the successful value satisfies a predicate, using an asynchronous error factory on violation.</summary>
    public static Task<Result<TValue>> EnsureAsync<TValue>(
        this Result<TValue> result,
        Func<TValue, CancellationToken, Task<bool>>? predicate,
        Func<TValue, CancellationToken, Task<IError>>? onViolation,
        CancellationToken cancellationToken = default)
        => EnsureAsyncCore(result, predicate, onViolation, cancellationToken);

    private static Result<TValue> EnsureCore<TValue>(
        Result<TValue> result,
        Func<TValue, bool>? predicate,
        Func<TValue, IError?>? errorFactory)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsFailure)
            return result;
        if (predicate is null)
            return Result<TValue>.Fail(ResultError.Flow.CallbackMissing(nameof(Ensure)));
        if (predicate(result.Value))
            return result;
        if (errorFactory is null)
            return Result<TValue>.Fail(ResultError.Flow.CallbackMissing("Ensure error"));

        var error = errorFactory(result.Value);
        return error is null
            ? Result<TValue>.Fail(ResultError.Flow.ErrorMissing(nameof(Ensure)))
            : Result<TValue>.Fail(error);
    }

    private static async Task<Result<TValue>> EnsureAsyncCore<TValue>(
        Result<TValue> result,
        Func<TValue, CancellationToken, Task<bool>>? predicate,
        Func<TValue, CancellationToken, Task<IError>>? errorFactory,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsFailure)
            return result;
        if (predicate is null)
            return Result<TValue>.Fail(ResultError.Flow.CallbackMissing(nameof(EnsureAsync)));

        var predicateTask = predicate(result.Value, cancellationToken);
        if (predicateTask is null)
            return Result<TValue>.Fail(ResultError.Flow.ResultMissing(nameof(EnsureAsync)));
        if (await predicateTask.ConfigureAwait(false))
            return result;
        if (errorFactory is null)
            return Result<TValue>.Fail(ResultError.Flow.CallbackMissing("EnsureAsync error"));

        var errorTask = errorFactory(result.Value, cancellationToken);
        if (errorTask is null)
            return Result<TValue>.Fail(ResultError.Flow.ResultMissing("EnsureAsync error"));
        var error = await errorTask.ConfigureAwait(false);
        return error is null
            ? Result<TValue>.Fail(ResultError.Flow.ErrorMissing(nameof(EnsureAsync)))
            : Result<TValue>.Fail(error);
    }

    #endregion

    #region Recover

    /// <summary>Replaces a failed non-generic result using its errors.</summary>
    public static Result Recover(
        this Result result,
        Func<IReadOnlyList<IError>, Result>? recovery)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsSuccess)
            return result;
        if (recovery is null)
            return Result.Fail(ResultError.Flow.CallbackMissing(nameof(Recover)));

        return recovery(result.Errors)
            ?? Result.Fail(ResultError.Flow.ResultMissing(nameof(Recover)));
    }

    /// <summary>Recovers a failed generic result with a value.</summary>
    public static Result<TValue> Recover<TValue>(
        this Result<TValue> result,
        Func<IReadOnlyList<IError>, TValue>? recovery)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsSuccess)
            return result;
        if (recovery is null)
            return Result<TValue>.Fail(ResultError.Flow.CallbackMissing(nameof(Recover)));

        return Result<TValue>.Ok(recovery(result.Errors));
    }

    /// <summary>Recovers a failed generic result with another result.</summary>
    public static Result<TValue> Recover<TValue>(
        this Result<TValue> result,
        Func<IReadOnlyList<IError>, Result<TValue>>? recovery)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsSuccess)
            return result;
        if (recovery is null)
            return Result<TValue>.Fail(ResultError.Flow.CallbackMissing(nameof(Recover)));

        return recovery(result.Errors)
            ?? Result<TValue>.Fail(ResultError.Flow.ResultMissing(nameof(Recover)));
    }

    /// <summary>Asynchronously replaces a failed non-generic result using its errors.</summary>
    public static async Task<Result> RecoverAsync(
        this Result result,
        Func<IReadOnlyList<IError>, CancellationToken, Task<Result>>? recovery,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsSuccess)
            return result;
        if (recovery is null)
            return Result.Fail(ResultError.Flow.CallbackMissing(nameof(RecoverAsync)));

        var task = recovery(result.Errors, cancellationToken);
        if (task is null)
            return Result.Fail(ResultError.Flow.ResultMissing(nameof(RecoverAsync)));
        return await task.ConfigureAwait(false)
            ?? Result.Fail(ResultError.Flow.ResultMissing(nameof(RecoverAsync)));
    }

    /// <summary>Asynchronously recovers a failed generic result with a value.</summary>
    public static async Task<Result<TValue>> RecoverAsync<TValue>(
        this Result<TValue> result,
        Func<IReadOnlyList<IError>, CancellationToken, Task<TValue>>? recovery,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsSuccess)
            return result;
        if (recovery is null)
            return Result<TValue>.Fail(ResultError.Flow.CallbackMissing(nameof(RecoverAsync)));

        var task = recovery(result.Errors, cancellationToken);
        if (task is null)
            return Result<TValue>.Fail(ResultError.Flow.ResultMissing(nameof(RecoverAsync)));
        return Result<TValue>.Ok(await task.ConfigureAwait(false));
    }

    /// <summary>Asynchronously recovers a failed generic result with another result.</summary>
    public static async Task<Result<TValue>> RecoverAsync<TValue>(
        this Result<TValue> result,
        Func<IReadOnlyList<IError>, CancellationToken, Task<Result<TValue>>>? recovery,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsSuccess)
            return result;
        if (recovery is null)
            return Result<TValue>.Fail(ResultError.Flow.CallbackMissing(nameof(RecoverAsync)));

        var task = recovery(result.Errors, cancellationToken);
        if (task is null)
            return Result<TValue>.Fail(ResultError.Flow.ResultMissing(nameof(RecoverAsync)));
        return await task.ConfigureAwait(false)
            ?? Result<TValue>.Fail(ResultError.Flow.ResultMissing(nameof(RecoverAsync)));
    }

    #endregion

    #region Switch

    /// <summary>Invokes the callback for the current non-generic result state.</summary>
    public static Result Switch(
        this Result result,
        Action? onSuccess = null,
        Action<IReadOnlyList<IError>>? onFailure = null)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsSuccess)
            onSuccess?.Invoke();
        else
            onFailure?.Invoke(result.Errors);
        return result;
    }

    /// <summary>Invokes the callback for the current generic result state.</summary>
    public static Result<TValue> Switch<TValue>(
        this Result<TValue> result,
        Action<TValue>? onSuccess = null,
        Action<IReadOnlyList<IError>>? onFailure = null)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsSuccess)
            onSuccess?.Invoke(result.Value);
        else
            onFailure?.Invoke(result.Errors);
        return result;
    }

    /// <summary>Asynchronously invokes the callback for the current non-generic result state.</summary>
    public static async Task<Result> SwitchAsync(
        this Result result,
        Func<CancellationToken, Task>? onSuccess = null,
        Func<IReadOnlyList<IError>, CancellationToken, Task>? onFailure = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsSuccess && onSuccess is not null)
        {
            var task = onSuccess(cancellationToken);
            if (task is null)
                return Result.Fail(ResultError.Flow.ResultMissing(nameof(SwitchAsync)));
            await task.ConfigureAwait(false);
        }
        else if (result.IsFailure && onFailure is not null)
        {
            var task = onFailure(result.Errors, cancellationToken);
            if (task is null)
                return Result.Fail(ResultError.Flow.ResultMissing(nameof(SwitchAsync)));
            await task.ConfigureAwait(false);
        }
        return result;
    }

    /// <summary>Asynchronously invokes the callback for the current generic result state.</summary>
    public static async Task<Result<TValue>> SwitchAsync<TValue>(
        this Result<TValue> result,
        Func<TValue, CancellationToken, Task>? onSuccess = null,
        Func<IReadOnlyList<IError>, CancellationToken, Task>? onFailure = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsSuccess && onSuccess is not null)
        {
            var task = onSuccess(result.Value, cancellationToken);
            if (task is null)
                return Result<TValue>.Fail(ResultError.Flow.ResultMissing(nameof(SwitchAsync)));
            await task.ConfigureAwait(false);
        }
        else if (result.IsFailure && onFailure is not null)
        {
            var task = onFailure(result.Errors, cancellationToken);
            if (task is null)
                return Result<TValue>.Fail(ResultError.Flow.ResultMissing(nameof(SwitchAsync)));
            await task.ConfigureAwait(false);
        }
        return result;
    }

    #endregion

    #region Tap

    /// <summary>Runs an action for a successful non-generic result and returns the same result.</summary>
    public static Result Tap(this Result result, Action? onSuccess)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsSuccess)
            onSuccess?.Invoke();
        return result;
    }

    /// <summary>Runs an action for a successful generic result and returns the same result.</summary>
    public static Result<TValue> Tap<TValue>(this Result<TValue> result, Action<TValue>? onSuccess)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsSuccess)
            onSuccess?.Invoke(result.Value);
        return result;
    }

    /// <summary>Runs an action for a failed result and returns the same result.</summary>
    public static Result TapError(this Result result, Action<IReadOnlyList<IError>>? onFailure)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsFailure)
            onFailure?.Invoke(result.Errors);
        return result;
    }

    /// <summary>Runs an action for a failed generic result and returns the same result.</summary>
    public static Result<TValue> TapError<TValue>(
        this Result<TValue> result,
        Action<IReadOnlyList<IError>>? onFailure)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsFailure)
            onFailure?.Invoke(result.Errors);
        return result;
    }

    /// <summary>Asynchronously runs an action for a successful non-generic result.</summary>
    public static async Task<Result> TapAsync(
        this Result result,
        Func<CancellationToken, Task>? onSuccess,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsSuccess && onSuccess is not null)
        {
            var task = onSuccess(cancellationToken);
            if (task is null)
                return Result.Fail(ResultError.Flow.ResultMissing(nameof(TapAsync)));
            await task.ConfigureAwait(false);
        }
        return result;
    }

    /// <summary>Asynchronously runs an action for a successful generic result.</summary>
    public static async Task<Result<TValue>> TapAsync<TValue>(
        this Result<TValue> result,
        Func<TValue, CancellationToken, Task>? onSuccess,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsSuccess && onSuccess is not null)
        {
            var task = onSuccess(result.Value, cancellationToken);
            if (task is null)
                return Result<TValue>.Fail(ResultError.Flow.ResultMissing(nameof(TapAsync)));
            await task.ConfigureAwait(false);
        }
        return result;
    }

    /// <summary>Asynchronously runs an action for a failed non-generic result.</summary>
    public static async Task<Result> TapErrorAsync(
        this Result result,
        Func<IReadOnlyList<IError>, CancellationToken, Task>? onFailure,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsFailure && onFailure is not null)
        {
            var task = onFailure(result.Errors, cancellationToken);
            if (task is null)
                return Result.Fail(ResultError.Flow.ResultMissing(nameof(TapErrorAsync)));
            await task.ConfigureAwait(false);
        }
        return result;
    }

    /// <summary>Asynchronously runs an action for a failed generic result.</summary>
    public static async Task<Result<TValue>> TapErrorAsync<TValue>(
        this Result<TValue> result,
        Func<IReadOnlyList<IError>, CancellationToken, Task>? onFailure,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsFailure && onFailure is not null)
        {
            var task = onFailure(result.Errors, cancellationToken);
            if (task is null)
                return Result<TValue>.Fail(ResultError.Flow.ResultMissing(nameof(TapErrorAsync)));
            await task.ConfigureAwait(false);
        }
        return result;
    }

    #endregion

    #region Value Access

    /// <summary>Gets the value when the result succeeds, or throws for a failure.</summary>
    public static TValue ValueOrThrow<TValue>(this Result<TValue> result)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsFailure)
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.ToString())));

        return result.Value;
    }

    /// <summary>Gets the value when the result succeeds, or returns the specified fallback.</summary>
    public static TValue ValueOr<TValue>(this Result<TValue> result, TValue fallback)
    {
        ArgumentNullException.ThrowIfNull(result);
        return result.IsSuccess ? result.Value : fallback;
    }

    #endregion
}
