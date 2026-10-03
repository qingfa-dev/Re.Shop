using BuildingBlock.Domain.Events;
using BuildingBlock.Domain.Events.Domain;
using BuildingBlock.Kernel.Metadata;

namespace BuildingBlock.Domain.UnitTest.Events;

public sealed class EventValidatorSpec
{
    [Fact]
    public void Validate_Should_Return_The_Valid_Event()
    {
        var @event = new TestEvent();

        var result = EventValidator.Validate(@event);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(@event);
    }

    [Fact]
    public void Validate_Should_Return_Event_Required_For_Null()
    {
        var result = EventValidator.Validate<TestEvent>(null);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.Single().Code.ShouldBe("Event.Required");
    }

    [Fact]
    public void Validate_Should_Return_Event_Type_Required_For_Blank_Type()
    {
        var result = EventValidator.Validate(new TestEvent(eventType: " "));

        result.IsSuccess.ShouldBeFalse();
        result.Errors.Single().Code.ShouldBe("Event.Type.Required");
    }

    [Fact]
    public void Validate_Should_Return_Version_Invalid_For_Nonpositive_Version()
    {
        var result = EventValidator.Validate(new TestEvent(eventVersion: 0));

        result.IsSuccess.ShouldBeFalse();
        result.Errors.Single().Code.ShouldBe("Event.Version.Invalid");
    }

    [Fact]
    public void Validate_Should_Return_Event_Id_Required_For_Empty_Id()
    {
        var result = EventValidator.Validate(new TestEvent(eventId: Guid.Empty));

        result.IsSuccess.ShouldBeFalse();
        result.Errors.Single().Code.ShouldBe("Event.Id.Required");
    }

    [Fact]
    public void Validate_Should_Return_Occurrence_Time_Required_For_Default_Time()
    {
        var result = EventValidator.Validate(new TestEvent(occurredOnUtc: default(DateTimeOffset)));

        result.IsSuccess.ShouldBeFalse();
        result.Errors.Single().Code.ShouldBe("Event.OccurredOnUtc.Required");
    }

    [Fact]
    public void Validate_Should_Return_Utc_Error_For_Nonzero_Offset()
    {
        var occurredOn = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.FromHours(2));
        var result = EventValidator.Validate(new TestEvent(occurredOnUtc: occurredOn));

        result.IsSuccess.ShouldBeFalse();
        result.Errors.Single().Code.ShouldBe("Event.OccurredOnUtc.MustBeUtc");
    }

    [Fact]
    public void Validate_Should_Return_Metadata_Required_For_Null_Metadata()
    {
        var result = EventValidator.Validate(new TestEvent(useNullMetadata: true));

        result.IsSuccess.ShouldBeFalse();
        result.Errors.Single().Code.ShouldBe("Event.Metadata.Required");
    }

    [Fact]
    public void EnsureValid_Should_Throw_Argument_Exception_For_Invalid_Event()
    {
        var @event = new TestEvent(eventType: "");

        var exception = Should.Throw<ArgumentException>(() => EventValidator.EnsureValid(@event));

        exception.Message.ShouldContain("Event.Type.Required");
    }

    [Fact]
    public void EnsureValid_Should_Throw_Argument_Null_Exception_For_Null_Event()
    {
        Should.Throw<ArgumentNullException>(() => EventValidator.EnsureValid(null));
    }

    private sealed class TestEvent(
        string? eventType = "orders.test",
        int eventVersion = 1,
        Guid? eventId = null,
        DateTimeOffset? occurredOnUtc = null,
        bool useNullMetadata = false) : IEvent
    {
        public Guid EventId { get; } = eventId ?? Guid.NewGuid();
        public DateTimeOffset OccurredOnUtc { get; } = occurredOnUtc ?? DateTimeOffset.UtcNow;
        public string EventType { get; } = eventType!;
        public int EventVersion { get; } = eventVersion;
        public MetadataDictionary Metadata { get; } = useNullMetadata ? null! : new MetadataDictionary();
    }
}
