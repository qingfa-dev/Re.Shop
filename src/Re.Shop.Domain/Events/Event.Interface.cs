using Mediator;
using SharedKernel.Structures.Meta;

namespace Domain.Events;

/// <summary>Base contract for anything routed through Mediator that carries metadata.</summary>
public interface IEvent : INotification, IMetadata
{
}
