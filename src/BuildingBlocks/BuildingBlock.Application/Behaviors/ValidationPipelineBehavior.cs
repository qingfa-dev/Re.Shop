using System.Globalization;

using BuildingBlock.Application.Results;
using BuildingBlock.Kernel.Errors;
using BuildingBlock.Kernel.Results;
using FluentValidation;
using FluentValidation.Results;
using Mediator;

namespace BuildingBlock.Application.Behaviors;

public sealed class ValidationPipelineBehavior<TMessage, TResponse>
    : IApplicationPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
    where TResponse : IResult<Error>, IResultFailure<TResponse, Error>
{
    private readonly IValidator<TMessage>[] _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TMessage>> validators)
    {
        ArgumentNullException.ThrowIfNull(validators);
        _validators = validators.ToArray();
    }

    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        if (_validators.Length == 0)
            return await next(message, cancellationToken);

        var context = new ValidationContext<TMessage>(message);
        List<ValidationFailure>? failures = null;

        foreach (var validator in _validators)
        {
            var validationResult = await validator.ValidateAsync(context, cancellationToken);
            if (validationResult.Errors.Count == 0)
                continue;

            failures ??= [];
            failures.AddRange(validationResult.Errors);
        }

        if (failures is null)
            return await next(message, cancellationToken);

        var errors = failures.Select(failure =>
            ApplicationResult.Failure.ValidationFailed(
                failure.PropertyName,
                RedactAttemptedValue(failure)));
        return TResponse.Fail(errors);
    }

    private static string RedactAttemptedValue(ValidationFailure failure)
    {
        if (failure.AttemptedValue is null)
            return failure.ErrorMessage;

        var candidates = new HashSet<string>(StringComparer.Ordinal);
        if (failure.AttemptedValue is IFormattable formattable)
        {
            AddCandidate(formattable.ToString(null, CultureInfo.CurrentCulture));
            AddCandidate(formattable.ToString(null, CultureInfo.InvariantCulture));
        }
        else
        {
            AddCandidate(failure.AttemptedValue.ToString());
        }

        var message = failure.ErrorMessage;
        foreach (var candidate in candidates)
            message = message.Replace(candidate, "[redacted]", StringComparison.Ordinal);

        return message;

        void AddCandidate(string? candidate)
        {
            if (!string.IsNullOrWhiteSpace(candidate))
                candidates.Add(candidate);
        }
    }
}
