using Domain.Aggregates;
using Domain.Events.Domain;

namespace Domain.UnitTests.Domain.Aggregates;

public class StateAggregateRootSpec
{
    [Fact]
    public void Constructor_ShouldExposeIdAndStartWithNoDomainEvents()
    {
        var id = Guid.NewGuid();
        var aggregate = new TestAggregate(id);

        aggregate.Id.ShouldBe(id);
        aggregate.DomainEvents.ShouldBeEmpty();
    }

    [Fact]
    public void RaiseDomainEvent_ShouldPreserveOrderAndExposeReadOnlyCollection()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());
        aggregate.ChangeTo("first");
        aggregate.ChangeTo("second");

        aggregate.State.ShouldBe("second");
        aggregate.DomainEvents
            .Cast<TestDomainEvent>()
            .Select(domainEvent => domainEvent.Name)
            .ShouldBe(["first", "second"]);

        var events = aggregate.DomainEvents.ShouldBeAssignableTo<ICollection<IDomainEvent>>();
        events.IsReadOnly.ShouldBeTrue();
        Should.Throw<NotSupportedException>(
            () => events.Add(new TestDomainEvent("outside")));
    }

    [Fact]
    public void ClearDomainEvents_ShouldRemovePendingEvents()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());
        aggregate.ChangeTo("first");

        aggregate.ClearDomainEvents();

        aggregate.DomainEvents.ShouldBeEmpty();
    }

    [Fact]
    public void RejectedOperation_ShouldNotRaiseDomainEvent()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());
        aggregate.ChangeTo("first");

        var result = aggregate.TryChangeTo(null);

        result.ShouldBeFalse();
        aggregate.State.ShouldBe("first");
        aggregate.DomainEvents.Count.ShouldBe(1);
    }

    [Fact]
    public void RaiseDomainEvent_NullEvent_ShouldThrowArgumentNullException()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());

        Should.Throw<ArgumentNullException>(aggregate.RaiseNull);
        aggregate.DomainEvents.ShouldBeEmpty();
    }

    private sealed class TestAggregate(Guid id) : AggregateRoot<Guid>(id)
    {
        public string? State { get; private set; }

        public void ChangeTo(string value)
        {
            State = value;
            RaiseDomainEvent(new TestDomainEvent(value));
        }

        public bool TryChangeTo(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            ChangeTo(value);
            return true;
        }

        public void RaiseNull()
        {
            RaiseDomainEvent(null!);
        }
    }

    private sealed record TestDomainEvent(string Name) : IDomainEvent
    {
        public Dictionary<string, object?>? Metadata { get; set; }
    }
}
