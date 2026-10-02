using Domain.Aggregates;
using Domain.Events.Domain;
using Domain.Events.Sourcing;

namespace Domain.UnitTests.Domain.Aggregates;

public class EventSourcedAggregateRootSpec
{
    [Fact]
    public void RaiseEvent_ShouldApplyQueueAndAdvanceVersion()
    {
        var aggregate = new BalanceAggregate(Guid.NewGuid());

        aggregate.Add(5m);

        aggregate.Balance.ShouldBe(5m);
        aggregate.Version.ShouldBe(1);
        aggregate.UncommittedEvents.ShouldHaveSingleItem();
        aggregate.DomainEvents.ShouldHaveSingleItem();
        aggregate.UncommittedEvents.Single().ShouldBeSameAs(aggregate.DomainEvents.Single());
    }

    [Fact]
    public void LoadFromHistory_ShouldReplayInOrderWithoutQueuingEvents()
    {
        var aggregate = new BalanceAggregate(Guid.NewGuid());

        aggregate.LoadFromHistory(
        [
            new BalanceChanged(2m),
            new BalanceChanged(3m)
        ]);

        aggregate.Balance.ShouldBe(5m);
        aggregate.Version.ShouldBe(2);
        aggregate.DomainEvents.ShouldBeEmpty();
        aggregate.UncommittedEvents.ShouldBeEmpty();

        aggregate.Add(4m);

        aggregate.Balance.ShouldBe(9m);
        aggregate.Version.ShouldBe(3);
        aggregate.UncommittedEvents
            .Cast<BalanceChanged>()
            .Select(domainEvent => domainEvent.Amount)
            .ShouldBe([4m]);
    }

    [Fact]
    public void LoadFromHistory_EmptyHistory_ShouldBeValidButSingleUse()
    {
        var aggregate = new BalanceAggregate(Guid.NewGuid());

        aggregate.LoadFromHistory([]);

        aggregate.Version.ShouldBe(0);
        aggregate.Balance.ShouldBe(0m);
        Should.Throw<InvalidOperationException>(() => aggregate.LoadFromHistory([]));
    }

    [Fact]
    public void LoadFromHistory_NullHistory_ShouldThrowArgumentNullException()
    {
        var aggregate = new BalanceAggregate(Guid.NewGuid());

        Should.Throw<ArgumentNullException>(() => aggregate.LoadFromHistory(null!));
    }

    [Fact]
    public void LoadFromHistory_NullEvent_ShouldThrowArgumentNullException()
    {
        var aggregate = new BalanceAggregate(Guid.NewGuid());

        Should.Throw<ArgumentNullException>(
            () => aggregate.LoadFromHistory([null!]));

        aggregate.Version.ShouldBe(0);
        aggregate.DomainEvents.ShouldBeEmpty();
    }

    [Fact]
    public void LoadFromHistory_AfterRaise_ShouldThrowInvalidOperationException()
    {
        var aggregate = new BalanceAggregate(Guid.NewGuid());
        aggregate.Add(1m);

        Should.Throw<InvalidOperationException>(
            () => aggregate.LoadFromHistory([new BalanceChanged(2m)]));
    }

    [Fact]
    public void RaiseEvent_NullEvent_ShouldThrowArgumentNullException()
    {
        var aggregate = new BalanceAggregate(Guid.NewGuid());

        Should.Throw<ArgumentNullException>(aggregate.RaiseNull);

        aggregate.Version.ShouldBe(0);
        aggregate.UncommittedEvents.ShouldBeEmpty();
    }

    [Fact]
    public void RaiseEvent_ApplyThrows_ShouldPropagateWithoutQueueingOrAdvancingVersion()
    {
        var aggregate = new BalanceAggregate(Guid.NewGuid());

        Should.Throw<InvalidOperationException>(
            () => aggregate.Add(4m, failAfterApply: true));

        aggregate.Version.ShouldBe(0);
        aggregate.UncommittedEvents.ShouldBeEmpty();
        aggregate.DomainEvents.ShouldBeEmpty();
        aggregate.Balance.ShouldBe(4m);
    }

    [Fact]
    public void ClearMethods_ShouldClearSharedPendingEventsWithoutRewindingVersion()
    {
        var aggregate = new BalanceAggregate(Guid.NewGuid());
        aggregate.Add(4m);
        var firstVersion = aggregate.Version;

        aggregate.ClearDomainEvents();

        aggregate.DomainEvents.ShouldBeEmpty();
        aggregate.UncommittedEvents.ShouldBeEmpty();
        aggregate.Version.ShouldBe(firstVersion);

        aggregate.Add(2m);
        var secondVersion = aggregate.Version;

        aggregate.ClearUncommittedEvents();

        aggregate.DomainEvents.ShouldBeEmpty();
        aggregate.UncommittedEvents.ShouldBeEmpty();
        aggregate.Version.ShouldBe(secondVersion);
    }

    private sealed class BalanceAggregate(Guid id) : EventSourcedAggregateRoot<Guid>(id)
    {
        public decimal Balance { get; private set; }

        public void Add(decimal amount, bool failAfterApply = false)
        {
            RaiseEvent(new BalanceChanged(amount, failAfterApply));
        }

        public void RaiseNull()
        {
            RaiseEvent(null!);
        }

        protected override void Apply(IEventSourcedEvent domainEvent)
        {
            var change = (BalanceChanged)domainEvent;
            Balance += change.Amount;

            if (change.FailAfterApply)
            {
                throw new InvalidOperationException("Apply failed after changing state.");
            }
        }
    }

    private sealed record BalanceChanged(
        decimal Amount,
        bool FailAfterApply = false) : IEventSourcedEvent
    {
        public Dictionary<string, object?>? Metadata { get; set; }
    }
}
