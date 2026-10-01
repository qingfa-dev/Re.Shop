namespace SharedKernel.Structures.Errors;

public static class ErrorGuard
{
    public static void ValidateCode(string? code)
    {
        // Validate: code is required
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentNullException(nameof(code),
            $"{ErrorConstant.Result.Failure.Code.NullOrWhitespace.Code} : {ErrorConstant.Result.Failure.Code.NullOrWhitespace.Message}");

        // Validate: code length
        if (code.Length > ErrorConstant.Constraint.Code.MaxLength)
            throw new ArgumentOutOfRangeException(nameof(code),
            $"{ErrorConstant.Result.Failure.Code.ExceedsMaxLength.Code} : {string.Format(ErrorConstant.Result.Failure.Code.ExceedsMaxLength.Message, ErrorConstant.Constraint.Code.MaxLength)}");
    }

    public static void ValidateMessage(string? message)
    {
        // Validate: message is required
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentNullException(nameof(message),
            $"{ErrorConstant.Result.Failure.Message.NullOrWhitespace.Code} : {ErrorConstant.Result.Failure.Message.NullOrWhitespace.Message}");

        // Validate: message length
        if (message.Length > ErrorConstant.Constraint.Message.MaxLength)
            throw new ArgumentOutOfRangeException(nameof(message),
            $"{ErrorConstant.Result.Failure.Message.ExceedsMaxLength.Code} : {string.Format(ErrorConstant.Result.Failure.Message.ExceedsMaxLength.Message, ErrorConstant.Constraint.Message.MaxLength)}");
    }

    public static void ValidateStatus(int status)
    {
        // Validate: status range
        if (status < ErrorConstant.Constraint.Status.Min || status > ErrorConstant.Constraint.Status.Max)
            throw new ArgumentOutOfRangeException(nameof(status),
            $"{ErrorConstant.Result.Failure.Status.OutOfRange.Code} : {string.Format(ErrorConstant.Result.Failure.Status.OutOfRange.Message, ErrorConstant.Constraint.Status.Min, ErrorConstant.Constraint.Status.Max)}");
    }
}
