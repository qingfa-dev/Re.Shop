using Domain.Aggregates;
using Domain.Entities;
using Domain.Events;
using Domain.Events.Domain;
using Domain.Events.Sourcing;
using Mediator;
using SharedKernel.Concerns.Foundation.Identifiable;
using SharedKernel.Structures.Meta;

namespace Domain.UnitTests.Contracts;

public class DomainContractSpec
{
    [Fact]
    public void EntityContract_ShouldRefineSharedKernelIdentity()
    {
        typeof(IEntity<Guid>)
            .IsAssignableTo(typeof(IIdentifiable<Guid>))
            .ShouldBeTrue();
    }

    [Fact]
    public void AggregateContracts_ShouldExposeTheirRequiredMembers()
    {
        typeof(IAggregateRoot<Guid>)
            .GetProperty(nameof(IAggregateRoot<Guid>.DomainEvents))
            .ShouldNotBeNull();
        typeof(IAggregateRoot<Guid>)
            .GetMethod(nameof(IAggregateRoot<Guid>.ClearDomainEvents))
            .ShouldNotBeNull();

        typeof(IEventSourcedAggregate<Guid>)
            .IsAssignableTo(typeof(IAggregateRoot<Guid>))
            .ShouldBeTrue();
        typeof(IEventSourcedAggregate<Guid>)
            .GetProperty(nameof(IEventSourcedAggregate<Guid>.Version))
            .ShouldNotBeNull();
        typeof(IEventSourcedAggregate<Guid>)
            .GetProperty(nameof(IEventSourcedAggregate<Guid>.UncommittedEvents))
            .ShouldNotBeNull();
        typeof(IEventSourcedAggregate<Guid>)
            .GetMethod(nameof(IEventSourcedAggregate<Guid>.LoadFromHistory))
            .ShouldNotBeNull();
        typeof(IEventSourcedAggregate<Guid>)
            .GetMethod(nameof(IEventSourcedAggregate<Guid>.ClearUncommittedEvents))
            .ShouldNotBeNull();
    }

    [Fact]
    public void EventContracts_ShouldUseMediatorNotificationsAndMetadata()
    {
        typeof(IEvent).IsAssignableTo(typeof(INotification)).ShouldBeTrue();
        typeof(IEvent).IsAssignableTo(typeof(IMetadata)).ShouldBeTrue();
        typeof(IDomainEvent).IsAssignableTo(typeof(IEvent)).ShouldBeTrue();
        typeof(IEventSourcedEvent).IsAssignableTo(typeof(IDomainEvent)).ShouldBeTrue();
        typeof(IEventHandler<TestDomainEvent>)
            .IsAssignableTo(typeof(INotificationHandler<TestDomainEvent>))
            .ShouldBeTrue();
        typeof(IDomainEventHandler<TestDomainEvent>)
            .IsAssignableTo(typeof(IEventHandler<TestDomainEvent>))
            .ShouldBeTrue();
    }

    private sealed record TestDomainEvent : IDomainEvent
    {
        public Dictionary<string, object?>? Metadata { get; set; }
    }
}
