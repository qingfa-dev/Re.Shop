using BuildingBlock.Kernel.Errors;

namespace BuildingBlock.Kernel.Results;

public partial record Result<TValue>
{
    #region Implicit Success Conversion

    /// <summary>Converts a value to a successful generic result.</summary>
    public static implicit operator Result<TValue>(TValue value)
        => Ok(value);

    #endregion

    #region Implicit Failure Conversions

    /// <summary>Converts an error to a failed generic result.</summary>
    /// <remarks>
    /// When <typeparamref name="TValue"/> is <see cref="Error"/>, this conversion
    /// conflicts with the value conversion. Use <see cref="Result{TValue}.Fail(Error[])"/>
    /// or <see cref="Result{TValue}.Ok(TValue, int)"/> explicitly in that case.
    /// </remarks>
    public static implicit operator Result<TValue>(Error error)
        => Fail(error);

    /// <summary>Converts an array of errors to a failed generic result.</summary>
    /// <remarks>
    /// Error collections exposed as <see cref="List{T}"/> cannot be
    /// implicitly converted by C#. Use <see cref="Result{TValue}.Fail(IEnumerable{Error})"/>
    /// for those collections.
    /// </remarks>
    public static implicit operator Result<TValue>(Error[] errors)
        => Fail(errors);
    
    /// <summary>Converts a list of errors to a failed generic result.</summary>
    /// <remarks>
    /// Error collections exposed as <see cref="List{T}"/> cannot be
    /// implicitly converted by C#. Use <see cref="Result{TValue}.Fail(IEnumerable{Error})"/> for those collections.
    /// </remarks>
    public static implicit operator Result<TValue>(List<Error> errors)
        => Fail(errors);

    #endregion
}
