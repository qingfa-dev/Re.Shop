using BuildingBlock.Domain.Events;
using BuildingBlock.Domain.Events.Domain;
using BuildingBlock.Domain.Events.Integration;
using BuildingBlock.Domain.Aggregates;
using BuildingBlock.Domain.Aggregates.Repositories;
using BuildingBlock.Kernel.Metadata;
using Mediator;

namespace BuildingBlock.Domain.UnitTest.Events;

[Trait("Category", "Contract")]
public sealed class EventContractsSpec
{
    [Fact]
    public void Event_Should_Keep_Default_Identity_And_Occurrence_Time_Stable()
    {
        var @event = new TestDomainEvent();

        var eventId = @event.EventId;
        var occurredOnUtc = @event.OccurredOnUtc;

        @event.EventId.ShouldBe(eventId);
        @event.OccurredOnUtc.ShouldBe(occurredOnUtc);
        eventId.ShouldNotBe(Guid.Empty);
        occurredOnUtc.ShouldNotBe(default);
        occurredOnUtc.Offset.ShouldBe(TimeSpan.Zero);
    }

    [Fact]
    public void Event_Should_Preserve_Envelope_Values_And_Normalize_Timestamp()
    {
        var eventId = Guid.NewGuid();
        var occurredOn = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.FromHours(5));
        var @event = new TestDomainEvent("orders.placed", 3, eventId, occurredOn);

        @event.EventId.ShouldBe(eventId);
        @event.OccurredOnUtc.ShouldBe(occurredOn.ToUniversalTime());
        @event.OccurredOnUtc.Offset.ShouldBe(TimeSpan.Zero);
        @event.EventType.ShouldBe("orders.placed");
        @event.EventVersion.ShouldBe(3);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Event_Should_Reject_Blank_Event_Type(string eventType)
    {
        Should.Throw<ArgumentException>(() => new TestDomainEvent(eventType));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Event_Should_Reject_Nonpositive_Version(int eventVersion)
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new TestDomainEvent(eventVersion: eventVersion));
    }

    [Fact]
    public void Event_Should_Reject_Empty_Event_Id()
    {
        Should.Throw<ArgumentException>(() => new TestDomainEvent(eventId: Guid.Empty));
    }

    [Fact]
    public void Event_Should_Reject_Default_Occurrence_Time()
    {
        Should.Throw<ArgumentException>(() => new TestDomainEvent(occurredOnUtc: (DateTimeOffset?)default(DateTimeOffset)));
    }

    [Fact]
    public void Event_Should_Copy_Metadata_Into_Independent_Dictionaries()
    {
        var suppliedMetadata = MetadataDictionary.Create().With("source", "caller");
        var first = new TestDomainEvent(metadata: suppliedMetadata);
        var second = new TestDomainEvent(metadata: suppliedMetadata);

        first.Metadata.ShouldNotBeSameAs(suppliedMetadata);
        first.Metadata.ShouldNotBeSameAs(second.Metadata);
        first.Metadata["source"].ShouldBe("caller");

        suppliedMetadata["source"] = "changed";
        first.Metadata["source"].ShouldBe("caller");
        first.Metadata["first-only"] = true;
        second.Metadata.ContainsKey("first-only").ShouldBeFalse();
    }

    [Fact]
    public void Event_Should_Create_Independent_Metadata_When_None_Is_Supplied()
    {
        var first = new TestDomainEvent();
        var second = new TestDomainEvent();

        first.Metadata.ShouldNotBeSameAs(second.Metadata);
        first.Metadata.ShouldNotBeSameAs(MetadataDictionary.Empty);
        second.Metadata.ShouldNotBeSameAs(MetadataDictionary.Empty);
    }

    [Fact]
    public void Event_Kinds_Should_Keep_Notification_Lane_Separate()
    {
        typeof(INotification).IsAssignableFrom(typeof(IDomainEvent)).ShouldBeTrue();
        typeof(INotification).IsAssignableFrom(typeof(IIntegrationEvent)).ShouldBeFalse();
    }

    [Fact]
    public void Domain_Event_Handler_Should_Implement_Mediator_Notification_Handler()
    {
        typeof(INotificationHandler<TestDomainEvent>)
            .IsAssignableFrom(typeof(TestDomainEventHandler))
            .ShouldBeTrue();
    }

    [Fact]
    public void Domain_Event_Dispatcher_Should_Expose_Only_Domain_Publish_Contract()
    {
        var methods = typeof(IDomainEventDispatcher).GetMethods();

        methods.Length.ShouldBe(1);
        methods[0].Name.ShouldBe("PublishAsync");
        methods[0].ReturnType.ShouldBe(typeof(Task));
        methods[0].IsGenericMethod.ShouldBeFalse();
        methods[0].GetParameters().Select(parameter => parameter.ParameterType)
            .ShouldBe(new[] { typeof(IDomainEvent), typeof(CancellationToken) });
    }

    [Fact]
    public void Domain_Event_Dispatcher_Should_Use_Mediator_Notification_Publisher()
    {
        typeof(INotificationPublisher).IsAssignableFrom(typeof(IDomainEventDispatcher)).ShouldBeTrue();
    }

    [Fact]
    public void Aggregate_Repository_Should_Expose_Only_Load_Add_And_Remove()
    {
        var repositoryType = typeof(IAggregateRepository<TestAggregate, Guid>);
        var methods = repositoryType.GetMethods();

        methods.Select(method => method.Name)
            .ShouldBe(new[] { "GetByIdAsync", "AddAsync", "RemoveAsync" });

        var getById = methods.Single(method => method.Name == "GetByIdAsync");
        getById.ReturnType.ShouldBe(typeof(ValueTask<TestAggregate>));
        getById.GetParameters().Select(parameter => parameter.ParameterType)
            .ShouldBe(new[] { typeof(Guid), typeof(CancellationToken) });

        foreach (var methodName in new[] { "AddAsync", "RemoveAsync" })
        {
            var method = methods.Single(candidate => candidate.Name == methodName);
            method.ReturnType.ShouldBe(typeof(ValueTask));
            method.GetParameters().Select(parameter => parameter.ParameterType)
                .ShouldBe(new[] { typeof(TestAggregate), typeof(CancellationToken) });
        }

        var aggregateTypeParameter = typeof(IAggregateRepository<,>).GetGenericArguments()[0];
        aggregateTypeParameter.GenericParameterAttributes
            .HasFlag(System.Reflection.GenericParameterAttributes.ReferenceTypeConstraint)
            .ShouldBeTrue();
        aggregateTypeParameter.GetGenericParameterConstraints()
            .ShouldContain(typeof(IAggregateRoot));
    }

    private sealed class TestDomainEventHandler : IDomainEventHandler<TestDomainEvent>
    {
        public ValueTask Handle(TestDomainEvent notification, CancellationToken cancellationToken)
            => ValueTask.CompletedTask;
    }

    private sealed class TestDomainEvent(
        string eventType = "orders.test-domain",
        int eventVersion = 1,
        Guid? eventId = null,
        DateTimeOffset? occurredOnUtc = null,
        MetadataDictionary? metadata = null)
        : DomainEvent(eventType, eventVersion, eventId, occurredOnUtc, metadata);

    private sealed class TestIntegrationEvent(
        string eventType = "orders.test-integration",
        int eventVersion = 1,
        Guid? eventId = null,
        DateTimeOffset? occurredOnUtc = null,
        MetadataDictionary? metadata = null)
        : IntegrationEvent(eventType, eventVersion, eventId, occurredOnUtc, metadata);

    private sealed class TestAggregate : AggregateRoot;
}
