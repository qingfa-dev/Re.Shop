namespace SharedKernel.UnitTests.Structures.Results;

public class ResultFlowControlSpec
{
    #region Map

    [Fact]
    public void Map_TransformsSuccessfulValueAndPreservesMetadata()
    {
        var result = Result<string, Error>.Success("saved")
            .WithMetadata("trace", "abc");

        var mapped = result.Map(value => value.Length);

        mapped.IsSuccess.ShouldBeTrue();
        mapped.Value.ShouldBe(5);
        mapped.Metadata!.ShouldContainKey("trace");
    }

    [Fact]
    public void Map_OnFailureDoesNotInvokeMapper()
    {
        var error = new Error("Test.NotFound", "Not found", 404);
        var invoked = false;

        var mapped = Result<string, Error>.Failure(error).Map(value =>
        {
            invoked = true;
            return value.Length;
        });

        invoked.ShouldBeFalse();
        mapped.IsFailure.ShouldBeTrue();
        mapped.Errors.ShouldHaveSingleItem().ShouldBe(error);
    }

    #endregion

    #region Bind

    [Fact]
    public void Bind_TransformsSuccessfulValue()
    {
        var result = Result<string, Error>.Success("saved")
            .Bind(value => Result<int, Error>.Success(value.Length));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(5);
    }

    [Fact]
    public void Bind_WithUnitResultPreservesNoPayloadSuccess()
    {
        var result = Result<Unit, Error>.Success(Unit.Value)
            .Bind(() => Result<Unit, Error>.Success(Unit.Value));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(Unit.Value);
    }

    #endregion

    #region Match

    [Fact]
    public void Match_UsesNoValueSuccessCallback()
    {
        var result = Result<Unit, Error>.Success(Unit.Value);

        var match = result.Match(() => "success", _ => "failure");

        match.ShouldBe("success");
    }

    #endregion

    #region Tap

    [Fact]
    public void Tap_WithUnitRunsSuccessCallback()
    {
        var tapped = false;

        var result = Result<Unit, Error>.Success(Unit.Value)
            .Tap(() => tapped = true);

        tapped.ShouldBeTrue();
        result.IsSuccess.ShouldBeTrue();
    }

    #endregion

    #region TapErrors

    [Fact]
    public void TapErrors_ReceivesEveryFailure()
    {
        var errors = new List<Error>
        {
            new("Test.First", "First", 400),
            new("Test.Second", "Second", 400)
        };
        List<Error>? received = null;

        Result<string, Error>.Failure(errors).TapErrors(value => received = value);

        received.ShouldBe(errors);
    }

    #endregion

    #region Recover

    [Fact]
    public void Recover_WithUnitResultReturnsSuccessAndPreservesMetadata()
    {
        var result = Result<Unit, Error>.Failure(
                new Error("Test.BadRequest", "Bad request", 400))
            .WithMetadata("trace", "abc");

        var recovered = result.Recover(
            (IReadOnlyList<Error> _) => Result<Unit, Error>.Success(Unit.Value));

        recovered.IsSuccess.ShouldBeTrue();
        recovered.Value.ShouldBe(Unit.Value);
        recovered.Metadata!.ShouldContainKey("trace");
    }

    #endregion

    #region MapError

    [Fact]
    public void MapError_ChangesErrorType()
    {
        var error = new Error("Test.NotFound", "Not found", 404);

        var result = Result<Unit, Error>.Failure(error)
            .MapError(value => new AlternateError(value.Code, value.Message, value.Status));

        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldHaveSingleItem().ShouldBeOfType<AlternateError>()
            .Code.ShouldBe(error.Code);
    }

    #endregion

    #region ThrowIfFailure

    [Fact]
    public void ThrowIfFailure_ThrowsForFailuresOnly()
    {
        Should.Throw<InvalidOperationException>(() =>
            Result<Unit, Error>.Failure(
                new Error("Test.BadRequest", "Bad request", 400)).ThrowIfFailure());

        Result<Unit, Error>.Success(Unit.Value).ThrowIfFailure().IsSuccess.ShouldBeTrue();
    }

    #endregion

    private sealed record AlternateError(string Code, string Message, int Status) : IError
    {
        public Dictionary<string, object?>? Metadata { get; set; }
    }
}
