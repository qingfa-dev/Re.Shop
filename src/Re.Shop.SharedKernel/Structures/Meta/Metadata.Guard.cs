using System.Diagnostics.CodeAnalysis;

namespace SharedKernel.Structures.Meta;

public static class MetadataGuard
{
    public static void ValidateKey([NotNull] string? key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(
                nameof(key),
                $"{MetadataConstant.Result.Failure.Key.NullOrWhitespace.Code} : {MetadataConstant.Result.Failure.Key.NullOrWhitespace.Message}");
        }
    }

    public static void ValidateValue(object? value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(
                nameof(value),
                $"{MetadataConstant.Result.Failure.Value.Null.Code} : {MetadataConstant.Result.Failure.Value.Null.Message}");
        }
    }
}
