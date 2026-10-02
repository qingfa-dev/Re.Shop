using System.Diagnostics.CodeAnalysis;

namespace SharedKernel.Structures.Meta;

public static class MetadataGuard
{
    public static void ValidateKey([NotNull] string? key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(
                paramName: nameof(key),
                message:
                    $"{MetadataConstant.Errors.Key.NullOrWhitespace.Code} : " +
                    $"{MetadataConstant.Errors.Key.NullOrWhitespace.Message}");
        }
    }

    public static void ValidateValue([NotNull] object? value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(
                paramName: nameof(value),
                message:
                    $"{MetadataConstant.Errors.Value.Null.Code} : " +
                    $"{MetadataConstant.Errors.Value.Null.Message}");
        }
    }
}
