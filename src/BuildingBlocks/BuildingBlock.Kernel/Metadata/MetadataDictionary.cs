namespace BuildingBlock.Kernel.Metadata;

/// <summary>Case-insensitive metadata dictionary.</summary>
/// <remarks>
/// Callers must treat <see cref="Empty"/> as read-only; use <see cref="Create()"/> when mutation is intended.
/// Key comparison follows <see cref="StringComparer.OrdinalIgnoreCase"/>.
/// </remarks>
public sealed class MetadataDictionary : Dictionary<string, object>, IMetadataDictionary
{
    #region Constructors

    /// <summary>Initializes an empty case-insensitive dictionary.</summary>
    /// <remarks>Equivalent to <see cref="Create()"/> — returns a fresh mutable instance.</remarks>
    public MetadataDictionary()
        : base(StringComparer.OrdinalIgnoreCase)
    {
    }

    /// <summary>Initializes a copy of <paramref name="dictionary"/> with case-insensitive keys.</summary>
    /// <param name="dictionary">The source dictionary to copy. Must not be <c>null</c>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is <c>null</c>.</exception>
    /// <remarks>Entries are inserted in enumeration order; duplicate keys (case-insensitive) keep the last value.</remarks>
    public MetadataDictionary(IDictionary<string, object> dictionary)
        : base(
            EnsureNotNull(dictionary),
            StringComparer.OrdinalIgnoreCase)
    {
    }

    /// <summary>Initializes from <paramref name="pairs"/> with case-insensitive keys.</summary>
    /// <param name="pairs">The key-value pairs to populate. Must not be <c>null</c>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="pairs"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Any key in <paramref name="pairs"/> is <c>null</c>.</exception>
    /// <remarks>Duplicate keys (case-insensitive) keep the last value. Null keys throw immediately.</remarks>
    public MetadataDictionary(IEnumerable<KeyValuePair<string, object>> pairs)
        : base(StringComparer.OrdinalIgnoreCase)
    {
        if (pairs is null)
        {
            throw new ArgumentNullException(
                nameof(pairs),
                MetadataConstant.Result.Failure.Dictionary.Argument.Null.Pattern);
        }

        foreach (var pair in pairs)
        {
            if (pair.Key is null)
            {
                throw new ArgumentException(
                    MetadataConstant.Result.Failure.Key.Argument.Null.Pattern,
                    nameof(pairs));
            }

            this[pair.Key] = pair.Value;
        }
    }

    #endregion

    #region Factory Methods

    /// <summary>Creates a fresh empty instance.</summary>
    /// <returns>A new mutable <see cref="MetadataDictionary"/>.</returns>
    /// <remarks>Prefer over the constructor for fluent chains; each call returns a distinct instance.</remarks>
    public static MetadataDictionary Create()
        => new();

    /// <summary>Creates a copy of <paramref name="dictionary"/>.</summary>
    /// <param name="dictionary">The source dictionary to copy. Must not be <c>null</c>.</param>
    /// <returns>A new <see cref="MetadataDictionary"/> with the same entries.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is <c>null</c>.</exception>
    public static MetadataDictionary Create(IDictionary<string, object> dictionary)
        => new(dictionary);

    /// <summary>Creates an instance populated from <paramref name="pairs"/>.</summary>
    /// <param name="pairs">The key-value pairs to populate. Must not be <c>null</c>.</param>
    /// <returns>A new <see cref="MetadataDictionary"/> with the given entries.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pairs"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Any key in <paramref name="pairs"/> is <c>null</c>.</exception>
    public static MetadataDictionary Create(IEnumerable<KeyValuePair<string, object>> pairs)
        => new(pairs);

    /// <summary>A shared, empty, immutable-by-convention instance.</summary>
    /// <value>A singleton <see cref="MetadataDictionary"/> with zero entries.</value>
    /// <remarks>
    /// Callers must treat it as read-only; use <see cref="Create()"/> when you intend to mutate.
    /// The same instance is returned on every access.
    /// </remarks>
    public static MetadataDictionary Empty { get; } = new();

    /// <summary>Fluent add/overwrite. Returns <c>this</c> so callers can chain.</summary>
    /// <param name="key">The metadata key. Must not be <c>null</c>.</param>
    /// <param name="value">The value to store. Must not be <c>null</c>.</param>
    /// <returns>This instance, for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> or <paramref name="value"/> is <c>null</c>.</exception>
    /// <exception cref="NotSupportedException">The backing dictionary is immutable.</exception>
    /// <remarks>
    /// Example: <c>MetadataDictionary.Create().With("a", 1).With("b", "two")</c>.
    /// Case-insensitive key matching follows <see cref="StringComparer.OrdinalIgnoreCase"/>.
    /// </remarks>
    public MetadataDictionary With(string key, object value)
    {
        if (key is null)
        {
            throw new ArgumentNullException(
                nameof(key),
                MetadataConstant.Result.Failure.Key.Argument.Null.Pattern);
        }

        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        this[key] = value;
        return this;
    }

    #endregion

    #region Helpers

    /// <summary>Validates that <paramref name="dictionary"/> is not <c>null</c>.</summary>
    /// <param name="dictionary">The dictionary to validate.</param>
    /// <returns><paramref name="dictionary"/> when non-null.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is <c>null</c>.</exception>
    private static IDictionary<string, object> EnsureNotNull(IDictionary<string, object> dictionary)
    {
        if (dictionary is null)
        {
            throw new ArgumentNullException(
                nameof(dictionary),
                MetadataConstant.Result.Failure.Dictionary.Argument.Null.Pattern);
        }

        return dictionary;
    }

    #endregion
}