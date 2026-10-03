using BuildingBlock.Kernel.Errors;
using BuildingBlock.Kernel.Results;

namespace BuildingBlock.Domain.Events;

public static class EventResult
{
    public static class Success
    {
        public static Result<TEvent> Validated<TEvent>(TEvent value)
            => Result<TEvent>.Success(value);
    }

    public static class Failure
    {
        public static Error EventRequired => Error.UnprocessableEntity(
            code: "Event.Required",
            message: "Event is required.");

        public static Error EventTypeRequired => Error.UnprocessableEntity(
            code: "Event.Type.Required",
            message: "Event type is required.");

        public static Error EventVersionInvalid => Error.UnprocessableEntity(
            code: "Event.Version.Invalid",
            message: $"Event version must be at least {EventConstant.Constraints.MinimumEventVersion}.");

        public static Error EventIdRequired => Error.UnprocessableEntity(
            code: "Event.Id.Required",
            message: "Event ID must not be empty.");

        public static Error OccurredOnUtcRequired => Error.UnprocessableEntity(
            code: "Event.OccurredOnUtc.Required",
            message: "Event occurrence time is required.");

        public static Error OccurredOnUtcMustBeUtc => Error.UnprocessableEntity(
            code: "Event.OccurredOnUtc.MustBeUtc",
            message: "Event occurrence time must use the UTC offset.");

        public static Error MetadataRequired => Error.UnprocessableEntity(
            code: "Event.Metadata.Required",
            message: "Event metadata is required.");
    }
}
