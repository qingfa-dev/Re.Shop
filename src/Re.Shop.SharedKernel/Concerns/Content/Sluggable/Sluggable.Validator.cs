using System.Text.RegularExpressions;

namespace SharedKernel.Concerns.Content.Sluggable;

public static class SluggableValidator
{
    /// <summary>
    /// Validates the slug that <see cref="SluggableExtensions.SetSlug{TValue}"/>
    /// is about to apply, without mutating the entity.
    /// </summary>
    public static Result<TValue, Error> ValidateSlug<TValue>(
        TValue auditable,
        string? slug)
        where TValue : ISluggable
    {
        if (auditable is null)
        {
            return Result<TValue, Error>.Failure(
                SluggableResult.Failure.EntityRequired);
        }

        return Result<TValue, Error>.Success(auditable)
            .Ensure(
                predicate: _ => !string.IsNullOrWhiteSpace(slug),
                error: SluggableResult.Failure.SlugRequired)

            .Ensure(
                predicate: _ =>
                    slug!.Length <= SluggableConstant.Constraints.MaxSlugLength,
                error: SluggableResult.Failure.SlugTooLong)

            .Ensure(
                predicate: _ =>
                    Regex.IsMatch(slug!, SluggableConstant.Patterns.Slug),
                error: SluggableResult.Failure.SlugInvalid);
    }
}
