namespace BuildingBlocks.Core.ValueObjects.Identity;

/// <summary>An absolute HTTP or HTTPS URL retained after trimming whitespace.</summary>
/// <param name="Value">The trimmed URL created by <see cref="Create"/>.</param>
public readonly partial record struct Url(string? Value)
{
    /// <summary>Creates a URL within the configured length limit.</summary>
    /// <param name="value">The candidate absolute HTTP or HTTPS URL; surrounding whitespace is removed.</param>
    /// <returns>The trimmed URL, or a validation error.</returns>
    public static Result<Url, Error> Create(string? value)
    {
        var candidate = new Url(value?.Trim());

        return UrlValidator.Validate(candidate);
    }
}

/// <summary>Groups validation errors for <see cref="Url"/> creation.</summary>
public static class UrlResult
{
    /// <summary>Contains URL validation errors.</summary>
    public static class Errors
    {
        #region Validation

        /// <summary>Error returned when the URL is missing.</summary>
        public static Error ValueRequired => Error.Validation(
            code: "Url.Value.Required",
            message: "Url must be specified.");

        /// <summary>Error returned when the URL exceeds its maximum length.</summary>
        public static Error ValueTooLong => Error.Validation(
            code: "Url.Value.TooLong",
            message:
                $"Url cannot exceed {UrlConstant.Constraints.MaxLength} characters.");

        /// <summary>Error returned when the URL is not an absolute HTTP or HTTPS URL.</summary>
        public static Error ValueInvalid => Error.Validation(
            code: "Url.Value.Invalid",
            message: "Url must be an absolute http(s) address.");

        #endregion
    }
}

/// <summary>Defines URL length constraints.</summary>
public static class UrlConstant
{
    /// <summary>URL length limits.</summary>
    public static class Constraints
    {
        /// <summary>Maximum accepted URL length.</summary>
        public const int MaxLength = 2048;
    }
}

/// <summary>Validates URLs without modifying them.</summary>
public static class UrlValidator
{
    /// <summary>
    /// Validates a url without mutating it.
    /// </summary>
    /// <summary>Returns success when the value is an absolute HTTP or HTTPS URL within the length limit.</summary>
    /// <param name="url">The URL to validate.</param>
    /// <returns>The unchanged URL on success, or its validation errors.</returns>
    public static Result<Url, Error> Validate(Url url)
    {
        return Result<Url, Error>.Success(url)
            .Ensure(
                predicate: _ => !string.IsNullOrWhiteSpace(url.Value),
                error: UrlResult.Errors.ValueRequired)

            .Ensure(
                predicate: _ =>
                    url.Value!.Length <= UrlConstant.Constraints.MaxLength,
                error: UrlResult.Errors.ValueTooLong)

            .Ensure(
                predicate: _ =>
                    Uri.TryCreate(url.Value, UriKind.Absolute, out var uri) &&
                    (uri.Scheme == Uri.UriSchemeHttp ||
                     uri.Scheme == Uri.UriSchemeHttps),
                error: UrlResult.Errors.ValueInvalid);
    }
}
