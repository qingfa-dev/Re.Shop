using System.Globalization;
using System.Text;

namespace BuildingBlocks.Domain.Concerns.Content.Sluggable;

public static class SluggableExtensions
{
    /// <summary>
    /// Sets the slug from an already slugified value.
    /// Validation runs before any mutation, so a failure leaves the entity untouched.
    /// </summary>
    public static Result<TValue> SetSlug<TValue>(
        this Result<TValue> result,
        string? slug)
        where TValue : ISluggable
    {
        return result
            .Bind(entity => SluggableValidator.ValidateSlug(entity, slug))
            .Tap(entity =>
            {
                entity.Slug = slug!;
            });
    }

    /// <summary>
    /// Slugifies free text (lowercase, diacritics folded, non-alphanumerics
    /// collapsed to single hyphens) and sets it as the slug.
    /// </summary>
    public static Result<TValue> SetSlugFromText<TValue>(
        this Result<TValue> result,
        string? text)
        where TValue : ISluggable
    {
        return result.SetSlug(Slugify(text));
    }

    private static string Slugify(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var normalized = text
            .ToLowerInvariant()
            .Normalize(NormalizationForm.FormD);

        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) ==
                UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (character is (>= 'a' and <= 'z') or (>= '0' and <= '9'))
            {
                builder.Append(character);
            }
            else if (builder.Length > 0 && builder[^1] != '-')
            {
                builder.Append('-');
            }
        }

        return builder.ToString().Trim('-');
    }
}
