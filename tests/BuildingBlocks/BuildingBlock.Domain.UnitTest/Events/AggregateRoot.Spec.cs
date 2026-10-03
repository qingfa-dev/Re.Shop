using BuildingBlock.Domain.Aggregates;
using BuildingBlock.Domain.Events.Domain;

namespace BuildingBlock.Domain.UnitTest.Events;

public sealed class AggregateRootSpec
{
    [Fact]
    public void Aggregate_Root_Should_Implement_Domain_Event_Contracts()
    {
        var aggregate = new TestAggregate();

        typeof(IHasDomainEvents).IsAssignableFrom(typeof(IAggregateRoot)).ShouldBeTrue();
        aggregate.ShouldBeAssignableTo<IAggregateRoot>();
    }

    [Fact]
    public void Domain_Events_Should_Start_Empty()
    {
        var aggregate = new TestAggregate();

        aggregate.DomainEvents.ShouldBeEmpty();
    }

    [Fact]
    public void Domain_Events_Should_Preserve_Insertion_Order()
    {
        var aggregate = new TestAggregate();
        var first = new TestDomainEvent();
        var second = new TestDomainEvent();

        aggregate.Raise(first);
        aggregate.Raise(second);

        aggregate.DomainEvents.ShouldBe(new IDomainEvent[] { first, second });
    }

    [Fact]
    public void Domain_Events_Should_Not_Allow_Mutation_Through_Exposed_Collection()
    {
        var aggregate = new TestAggregate();
        var @event = new TestDomainEvent();
        aggregate.Raise(@event);

        Should.Throw<NotSupportedException>(
            () => ((IList<IDomainEvent>)aggregate.DomainEvents).Add(new TestDomainEvent()));

        aggregate.DomainEvents.ShouldBe(new IDomainEvent[] { @event });
    }

    [Fact]
    public void Raise_Domain_Event_Should_Reject_Null()
    {
        var aggregate = new TestAggregate();

        Should.Throw<ArgumentNullException>(() => aggregate.Raise(null!));
    }

    [Fact]
    public void Clear_Domain_Events_Should_Empty_Pending_Events()
    {
        var aggregate = new TestAggregate();
        aggregate.Raise(new TestDomainEvent());

        aggregate.ClearDomainEvents();

        aggregate.DomainEvents.ShouldBeEmpty();
    }

    [Fact]
    public void Domain_Events_Can_Be_Raised_After_Clearing()
    {
        var aggregate = new TestAggregate();
        aggregate.Raise(new TestDomainEvent());
        aggregate.ClearDomainEvents();
        var nextEvent = new TestDomainEvent();

        aggregate.Raise(nextEvent);

        aggregate.DomainEvents.ShouldBe(new IDomainEvent[] { nextEvent });
    }

    private sealed class TestAggregate : AggregateRoot
    {
        public void Raise(IDomainEvent domainEvent) => RaiseDomainEvent(domainEvent);
    }

    private sealed class TestDomainEvent() : DomainEvent("orders.test-domain", 1);
}
