using System.Diagnostics.CodeAnalysis;

using BuildingBlock.Kernel.Errors;
using BuildingBlock.Kernel.Metadata;

namespace BuildingBlock.Kernel.Results;

/// <summary>Base result contract exposing state, errors, and metadata.</summary>
#region Result

public interface IResult<TError> : IMetadata
where TError : IError
{
    #region State

    /// <summary>Gets a value indicating whether the result is a success.</summary>
    bool HasValue { get; }
    /// <summary>Gets a value indicating whether the result is a success.</summary>
    bool IsSuccess { get; }
    /// <summary>Gets a value indicating whether the result is a failure.</summary>
    bool IsFailure { get; }
    /// <summary>Gets or sets the HTTP status code.</summary>
    int StatusCode { get; set; }

    #endregion

    #region Errors

    /// <summary>Gets the error list associated with the result.</summary>
    List<TError> Errors { get; }

    #endregion
}

#endregion

/// <summary>Generic result contract adding value access.</summary>
#region Result<TValue>

public interface IResult<TValue, TError> : IResult<TError>
    where TError : IError
{
    #region Value

    /// <summary>Gets the result value when successful; undefined on failure.</summary>
    [AllowNull]
    TValue Value { get; }

    /// <summary>Tries to get the value when successful.</summary>
    /// <param name="value">Receives the value when successful; <see langword="default"/> otherwise.</param>
    /// <returns><see langword="true"/> when the result is a success; otherwise <see langword="false"/>.</returns>
    bool TryGetValue(
        [MaybeNullWhen(false)] out TValue value);

    #endregion
}
#endregion

/// <summary>Contract for failure factory methods.</summary>
public interface IResultFailure<TSelf, TError>
    where TSelf : IResultFailure<TSelf, TError>
    where TError : IError
{
    /// <summary>Creates a failed result from the supplied errors.</summary>
    /// <param name="errors">The errors to attach.</param>
    /// <returns>A failed result.</returns>
    static abstract TSelf Fail(IEnumerable<TError> errors);
}
