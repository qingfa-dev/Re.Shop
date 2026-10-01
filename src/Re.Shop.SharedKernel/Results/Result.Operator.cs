using SharedKernel.Errors;

namespace SharedKernel.Results;

public partial record Result<TValue, TError>
    where TError : IError
{
    #region Implicit Operators

    public static implicit operator Result<TValue, TError>(TValue value)
    {
        return Success(value);
    }

    public static implicit operator Result<TValue, TError>(TError error)
    {
        return Failure(error);
    }

    public static implicit operator Result<TValue, TError>(List<TError> errors)
    {
        return Failure(errors);
    }

    public static implicit operator Result<TValue, TError>(TError[] errors)
    {
        return Failure(errors);
    }

    #endregion
}
