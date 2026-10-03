using BuildingBlock.Kernel.Errors;
using BuildingBlock.Kernel.Results;

using FluentValidation;

using Mediator;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BuildingBlock.Application.UnitTest;

public sealed class MediatorIntegrationSpec
{
    [Fact]
    public async Task Send_Should_Run_Wrapped_Command_Handler_And_Pipeline()
    {
        using var host = CreateHost();
        var sender = host.Services.GetRequiredService<Mediator.ISender>();

        var response = await sender.Send(new TestCommand("command"));

        response.IsSuccess.ShouldBeTrue();
        response.Value.ShouldBe("pipeline:command");
    }

    [Fact]
    public async Task Send_Should_Run_Wrapped_Query_Handler()
    {
        using var host = CreateHost();
        var sender = host.Services.GetRequiredService<Mediator.ISender>();

        var response = await sender.Send(new TestQuery("query"));

        response.IsSuccess.ShouldBeTrue();
        response.Value.ShouldBe("query");
    }

    [Fact]
    public async Task Send_Should_Return_Validation_Errors_And_Skip_Command_Handler()
    {
        using var host = CreateHost();
        var sender = host.Services.GetRequiredService<Mediator.ISender>();
        var counter = host.Services.GetRequiredService<TestHandlerInvocationCounter>();

        var response = await sender.Send(new TestCommand(""));

        response.IsFailure.ShouldBeTrue();
        response.StatusCode.ShouldBe(422);
        response.Errors.Count.ShouldBe(2);
        response.Errors.Select(error => error.Code).Distinct()
            .ShouldBe(new[] { "Application.Validation.Failed" });
        response.Errors.Select(error => error.Message).ShouldBe(new[]
        {
            "Value: Value is required.",
            "Value: Value is too short."
        });
        counter.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Send_Should_Return_Validation_Errors_And_Skip_Query_Handler()
    {
        using var host = CreateHost();
        var sender = host.Services.GetRequiredService<Mediator.ISender>();
        var counter = host.Services.GetRequiredService<TestHandlerInvocationCounter>();

        var response = await sender.Send(new TestQuery(""));

        response.IsFailure.ShouldBeTrue();
        response.StatusCode.ShouldBe(422);
        response.Errors.Count.ShouldBe(1);
        counter.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Send_Should_Preserve_Handler_Failure_Through_Command_Pipeline()
    {
        using var host = CreateHost();
        var sender = host.Services.GetRequiredService<Mediator.ISender>();
        var counter = host.Services.GetRequiredService<TestHandlerInvocationCounter>();

        var response = await sender.Send(new TestCommand("handler-failure"));

        response.IsFailure.ShouldBeTrue();
        response.Errors.ShouldContain(new Error(
            "Test.Handler.Failed",
            "Handler rejected the command.",
            status: ErrorConstant.StatusCode.Conflict,
            type: "https://errors.kernel/conflict",
            severity: ErrorSeverity.Warning));
        counter.Count.ShouldBe(1);
    }

    private static IHost CreateHost()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddSingleton<TestHandlerInvocationCounter>();
        builder.Services.AddSingleton<IValidator<TestCommand>, TestCommandValidator>();
        builder.Services.AddSingleton<IValidator<TestQuery>, TestQueryValidator>();
        builder.Services.AddMediator(options =>
        {
            options.Assemblies = [typeof(TestCommandHandler)];
            options.PipelineBehaviors =
            [
                typeof(BuildingBlock.Application.Behaviors.ValidationPipelineBehavior<,>),
                typeof(TestCommandPipelineBehavior)
            ];
        });
        return builder.Build();
    }
}

public sealed record TestCommand(string Value) : BuildingBlock.Application.Commands.ICommand<string>;

public sealed class TestCommandHandler
    : BuildingBlock.Application.Commands.ICommandHandler<TestCommand, string>
{
    private readonly TestHandlerInvocationCounter _counter;

    public TestCommandHandler(TestHandlerInvocationCounter counter)
        => _counter = counter;

    public ValueTask<Result<string>> Handle(TestCommand command, CancellationToken cancellationToken)
    {
        _counter.Increment();
        return ValueTask.FromResult(command.Value == "handler-failure"
            ? Result<string>.Fail(Error.Conflict(
                "Test.Handler.Failed",
                "Handler rejected the command."))
            : Result<string>.Success(command.Value));
    }
}

public sealed record TestQuery(string Value) : BuildingBlock.Application.Queries.IQuery<string>;

public sealed class TestQueryHandler
    : BuildingBlock.Application.Queries.IQueryHandler<TestQuery, string>
{
    private readonly TestHandlerInvocationCounter _counter;

    public TestQueryHandler(TestHandlerInvocationCounter counter)
        => _counter = counter;

    public ValueTask<Result<string>> Handle(TestQuery query, CancellationToken cancellationToken)
    {
        _counter.Increment();
        return ValueTask.FromResult(Result<string>.Success(query.Value));
    }
}

public sealed class TestCommandPipelineBehavior
    : BuildingBlock.Application.Behaviors.IApplicationPipelineBehavior<TestCommand, Result<string>>
{
    public async ValueTask<Result<string>> Handle(
        TestCommand message,
        MessageHandlerDelegate<TestCommand, Result<string>> next,
        CancellationToken cancellationToken)
    {
        var response = await next(message, cancellationToken);
        return response.Bind(value => Result<string>.Success($"pipeline:{value}"));
    }
}

public sealed class TestHandlerInvocationCounter
{
    private int _count;

    public int Count => Volatile.Read(ref _count);

    public void Increment()
        => Interlocked.Increment(ref _count);
}

public sealed class TestCommandValidator : AbstractValidator<TestCommand>
{
    public TestCommandValidator()
    {
        RuleFor(command => command.Value).NotEmpty().WithMessage("Value is required.");
        RuleFor(command => command.Value).MinimumLength(3).WithMessage("Value is too short.");
    }
}

public sealed class TestQueryValidator : AbstractValidator<TestQuery>
{
    public TestQueryValidator()
        => RuleFor(query => query.Value)
            .Must(value => !string.IsNullOrWhiteSpace(value))
            .WithMessage("Value is required.");
}
