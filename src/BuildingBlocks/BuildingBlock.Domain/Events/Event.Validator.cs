using BuildingBlock.Kernel.Results;

namespace BuildingBlock.Domain.Events;

public static class EventValidator
{
    public static Result<TEvent> Validate<TEvent>(TEvent? @event)
        where TEvent : IEvent
    {
        if (@event is null)
        {
            return Result<TEvent>.Fail(EventResult.Failure.EventRequired);
        }

        return EventResult.Success.Validated(@event)
            .Ensure(
                predicate: e => !string.IsNullOrWhiteSpace(e.EventType),
                errorValue: EventResult.Failure.EventTypeRequired)
            .Ensure(
                predicate: e => e.EventVersion >= EventConstant.Constraints.MinimumEventVersion,
                errorValue: EventResult.Failure.EventVersionInvalid)
            .Ensure(
                predicate: e => e.EventId != Guid.Empty,
                errorValue: EventResult.Failure.EventIdRequired)
            .Ensure(
                predicate: e => e.OccurredOnUtc != default,
                errorValue: EventResult.Failure.OccurredOnUtcRequired)
            .Ensure(
                predicate: e => e.OccurredOnUtc.Offset == TimeSpan.Zero,
                errorValue: EventResult.Failure.OccurredOnUtcMustBeUtc)
            .Ensure(
                predicate: e => e.Metadata is not null,
                errorValue: EventResult.Failure.MetadataRequired);
    }

    public static void EnsureValid(IEvent? @event)
    {
        ArgumentNullException.ThrowIfNull(@event);

        var result = Validate(@event);
        if (result.IsFailure)
        {
            var message = string.Join(
                Environment.NewLine,
                result.Errors.Select(error => $"{error.Code}: {error.Message}"));

            if (result.Errors.Any(error =>
                    error.Code == EventResult.Failure.EventVersionInvalid.Code))
            {
                throw new ArgumentOutOfRangeException(
                    "eventVersion",
                    @event.EventVersion,
                    message);
            }

            throw new ArgumentException(message, nameof(@event));
        }
    }
}
