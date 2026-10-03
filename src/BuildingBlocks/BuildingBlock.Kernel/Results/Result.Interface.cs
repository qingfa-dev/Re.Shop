using System.Diagnostics.CodeAnalysis;

using BuildingBlock.Kernel.Errors;
using BuildingBlock.Kernel.Metadata;

namespace BuildingBlock.Kernel.Results;

#region Result

public interface IResult<TError> : IMetadata
where TError : IError
{
    #region State

    bool HasValue { get; }
    bool IsSuccess { get; }
    bool IsFailure { get; }
    int StatusCode { get; set; }

    #endregion

    #region Errors

    List<TError> Errors { get; }

    #endregion
}

#endregion

#region Result<TValue>

public interface IResult<TValue, TError> : IResult<TError>
    where TError : IError
{
    #region Value

[AllowNull]
    TValue Value { get; }

    bool TryGetValue(
        [MaybeNullWhen(false)] out TValue value);

    #endregion
}
#endregion

public interface IResultFailure<TSelf, TError>
    where TSelf : IResultFailure<TSelf, TError>
    where TError : IError
{
    static abstract TSelf Fail(IEnumerable<TError> errors);
}
