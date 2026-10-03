using BuildingBlock.Kernel.Errors;

namespace BuildingBlock.Kernel.Results;

public partial record Result
{
    #region Implicit Failure Conversions

    /// <summary>Converts an error to a failed result.</summary>
    public static implicit operator Result(Error error)
        => Fail(error);

    /// <summary>Converts an array of errors to a failed result.</summary>
    /// <remarks>
    /// For error collections exposed as <see cref="List{T}"/>, use
    /// <see cref="Result.Fail(IEnumerable{Error})"/> because C# does not permit
    /// implicit conversion operators from interface source types.
    /// </remarks>
    public static implicit operator Result(Error[] errors)
        => Fail(errors);

    #endregion
}
