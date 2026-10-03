using BuildingBlock.Kernel.Metadata;

namespace BuildingBlock.Kernel.Results;

/// <summary>Provides copy-based metadata and status extensions for results.</summary>
public static partial class ResultExtension
{
    #region Non-generic Result

    /// <summary>Returns a copy of the result with the metadata entry added or replaced.</summary>
    /// <param name="result">The result to copy.</param>
    /// <param name="key">The metadata key.</param>
    /// <param name="value">The non-null metadata value.</param>
    /// <returns>A copy containing the metadata entry.</returns>
    public static Result WithMetadata(this Result result, string key, object value)
    {
        ArgumentNullException.ThrowIfNull(result);
        var copy = result.CopyWith();
        copy.SetValue(key, value);
        return copy;
    }

    /// <summary>Returns a copy of the result with the specified status code.</summary>
    /// <param name="result">The result to copy.</param>
    /// <param name="statusCode">The status code to assign.</param>
    /// <returns>A validated copy with the specified status code.</returns>
    public static Result WithStatus(this Result result, int statusCode)
    {
        ArgumentNullException.ThrowIfNull(result);
        return result.CopyWith(statusCode: statusCode);
    }

    #endregion

    #region Generic Result

    /// <summary>Returns a copy of the generic result with the metadata entry added or replaced.</summary>
    /// <typeparam name="TValue">The result value type.</typeparam>
    /// <param name="result">The result to copy.</param>
    /// <param name="key">The metadata key.</param>
    /// <param name="value">The non-null metadata value.</param>
    /// <returns>A copy containing the metadata entry.</returns>
    public static Result<TValue> WithMetadata<TValue>(
        this Result<TValue> result,
        string key,
        object value)
    {
        ArgumentNullException.ThrowIfNull(result);
        var copy = result.CopyWith();
        copy.SetValue(key, value);
        return copy;
    }

    /// <summary>Returns a copy of the generic result with the specified status code.</summary>
    /// <typeparam name="TValue">The result value type.</typeparam>
    /// <param name="result">The result to copy.</param>
    /// <param name="statusCode">The status code to assign.</param>
    /// <returns>A validated copy with the specified status code.</returns>
    public static Result<TValue> WithStatus<TValue>(
        this Result<TValue> result,
        int statusCode)
    {
        ArgumentNullException.ThrowIfNull(result);
        return result.CopyWith(statusCode: statusCode);
    }

    #endregion
}
