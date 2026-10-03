using BuildingBlock.Persistence.Abstractions.Repositories;
using BuildingBlock.Persistence.Abstractions.UnitOfWork;
using BuildingBlock.Kernel.Results;
using BuildingBlock.Domain.Events.Integration;
using BuildingBlock.Messaging.Abstractions.Inbox;
using BuildingBlock.Messaging.Abstractions.Integration;
using BuildingBlock.Messaging.Abstractions.Outbox;
using BuildingBlock.Persistence.Abstractions.Specifications;

namespace BuildingBlock.Foundation.Abstractions.UnitTest.Contracts;

public sealed class ContractShapeSpec
{
    [Fact]
    public void Unit_Of_Work_Should_Return_Result_Unit_From_Commit()
    {
        var commit = typeof(IUnitOfWork).GetMethod(nameof(IUnitOfWork.CommitAsync));

        commit.ShouldNotBeNull();
        commit!.ReturnType.ShouldBe(typeof(ValueTask<Result<Unit>>));
        commit.GetParameters().Select(parameter => parameter.ParameterType)
            .ShouldBe(new[] { typeof(CancellationToken) });
    }

    [Fact]
    public void Read_Repository_Should_Expose_Only_Materialized_Specification_Operations()
    {
        var methods = typeof(IReadRepository<>).GetMethods();
        var entityType = typeof(IReadRepository<>).GetGenericArguments()[0];
        var entitySpecificationType = typeof(ISpecification<>).MakeGenericType(entityType);

        methods.Select(method => method.Name).Order()
            .ShouldBe(new[] { "CountAsync", "FirstOrDefaultAsync", "ListAsync", "ListAsync" });

        var entityList = methods.Single(method => method.Name == "ListAsync" && !method.IsGenericMethod);
        entityList.ReturnType.ShouldBe(
            typeof(ValueTask<>).MakeGenericType(
                typeof(IReadOnlyList<>).MakeGenericType(entityType)));
        entityList.GetParameters().Select(parameter => parameter.ParameterType)
            .ShouldBe(new[] { entitySpecificationType, typeof(CancellationToken) });

        var projectionList = methods.Single(method => method.Name == "ListAsync" && method.IsGenericMethod);
        var projectionType = projectionList.GetGenericArguments()[0];
        projectionList.ReturnType.ShouldBe(
            typeof(ValueTask<>).MakeGenericType(
                typeof(IReadOnlyList<>).MakeGenericType(projectionType)));
        projectionList.GetParameters().Select(parameter => parameter.ParameterType)
            .ShouldBe(new[]
            {
                typeof(ISpecification<,>).MakeGenericType(entityType, projectionType),
                typeof(CancellationToken)
            });

        var first = methods.Single(method => method.Name == "FirstOrDefaultAsync");
        first.ReturnType.ShouldBe(typeof(ValueTask<>).MakeGenericType(entityType));
        first.GetParameters().Select(parameter => parameter.ParameterType)
            .ShouldBe(new[] { entitySpecificationType, typeof(CancellationToken) });

        var count = methods.Single(method => method.Name == "CountAsync");
        count.ReturnType.ShouldBe(typeof(ValueTask<long>));
        count.GetParameters().Select(parameter => parameter.ParameterType)
            .ShouldBe(new[] { entitySpecificationType, typeof(CancellationToken) });

        methods.SelectMany(method => method.GetParameters())
            .Select(parameter => parameter.ParameterType.FullName ?? parameter.ParameterType.Name)
            .Where(value => value.Contains("IQueryable", StringComparison.Ordinal))
            .ShouldBeEmpty();
        methods.Select(method => method.ReturnType.FullName ?? method.ReturnType.Name)
            .Where(value => value.Contains("IQueryable", StringComparison.Ordinal))
            .ShouldBeEmpty();
    }

    [Fact]
    public void Persistence_Abstractions_Should_Not_Reference_Providers()
    {
        var references = typeof(IUnitOfWork).Assembly.GetReferencedAssemblies();

        references.Select(reference => reference.Name)
            .Where(name =>
                name is not null &&
                (name.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.Ordinal) ||
                 name.Contains("SqlClient", StringComparison.Ordinal) ||
                 name.Contains("Npgsql", StringComparison.Ordinal) ||
                 name.Contains("MySql", StringComparison.Ordinal) ||
                 name.Contains("Mongo", StringComparison.Ordinal) ||
                 name.Contains("RabbitMQ", StringComparison.Ordinal) ||
                 name.Contains("Kafka", StringComparison.Ordinal)))
            .ShouldBeEmpty();
    }

    [Fact]
    public void Integration_Event_Publisher_Should_Preserve_Its_Publish_Contract()
    {
        var publish = typeof(IIntegrationEventPublisher).GetMethod(
            nameof(IIntegrationEventPublisher.PublishAsync));

        publish.ShouldNotBeNull();
        publish!.ReturnType.ShouldBe(typeof(Task));
        publish.GetParameters().Select(parameter => parameter.ParameterType)
            .ShouldBe(new[] { typeof(IIntegrationEvent), typeof(CancellationToken) });
    }

    [Fact]
    public void Outbox_Writer_Should_Accept_Integration_Events()
    {
        var add = typeof(IOutboxWriter).GetMethod(nameof(IOutboxWriter.AddAsync));

        add.ShouldNotBeNull();
        add!.ReturnType.ShouldBe(typeof(Task));
        add.GetParameters().Select(parameter => parameter.ParameterType)
            .ShouldBe(new[] { typeof(IIntegrationEvent), typeof(CancellationToken) });
    }

    [Fact]
    public void Inbox_Store_Should_Register_Consumer_And_Event_Identity()
    {
        var register = typeof(IInboxStore).GetMethod(nameof(IInboxStore.TryRegisterAsync));

        register.ShouldNotBeNull();
        register!.ReturnType.ShouldBe(typeof(ValueTask<bool>));
        register.GetParameters().Select(parameter => parameter.ParameterType)
            .ShouldBe(new[] { typeof(string), typeof(Guid), typeof(CancellationToken) });
    }

    [Fact]
    public void Messaging_Abstractions_Should_Not_Reference_Providers()
    {
        var references = typeof(IIntegrationEventPublisher).Assembly.GetReferencedAssemblies();

        references.Select(reference => reference.Name)
            .Where(name =>
                name is not null &&
                (name.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.Ordinal) ||
                 name.Contains("SqlClient", StringComparison.Ordinal) ||
                 name.Contains("Npgsql", StringComparison.Ordinal) ||
                 name.Contains("MySql", StringComparison.Ordinal) ||
                 name.Contains("Mongo", StringComparison.Ordinal) ||
                 name.Contains("RabbitMQ", StringComparison.Ordinal) ||
                 name.Contains("Kafka", StringComparison.Ordinal)))
            .ShouldBeEmpty();
    }
}
