using BuildingBlock.Application.Events;
using BuildingBlock.Domain.Events.Domain;

namespace BuildingBlock.Application.UnitTest.Events;

public sealed class PostCommitDomainEventDispatcherContractSpec
{
    [Fact]
    public void DispatchAsync_Should_Accept_Ordered_Domain_Events_And_Optional_Cancellation()
    {
        var method = typeof(IPostCommitDomainEventDispatcher)
            .GetMethods()
            .Single(candidate =>
                candidate.Name == nameof(IPostCommitDomainEventDispatcher.DispatchAsync));

        method.ReturnType.ShouldBe(typeof(Task));
        method.GetParameters().Select(parameter => parameter.ParameterType)
            .ShouldBe(new[]
            {
                typeof(IReadOnlyList<IDomainEvent>),
                typeof(CancellationToken)
            });
        method.GetParameters()[1].HasDefaultValue.ShouldBeTrue();
    }
}
