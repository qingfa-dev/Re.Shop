using System.Text.RegularExpressions;

namespace BuildingBlocks.Domain.Concerns.Content.Sluggable;

public static class SluggableValidator
{
    /// <summary>
    /// Validates the slug that <see cref="SluggableExtensions.SetSlug{TValue}"/>
    /// is about to apply, without mutating the entity.
    /// </summary>
    public static Result<TValue> ValidateSlug<TValue>(
        TValue auditable,
        string? slug)
        where TValue : ISluggable
    {
        if (auditable is null)
        {
            return Result<TValue>.Fail(
                SluggableResult.Failure.EntityRequired);
        }

        return Result<TValue>.Success(auditable)
            .Ensure(
                predicate: _ => !string.IsNullOrWhiteSpace(slug),
                errorValue: SluggableResult.Failure.SlugRequired)

            .Ensure(
                predicate: _ =>
                    slug!.Length <= SluggableConstant.Constraints.MaxSlugLength,
                errorValue: SluggableResult.Failure.SlugTooLong)

            .Ensure(
                predicate: _ =>
                    Regex.IsMatch(slug!, SluggableConstant.Patterns.Slug),
                errorValue: SluggableResult.Failure.SlugInvalid);
    }
}
