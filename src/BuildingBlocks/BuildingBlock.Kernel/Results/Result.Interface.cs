using System.Diagnostics.CodeAnalysis;

using BuildingBlock.Kernel.Errors;
using BuildingBlock.Kernel.Metadata;

namespace BuildingBlock.Kernel.Results;

#region Result

public interface IResult : IMetadata
{
    #region State

    bool HasValue { get; }
    bool IsSuccess { get; }
    bool IsFailure { get; }
    int StatusCode { get; set; }

    #endregion

    #region Errors

    IReadOnlyList<IError> Errors { get; }

    #endregion
}

#endregion

#region Result<TValue>

public interface IResult<TValue> : IResult
{
    #region Value

[AllowNull]
    TValue Value { get; }

    bool TryGetValue(
        [MaybeNullWhen(false)] out TValue value);

    #endregion
}

#endregion
