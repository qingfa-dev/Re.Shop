namespace BuildingBlock.Kernel.Metadata;

public sealed class MetadataDictionary : Dictionary<string, object>, IMetadataDictionary
{
    #region Constructors

    public MetadataDictionary()
        : base(StringComparer.OrdinalIgnoreCase)
    {
    }

    public MetadataDictionary(IDictionary<string, object> dictionary)
        : base(
            EnsureNotNull(dictionary),
            StringComparer.OrdinalIgnoreCase)
    {
    }

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

    public static MetadataDictionary Create()
        => new();

    public static MetadataDictionary Create(IDictionary<string, object> dictionary)
        => new(dictionary);

    public static MetadataDictionary Create(IEnumerable<KeyValuePair<string, object>> pairs)
        => new(pairs);

    /// <summary>
    /// A shared, empty, immutable-by-convention instance. Callers must treat it as read-only;
    /// use <see cref="Create()"/> when you intend to mutate.
    /// </summary>
    public static MetadataDictionary Empty { get; } = new();

    /// <summary>
    /// Fluent add/overwrite. Returns <c>this</c> so callers can chain:
    /// <c>MetadataDictionary.Create().With("a", 1).With("b", "two")</c>.
    /// </summary>
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