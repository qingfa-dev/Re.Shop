using System.Text.RegularExpressions;

namespace BuildingBlocks.Domain.Concerns.Content.Metafieldable;

public static class MetafieldableValidator
{
    /// <summary>
    /// Validates the metafield that
    /// <see cref="MetafieldableExtensions.SetMetafield{TValue}"/>
    /// is about to apply, without mutating the entity.
    /// </summary>
    public static Result<TValue> ValidateMetafield<TValue>(
        TValue auditable,
        string? @namespace,
        string? key)
        where TValue : IMetafieldable
    {
        if (auditable is null)
        {
            return Result<TValue>.Fail(
                MetafieldableResult.Failure.EntityRequired);
        }

        return Result<TValue>.Success(auditable)
            .Ensure(
                predicate: _ => !string.IsNullOrWhiteSpace(@namespace),
                errorValue: MetafieldableResult.Failure.NamespaceRequired)

            .Ensure(
                predicate: _ =>
                    @namespace!.Length <=
                    MetafieldableConstant.Constraints.MaxNamespaceLength,
                errorValue: MetafieldableResult.Failure.NamespaceTooLong)

            .Ensure(
                predicate: _ =>
                    Regex.IsMatch(
                        @namespace!,
                        MetafieldableConstant.Patterns.SnakeCase),
                errorValue: MetafieldableResult.Failure.NamespaceInvalid)

            .Ensure(
                predicate: _ => !string.IsNullOrWhiteSpace(key),
                errorValue: MetafieldableResult.Failure.KeyRequired)

            .Ensure(
                predicate: _ =>
                    key!.Length <=
                    MetafieldableConstant.Constraints.MaxKeyLength,
                errorValue: MetafieldableResult.Failure.KeyTooLong)

            .Ensure(
                predicate: _ =>
                    Regex.IsMatch(
                        key!,
                        MetafieldableConstant.Patterns.SnakeCase),
                errorValue: MetafieldableResult.Failure.KeyInvalid);
    }

    /// <summary>
    /// Validates that the metafield that
    /// <see cref="MetafieldableExtensions.RemoveMetafield{TValue}"/>
    /// targets is present, without mutating the entity.
    /// </summary>
    public static Result<TValue> ValidateMetafieldRemoval<TValue>(
        TValue auditable,
        string? @namespace,
        string? key)
        where TValue : IMetafieldable
    {
        if (auditable is null)
        {
            return Result<TValue>.Fail(
                MetafieldableResult.Failure.EntityRequired);
        }

        return Result<TValue>.Success(auditable)
            .Ensure(
                predicate: entity =>
                    entity.Metafields.Any(metafield =>
                        metafield.Namespace == @namespace &&
                        metafield.Key == key),
                errorValue: MetafieldableResult.Failure.MetafieldNotFound);
    }
}
