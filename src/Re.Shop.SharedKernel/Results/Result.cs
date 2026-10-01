using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using SharedKernel.Errors;

namespace SharedKernel.Results;

public partial record Result<TValue, TError> : IResult<TValue, TError>
    where TError : IError
{
    #region Constructor

    [JsonConstructor]
    private Result(
        TValue? value,
        bool isSuccess,
        int status,
        List<TError>? errors)
    {
        Value = value!;
        IsSuccess = isSuccess;
        Status = status;
        Errors = errors ?? [];
        Metadata = [];
    }

    #endregion

    #region Properties

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public int Status { get; }

    [AllowNull]
    public TValue Value { get; }

    [MemberNotNullWhen(true, nameof(Value))]
    public bool HasValue => IsSuccess && Value is not null;

    public List<TError> Errors { get; }

    public Dictionary<string, object?>? Metadata { get; set; }

    #endregion

    #region Methods

    public override string ToString()
    {
        if (IsSuccess)
        {
            if (typeof(TValue) == typeof(Unit))
            {
                return $"Success({Status})";
            }

            return $"Success({Status}, Value: {Value})";
        }

        var errorSummary = Errors.Count > 0
            ? string.Join("; ", Errors.Select(error => $"{error.Code}: {error.Message}"))
            : "None";

        return $"Failure({Status}, Errors({Errors.Count}): [{errorSummary}])";
    }

    #endregion
}
