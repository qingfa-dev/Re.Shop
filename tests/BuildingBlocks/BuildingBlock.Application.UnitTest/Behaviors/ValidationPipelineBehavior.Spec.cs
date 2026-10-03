using BuildingBlock.Application.Behaviors;
using BuildingBlock.Kernel.Errors;
using BuildingBlock.Kernel.Results;
using FluentValidation;
using Mediator;

namespace BuildingBlock.Application.UnitTest.Behaviors;

public sealed class ValidationPipelineBehaviorSpec
{
    [Fact]
    public async Task Handle_Should_Return_Next_Result_Unchanged_When_No_Validators_Exist()
    {
        var expected = Result<string>.Fail(Error.Conflict("test.failure", "Expected failure."));
        var nextCalls = 0;
        MessageHandlerDelegate<BuildingBlock.Application.UnitTest.TestCommand, Result<string>> next = (_, _) =>
        {
            nextCalls++;
            return ValueTask.FromResult(expected);
        };
        var behavior = new ValidationPipelineBehavior<BuildingBlock.Application.UnitTest.TestCommand, Result<string>>(
            Array.Empty<IValidator<BuildingBlock.Application.UnitTest.TestCommand>>());

        var actual = await behavior.Handle(new("value"), next, CancellationToken.None);

        actual.ShouldBeSameAs(expected);
        nextCalls.ShouldBe(1);
    }

    [Fact]
    public async Task Handle_Should_Pass_Cancellation_Token_To_Validators()
    {
        using var cancellationSource = new CancellationTokenSource();
        var expectedToken = cancellationSource.Token;
        var observedToken = CancellationToken.None;
        var validator = new InlineValidator<BuildingBlock.Application.UnitTest.TestCommand>();
        validator.RuleFor(query => query.Value)
            .MustAsync((_, token) =>
            {
                observedToken = token;
                return Task.FromResult(true);
            });
        MessageHandlerDelegate<BuildingBlock.Application.UnitTest.TestCommand, Result<string>> next = (_, _) =>
            ValueTask.FromResult(Result<string>.Success("next"));
        var behavior = new ValidationPipelineBehavior<BuildingBlock.Application.UnitTest.TestCommand, Result<string>>([validator]);

        await behavior.Handle(new("value"), next, expectedToken);

        observedToken.ShouldBe(expectedToken);
    }

    [Fact]
    public async Task Handle_Should_Propagate_Validator_Cancellation()
    {
        using var cancellationSource = new CancellationTokenSource();
        cancellationSource.Cancel();
        var validator = new InlineValidator<BuildingBlock.Application.UnitTest.TestCommand>();
        validator.RuleFor(query => query.Value)
            .MustAsync((_, token) => Task.FromCanceled<bool>(token));
        MessageHandlerDelegate<BuildingBlock.Application.UnitTest.TestCommand, Result<string>> next = (_, _) =>
            ValueTask.FromResult(Result<string>.Success("next"));
        var behavior = new ValidationPipelineBehavior<BuildingBlock.Application.UnitTest.TestCommand, Result<string>>([validator]);

        var action = async () => await behavior.Handle(
            new("value"),
            next,
            cancellationSource.Token);

        await Should.ThrowAsync<OperationCanceledException>(action);
    }

    [Fact]
    public async Task Handle_Should_Redact_Attempted_Value_From_Validation_Message()
    {
        var validator = new InlineValidator<BuildingBlock.Application.UnitTest.TestCommand>();
        validator.RuleFor(command => command.Value)
            .Must(_ => false)
            .WithMessage("Value '{PropertyValue}' is invalid.");
        MessageHandlerDelegate<BuildingBlock.Application.UnitTest.TestCommand, Result<string>> next = (_, _) =>
            ValueTask.FromResult(Result<string>.Success("next"));
        var behavior = new ValidationPipelineBehavior<BuildingBlock.Application.UnitTest.TestCommand, Result<string>>([validator]);

        var response = await behavior.Handle(
            new("sensitive-input"),
            next,
            CancellationToken.None);

        response.Errors.Single().Message.ShouldBe("Value: Value '[redacted]' is invalid.");
    }
}
