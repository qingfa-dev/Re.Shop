using BuildingBlock.Domain.Events;
using Mediator;

namespace BuildingBlock.Domain.Events.Domain;

public interface IDomainEvent : IEvent, INotification;