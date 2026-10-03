using BuildingBlock.Kernel.Errors;

namespace BuildingBlock.Application.Results;

public static class ApplicationResult
{
    public static class Failure
    {
        public static Error ValidationFailed(string? propertyName, string message)
        {
            ArgumentNullException.ThrowIfNull(message);

            var prefix = string.IsNullOrWhiteSpace(propertyName)
                ? string.Empty
                : $"{propertyName}: ";
            var renderedMessage = $"{prefix}{message}";
            if (renderedMessage.Length > ErrorConstant.Constraint.Message.MaxLength)
            {
                renderedMessage = renderedMessage[..ErrorConstant.Constraint.Message.MaxLength];
            }

            return Error.UnprocessableEntity(
                ApplicationConstant.Errors.ValidationFailed,
                renderedMessage);
        }
    }
}
