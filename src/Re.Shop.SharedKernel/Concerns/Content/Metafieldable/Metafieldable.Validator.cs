using System.Text.RegularExpressions;

namespace SharedKernel.Concerns.Content.Metafieldable;

public static class MetafieldableValidator
{
    /// <summary>
    /// Validates the metafield that
    /// <see cref="MetafieldableExtensions.SetMetafield{TValue}"/>
    /// is about to apply, without mutating the entity.
    /// </summary>
    public static Result<TValue, Error> ValidateMetafield<TValue>(
        TValue auditable,
        string? @namespace,
        string? key)
        where TValue : IMetafieldable
    {
        if (auditable is null)
        {
            return Result<TValue, Error>.Failure(
                MetafieldableResult.Failure.EntityRequired);
        }

        return Result<TValue, Error>.Success(auditable)
            .Ensure(
                predicate: _ => !string.IsNullOrWhiteSpace(@namespace),
                error: MetafieldableResult.Failure.NamespaceRequired)

            .Ensure(
                predicate: _ =>
                    @namespace!.Length <=
                    MetafieldableConstant.Constraints.MaxNamespaceLength,
                error: MetafieldableResult.Failure.NamespaceTooLong)

            .Ensure(
                predicate: _ =>
                    Regex.IsMatch(
                        @namespace!,
                        MetafieldableConstant.Patterns.SnakeCase),
                error: MetafieldableResult.Failure.NamespaceInvalid)

            .Ensure(
                predicate: _ => !string.IsNullOrWhiteSpace(key),
                error: MetafieldableResult.Failure.KeyRequired)

            .Ensure(
                predicate: _ =>
                    key!.Length <=
                    MetafieldableConstant.Constraints.MaxKeyLength,
                error: MetafieldableResult.Failure.KeyTooLong)

            .Ensure(
                predicate: _ =>
                    Regex.IsMatch(
                        key!,
                        MetafieldableConstant.Patterns.SnakeCase),
                error: MetafieldableResult.Failure.KeyInvalid);
    }

    /// <summary>
    /// Validates that the metafield that
    /// <see cref="MetafieldableExtensions.RemoveMetafield{TValue}"/>
    /// targets is present, without mutating the entity.
    /// </summary>
    public static Result<TValue, Error> ValidateMetafieldRemoval<TValue>(
        TValue auditable,
        string? @namespace,
        string? key)
        where TValue : IMetafieldable
    {
        if (auditable is null)
        {
            return Result<TValue, Error>.Failure(
                MetafieldableResult.Failure.EntityRequired);
        }

        return Result<TValue, Error>.Success(auditable)
            .Ensure(
                predicate: entity =>
                    entity.Metafields.Any(metafield =>
                        metafield.Namespace == @namespace &&
                        metafield.Key == key),
                error: MetafieldableResult.Failure.MetafieldNotFound);
    }
}
