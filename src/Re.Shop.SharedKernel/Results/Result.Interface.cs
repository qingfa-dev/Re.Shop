using System.Diagnostics.CodeAnalysis;

using SharedKernel.Errors;
using SharedKernel.Meta;

namespace SharedKernel.Results;

public interface IValueOf<out TValue>
{
    #region Properties

    [AllowNull]
    TValue Value { get; }

    #endregion
}

public interface IResult<TValue, TError> : IMetadata, IValueOf<TValue>
    where TError : IError
{
    #region Properties

    bool IsSuccess { get; }
    bool IsFailure { get; }
    int Status { get; }

    bool HasValue { get; }
    List<TError> Errors { get; }

    #endregion
}
