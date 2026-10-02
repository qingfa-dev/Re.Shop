using System.ComponentModel;
using System.Globalization;

namespace BuildingBlock.Kernel.Metadata;

public static class MetadataExtension
{
    // ---------------------------------------------------------------
    // Raw access
    // ---------------------------------------------------------------

    // Get: return either null or the raw value of the specified key
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

    // ---------------------------------------------------------------
    // Typed access with reflective conversion
    // ---------------------------------------------------------------

    // Get: return default or reflectively convert the value to TValue
    public static TValue? GetValueOrDefault<T, TValue>(this T request, string key)
        where T : IMetadata
    {
        var raw = request.GetValueOrDefault(key);
        return TryConvertValue(raw, out TValue? converted) ? converted : default;
    }

    // Get: return the supplied fallback or reflectively convert the value to TValue
    public static TValue? GetValueOrDefault<T, TValue>(
        this T request,
        string key,
        TValue? fallback)
        where T : IMetadata
    {
        var raw = request.GetValueOrDefault(key);
        return TryConvertValue(raw, out TValue? converted) ? converted : fallback;
    }

    // Get: return the raw value or throw if the key is missing
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

    // Get: return the reflectively-converted value or throw if missing / not convertible
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

    // TryGet: reflection-based; returns false if key missing or value not convertible
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

    // ---------------------------------------------------------------
    // Presence / mutation
    // ---------------------------------------------------------------

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

        if (request.Metadata is IDictionary<string, object> mutable)
        {
            mutable[key] = value;
            return;
        }

        throw new NotSupportedException(
            MetadataConstant.Result.Failure.Dictionary.Mutation.NotSupported.Pattern);
    }

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

        return request.Metadata is IDictionary<string, object> mutable
               && mutable.Remove(key);
    }

    // ---------------------------------------------------------------
    // Reflective conversion core
    // ---------------------------------------------------------------

    /// <summary>
    /// Attempts to convert <paramref name="value"/> to <typeparamref name="TValue"/>.
    /// Handles: direct assignability, string round-trips, Guid / DateTime /
    /// DateTimeOffset parsing, enums, <see cref="IConvertible"/> primitives,
    /// nullable unwrapping, and a <see cref="TypeConverter"/> fallback.
    /// </summary>
    internal static bool TryConvertValue<TValue>(object? value, out TValue? result)
    {
        result = default;

        if (value is null)
        {
            // Null into a nullable or reference-typed TValue is a valid "default" outcome.
            return default(TValue) is null;
        }

        // 1. Fast path — direct assignability (covers TValue, object, interfaces).
        if (value is TValue direct)
        {
            result = direct;
            return true;
        }

        var targetType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
        var sourceType = value.GetType();

        // 2. String target — anything can render as a string via ToString().
        if (targetType == typeof(string))
        {
            result = (TValue)(object)Convert.ToString(value, CultureInfo.InvariantCulture)!;
            return true;
        }

        // 3. Guid.
        if (targetType == typeof(Guid))
        {
            if (value is string gs && Guid.TryParse(gs, out var parsedGuid))
            {
                result = (TValue)(object)parsedGuid;
                return true;
            }

            return false;
        }

        // 4. DateTime / DateTimeOffset.
        if (targetType == typeof(DateTimeOffset))
        {
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

        // 5. Enums — accept name or underlying numeric.
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

        // 6. Primitive / IConvertible round-trip.
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
                // Fall through to TypeConverter.
            }
        }

        // 7. TypeConverter fallback — handles custom types and many BCL value types.
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
            // Swallow — conversion failure is signalled via the return value.
        }

        return false;
    }

    // ---------------------------------------------------------------
    // Typed convenience accessors for well-known metadata keys
    // ---------------------------------------------------------------

    public static Guid? GetCorrelationId<T>(this T request)
        where T : IMetadata
        => request.GetValueOrDefault<T, Guid?>(MetadataConstant.Keyword.CorrelationId);

    public static Guid? GetRequestId<T>(this T request)
        where T : IMetadata
        => request.GetValueOrDefault<T, Guid?>(MetadataConstant.Keyword.RequestId);

    public static Guid? GetCausationId<T>(this T request)
        where T : IMetadata
        => request.GetValueOrDefault<T, Guid?>(MetadataConstant.Keyword.CausationId);

    public static Guid? GetEventId<T>(this T request)
        where T : IMetadata
        => request.GetValueOrDefault<T, Guid?>(MetadataConstant.Keyword.EventId);

    public static long? GetAggregateVersion<T>(this T request)
        where T : IMetadata
        => request.GetValueOrDefault<T, long?>(MetadataConstant.Keyword.AggregateVersion);

    public static DateTimeOffset? GetTimestamp<T>(this T request)
        where T : IMetadata
        => request.GetValueOrDefault<T, DateTimeOffset?>(MetadataConstant.Keyword.Timestamp);

    public static string? GetMemberType<T>(this T request)
        where T : IMetadata
        => request.GetValueOrDefault<T, string?>(MetadataConstant.Keyword.MemberType);
}