using System.ComponentModel;
using System.Globalization;

namespace BuildingBlock.Kernel.Metadata;

/// <summary>Extension methods for <see cref="IMetadata"/> typed access, conversion, and mutation.</summary>
/// <remarks>
/// Organized by capability: raw access, typed access, required access, try-get, presence/mutation,
/// reflective conversion, and well-known key accessors.
/// </remarks>
public static class MetadataExtension
{
    #region Raw access

    /// <summary>Returns the raw <see cref="object"/> value for <paramref name="key"/> or <c>null</c> if missing.</summary>
    /// <param name="request">The metadata-bearing instance.</param>
    /// <param name="key">The metadata key to look up.</param>
    /// <returns>The raw value, or <c>null</c> when <paramref name="key"/> is absent.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> or <paramref name="key"/> is <c>null</c>.</exception>
    public static object? GetValueOrDefault<T>(this T request, string key)
        where T : IMetadata
    {
        if (request is null)
        {
            throw new ArgumentNullException(
                nameof(request),
                MetadataConstant.Result.Failure.Request.Argument.Null.Pattern);
        }

        if (key is null)
        {
            throw new ArgumentNullException(
                nameof(key),
                MetadataConstant.Result.Failure.Key.Argument.Null.Pattern);
        }

        return request.Metadata.TryGetValue(key, out var value) ? value : null;
    }

    #endregion

    #region Typed access

    /// <summary>Returns the reflectively converted value for <paramref name="key"/> or <c>default</c> if missing or not convertible.</summary>
    /// <param name="request">The metadata-bearing instance.</param>
    /// <param name="key">The metadata key to look up.</param>
    /// <typeparam name="T">The metadata-bearing type.</typeparam>
    /// <typeparam name="TValue">The target value type.</typeparam>
    /// <returns>The converted value, or <c>default(TValue)</c> when missing or not convertible.</returns>
    public static TValue? GetValueOrDefault<T, TValue>(this T request, string key)
        where T : IMetadata
    {
        var raw = request.GetValueOrDefault(key);
        return TryConvertValue(raw, out TValue? converted) ? converted : default;
    }

    /// <summary>Returns the reflectively converted value for <paramref name="key"/> or <paramref name="fallback"/> if missing or not convertible.</summary>
    /// <param name="request">The metadata-bearing instance.</param>
    /// <param name="key">The metadata key to look up.</param>
    /// <param name="fallback">Value to return when the key is missing or conversion fails.</param>
    /// <typeparam name="T">The metadata-bearing type.</typeparam>
    /// <typeparam name="TValue">The target value type.</typeparam>
    /// <returns>The converted value, or <paramref name="fallback"/> when missing or not convertible.</returns>
    public static TValue? GetValueOrDefault<T, TValue>(
        this T request,
        string key,
        TValue? fallback)
        where T : IMetadata
    {
        var raw = request.GetValueOrDefault(key);
        return TryConvertValue(raw, out TValue? converted) ? converted : fallback;
    }

    #endregion

    #region Required access

    /// <summary>Returns the raw value for <paramref name="key"/> or throws if missing.</summary>
    /// <param name="request">The metadata-bearing instance.</param>
    /// <param name="key">The metadata key to look up.</param>
    /// <typeparam name="T">The metadata-bearing type.</typeparam>
    /// <returns>The raw value.</returns>
    /// <exception cref="KeyNotFoundException">The key is not present in the metadata bag.</exception>
    public static object GetRequiredValue<T>(this T request, string key)
        where T : IMetadata
    {
        var value = request.GetValueOrDefault(key);

        if (value is null)
        {
            throw new KeyNotFoundException(
                MetadataConstant.Result.Failure.Key.Metadata.NotFound.Pattern);
        }

        return value;
    }

    /// <summary>Returns the reflectively converted value for <paramref name="key"/> or throws if missing or not convertible.</summary>
    /// <param name="request">The metadata-bearing instance.</param>
    /// <param name="key">The metadata key to look up.</param>
    /// <typeparam name="T">The metadata-bearing type.</typeparam>
    /// <typeparam name="TValue">The target value type.</typeparam>
    /// <returns>The converted value.</returns>
    /// <exception cref="KeyNotFoundException">The key is not present in the metadata bag.</exception>
    /// <exception cref="InvalidCastException">The value cannot be cast to <typeparamref name="TValue"/>.</exception>
    public static TValue GetRequiredValue<T, TValue>(this T request, string key)
        where T : IMetadata
    {
        var raw = request.GetValueOrDefault(key);

        if (raw is null)
        {
            throw new KeyNotFoundException(
                MetadataConstant.Result.Failure.Key.Metadata.NotFound.Pattern);
        }

        if (!TryConvertValue(raw, out TValue? converted) || converted is null)
        {
            throw new InvalidCastException(
                MetadataConstant.Result.Failure.Value.Metadata.InvalidCast.Pattern);
        }

        return converted;
    }

    #endregion

    #region TryGet

    /// <summary>Tries to get the reflectively converted value for <paramref name="key"/>.</summary>
    /// <param name="request">The metadata-bearing instance.</param>
    /// <param name="key">The metadata key to look up.</param>
    /// <param name="value">Receives the converted value, or <c>default</c> on failure.</param>
    /// <typeparam name="T">The metadata-bearing type.</typeparam>
    /// <typeparam name="TValue">The target value type.</typeparam>
    /// <returns><c>true</c> when the key exists and conversion succeeds; otherwise <c>false</c>.</returns>
    public static bool TryGetValue<T, TValue>(
        this T request,
        string key,
        out TValue? value)
        where T : IMetadata
    {
        var raw = request.GetValueOrDefault(key);

        if (raw is null)
        {
            value = default;
            return false;
        }

        return TryConvertValue(raw, out value);
    }

    #endregion

    #region Presence / mutation

    /// <summary>Returns <c>true</c> if <paramref name="key"/> exists in the metadata bag.</summary>
    /// <param name="request">The metadata-bearing instance.</param>
    /// <param name="key">The metadata key to check.</param>
    /// <typeparam name="T">The metadata-bearing type.</typeparam>
    /// <returns><c>true</c> when the key is present; otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> or <paramref name="key"/> is <c>null</c>.</exception>
    public static bool Contains<T>(this T request, string key)
        where T : IMetadata
    {
        if (request is null)
        {
            throw new ArgumentNullException(
                nameof(request),
                MetadataConstant.Result.Failure.Request.Argument.Null.Pattern);
        }

        if (key is null)
        {
            throw new ArgumentNullException(
                nameof(key),
                MetadataConstant.Result.Failure.Key.Argument.Null.Pattern);
        }

        return request.Metadata.ContainsKey(key);
    }

    /// <summary>Adds or overwrites <paramref name="key"/> with <paramref name="value"/>.</summary>
    /// <param name="request">The metadata-bearing instance.</param>
    /// <param name="key">The metadata key.</param>
    /// <param name="value">The value to store.</param>
    /// <typeparam name="T">The metadata-bearing type.</typeparam>
    /// <exception cref="ArgumentNullException"><paramref name="request"/>, <paramref name="key"/>, or <paramref name="value"/> is <c>null</c>.</exception>
    /// <exception cref="NotSupportedException">The backing dictionary is immutable.</exception>
    /// <remarks>
    /// Uses direct indexer assignment when the dictionary is mutable; otherwise throws.
    /// Case-insensitive key matching follows <see cref="StringComparer.OrdinalIgnoreCase"/>.
    /// </remarks>
    public static void SetValue<T>(this T request, string key, object value)
        where T : IMetadata
    {
        if (request is null)
        {
            throw new ArgumentNullException(
                nameof(request),
                MetadataConstant.Result.Failure.Request.Argument.Null.Pattern);
        }

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

        // Guard: Only mutable dictionaries accept writes; immutable backing throws.
        if (request.Metadata is IDictionary<string, object> mutable)
        {
            mutable[key] = value;
            return;
        }

        throw new NotSupportedException(
            MetadataConstant.Result.Failure.Dictionary.Mutation.NotSupported.Pattern);
    }

    /// <summary>Removes <paramref name="key"/> from the metadata bag.</summary>
    /// <param name="request">The metadata-bearing instance.</param>
    /// <param name="key">The metadata key to remove.</param>
    /// <typeparam name="T">The metadata-bearing type.</typeparam>
    /// <returns><c>true</c> if the key was found and removed; <c>false</c> if missing or the dictionary is immutable.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> or <paramref name="key"/> is <c>null</c>.</exception>
    public static bool Remove<T>(this T request, string key)
        where T : IMetadata
    {
        if (request is null)
        {
            throw new ArgumentNullException(
                nameof(request),
                MetadataConstant.Result.Failure.Request.Argument.Null.Pattern);
        }

        if (key is null)
        {
            throw new ArgumentNullException(
                nameof(key),
                MetadataConstant.Result.Failure.Key.Argument.Null.Pattern);
        }

        // Guard: Immutable dictionary returns false without mutation.
        return request.Metadata is IDictionary<string, object> mutable
               && mutable.Remove(key);
    }

    #endregion

    #region Reflective conversion core

    /// <summary>
    /// Attempts to convert <paramref name="value"/> to <typeparamref name="TValue"/>.
    /// Handles: direct assignability, string round-trips, Guid / DateTime /
    /// DateTimeOffset parsing, enums, <see cref="IConvertible"/> primitives,
    /// nullable unwrapping, and a <see cref="TypeConverter"/> fallback.
    /// </summary>
    /// <param name="value">The raw value to convert.</param>
    /// <param name="result">Receives the converted value, or <c>default</c> on failure.</param>
    /// <typeparam name="TValue">The target value type.</typeparam>
    /// <returns><c>true</c> when conversion succeeds; otherwise <c>false</c>.</returns>
    /// <remarks>
    /// Conversion chain: null check → direct assignability → string parsing → Guid → DateTimeOffset →
    /// DateTime → enum → IConvertible → TypeConverter fallback. No exception is thrown on failure.
    /// </remarks>
    internal static bool TryConvertValue<TValue>(object? value, out TValue? result)
    {
        result = default;

        // Guard: Null into a nullable or reference-typed TValue is a valid default outcome.
        if (value is null)
        {
            return default(TValue) is null;
        }

        // Compute: Fast path for direct assignability before reflection.
        if (value is TValue direct)
        {
            result = direct;
            return true;
        }

        var targetType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
        var sourceType = value.GetType();

        // Parse: String target via ToString().
        if (targetType == typeof(string))
        {
            result = (TValue)(object)Convert.ToString(value, CultureInfo.InvariantCulture)!;
            return true;
        }

        if (targetType == typeof(Guid))
        {
            // Parse: Guid from string; fail closed on invalid format.
            if (value is string gs && Guid.TryParse(gs, out var parsedGuid))
            {
                result = (TValue)(object)parsedGuid;
                return true;
            }

            return false;
        }

        if (targetType == typeof(DateTimeOffset))
        {
            // Parse: DateTimeOffset from string, then from DateTime.
            if (value is string dtoStr &&
                DateTimeOffset.TryParse(dtoStr, CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind, out var parsedDto))
            {
                result = (TValue)(object)parsedDto;
                return true;
            }

            if (value is DateTime dt)
            {
                result = (TValue)(object)new DateTimeOffset(dt, TimeSpan.Zero);
                return true;
            }

            return false;
        }

        if (targetType == typeof(DateTime))
        {
            // Parse: DateTime from string, then from DateTimeOffset.
            if (value is string dtStr &&
                DateTime.TryParse(dtStr, CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind, out var parsedDt))
            {
                result = (TValue)(object)parsedDt;
                return true;
            }

            if (value is DateTimeOffset dto)
            {
                result = (TValue)(object)dto.UtcDateTime;
                return true;
            }

            return false;
        }

        // Parse: Enum name (case-insensitive) or underlying integer; swallow parse failures.
        if (targetType.IsEnum)
        {
            try
            {
                if (value is string enumName)
                {
                    result = (TValue)Enum.Parse(targetType, enumName, ignoreCase: true);
                    return true;
                }

                result = (TValue)Enum.ToObject(targetType, value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Compute: IConvertible round-trip before TypeConverter fallback.
        if (value is IConvertible && typeof(IConvertible).IsAssignableFrom(targetType))
        {
            try
            {
                var converted = Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
                result = (TValue)converted!;
                return true;
            }
            catch
            {
                // Fallback: TypeConverter handles custom types and many BCL value types.
            }
        }

        // TypeConverter fallback — handles custom types and many BCL value types.
        try
        {
            var converter = TypeDescriptor.GetConverter(targetType);
            if (converter.CanConvertFrom(sourceType))
            {
                var converted = converter.ConvertFrom(
                    context: null,
                    culture: CultureInfo.InvariantCulture,
                    value: value);

                if (converted is not null)
                {
                    result = (TValue)converted;
                    return true;
                }
            }
        }
        catch
        {
            // Guard: Conversion failure signalled via return value, not exception.
        }

        return false;
    }

    #endregion

    #region Well-known key accessors

    /// <summary>Gets the correlation ID from metadata.</summary>
    /// <param name="request">The metadata-bearing instance.</param>
    /// <typeparam name="T">The metadata-bearing type.</typeparam>
    /// <returns>The correlation ID, or <c>null</c> if absent.</returns>
    /// <remarks>Uses <see cref="MetadataConstant.Keyword.CorrelationId"/> as the key.</remarks>
    public static Guid? GetCorrelationId<T>(this T request)
        where T : IMetadata
        => request.GetValueOrDefault<T, Guid?>(MetadataConstant.Keyword.CorrelationId);

    /// <summary>Gets the request ID from metadata.</summary>
    /// <param name="request">The metadata-bearing instance.</param>
    /// <typeparam name="T">The metadata-bearing type.</typeparam>
    /// <returns>The request ID, or <c>null</c> if absent.</returns>
    /// <remarks>Uses <see cref="MetadataConstant.Keyword.RequestId"/> as the key.</remarks>
    public static Guid? GetRequestId<T>(this T request)
        where T : IMetadata
        => request.GetValueOrDefault<T, Guid?>(MetadataConstant.Keyword.RequestId);

    /// <summary>Gets the causation ID from metadata.</summary>
    /// <param name="request">The metadata-bearing instance.</param>
    /// <typeparam name="T">The metadata-bearing type.</typeparam>
    /// <returns>The causation ID, or <c>null</c> if absent.</returns>
    /// <remarks>Uses <see cref="MetadataConstant.Keyword.CausationId"/> as the key.</remarks>
    public static Guid? GetCausationId<T>(this T request)
        where T : IMetadata
        => request.GetValueOrDefault<T, Guid?>(MetadataConstant.Keyword.CausationId);

    /// <summary>Gets the event ID from metadata.</summary>
    /// <param name="request">The metadata-bearing instance.</param>
    /// <typeparam name="T">The metadata-bearing type.</typeparam>
    /// <returns>The event ID, or <c>null</c> if absent.</returns>
    /// <remarks>Uses <see cref="MetadataConstant.Keyword.EventId"/> as the key.</remarks>
    public static Guid? GetEventId<T>(this T request)
        where T : IMetadata
        => request.GetValueOrDefault<T, Guid?>(MetadataConstant.Keyword.EventId);

    /// <summary>Gets the aggregate version from metadata.</summary>
    /// <param name="request">The metadata-bearing instance.</param>
    /// <typeparam name="T">The metadata-bearing type.</typeparam>
    /// <returns>The aggregate version, or <c>null</c> if absent.</returns>
    /// <remarks>Uses <see cref="MetadataConstant.Keyword.AggregateVersion"/> as the key.</remarks>
    public static long? GetAggregateVersion<T>(this T request)
        where T : IMetadata
        => request.GetValueOrDefault<T, long?>(MetadataConstant.Keyword.AggregateVersion);

    /// <summary>Gets the timestamp from metadata.</summary>
    /// <param name="request">The metadata-bearing instance.</param>
    /// <typeparam name="T">The metadata-bearing type.</typeparam>
    /// <returns>The timestamp, or <c>null</c> if absent.</returns>
    /// <remarks>Uses <see cref="MetadataConstant.Keyword.Timestamp"/> as the key.</remarks>
    public static DateTimeOffset? GetTimestamp<T>(this T request)
        where T : IMetadata
        => request.GetValueOrDefault<T, DateTimeOffset?>(MetadataConstant.Keyword.Timestamp);

    /// <summary>Gets the member type from metadata.</summary>
    /// <param name="request">The metadata-bearing instance.</param>
    /// <typeparam name="T">The metadata-bearing type.</typeparam>
    /// <returns>The member type string, or <c>null</c> if absent.</returns>
    /// <remarks>Uses <see cref="MetadataConstant.Keyword.MemberType"/> as the key.</remarks>
    public static string? GetMemberType<T>(this T request)
        where T : IMetadata
        => request.GetValueOrDefault<T, string?>(MetadataConstant.Keyword.MemberType);

    #endregion
}