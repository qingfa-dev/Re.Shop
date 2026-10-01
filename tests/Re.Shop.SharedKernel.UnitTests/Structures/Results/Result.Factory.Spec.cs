namespace SharedKernel.UnitTests.Structures.Results;

public class ResultFactorySpec
{
    #region Create

    [Theory]
    [InlineData(200)]
    [InlineData(201)]
    [InlineData(202)]
    [InlineData(204)]
    public void Success_WithValueAndSuccessfulStatus_ReturnsSuccess(int status)
    {
        var result = Result<string, Error>.Success("saved", status);

        result.IsSuccess.ShouldBeTrue();
        result.Status.ShouldBe(status);
        result.Value.ShouldBe("saved");
    }

    [Fact]
    public void Create_WithUnitFailure_UsesDefaultUnitAsNoPayload()
    {
        var error = new Error("Test.NotFound", "Not found", 404);

        var result = Result<Unit, Error>.Create(Unit.Value, false, 404, [error]);

        result.IsFailure.ShouldBeTrue();
        result.HasValue.ShouldBeFalse();
        result.Status.ShouldBe(404);
        result.Errors.ShouldHaveSingleItem().ShouldBe(error);
    }

    [Fact]
    public void Failure_WithNullOrEmptyErrors_ReturnsSafeFailure()
    {
        var nullFailure = Result<Unit, Error>.Failure((Error[])null!);
        var emptyFailure = Result<Unit, Error>.Failure(Array.Empty<Error>());

        nullFailure.IsFailure.ShouldBeTrue();
        nullFailure.Status.ShouldBe(500);
        nullFailure.Errors.ShouldBeEmpty();
        emptyFailure.IsFailure.ShouldBeTrue();
        emptyFailure.Status.ShouldBe(500);
        emptyFailure.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Create_WithNullSuccessValue_ReturnsValidationFailure()
    {
        var result = Result<string, Error>.Create(null, true, 200);

        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldHaveSingleItem().Code.ShouldBe(ResultConstant.Failure.Value.Required.Code);
    }

    [Fact]
    public void Create_WithValueOnFailure_ReturnsValidationFailure()
    {
        var error = new Error("Test.BadRequest", "Bad request", 400);

        var result = Result<string, Error>.Create("unexpected", false, 400, [error]);

        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldHaveSingleItem().Code.ShouldBe(ResultConstant.Failure.Value.NotAllowed.Code);
    }

    [Theory]
    [InlineData(99)]
    [InlineData(600)]
    public void Create_WithStatusOutsideHttpRange_ReturnsValidationFailure(int status)
    {
        var result = Result<string, Error>.Create("saved", true, status);

        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldHaveSingleItem().Code.ShouldBe(ResultConstant.Failure.Status.InvalidStatusCode.Code);
    }

    [Fact]
    public void Create_WithMixedErrorStatuses_ReturnsMixedStatusFailure()
    {
        var errors = new[]
        {
            new Error("Test.BadRequest", "Bad request", 400),
            new Error("Test.NotFound", "Not found", 404)
        };

        var result = Result<string, Error>.Create(default, false, 400, errors);

        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldHaveSingleItem().Code.ShouldBe(ResultConstant.Failure.Result.MixedStatusCodes.Code);
    }

    [Fact]
    public void Failure_WithTooManyErrors_ReturnsMaximumErrorValidationFailure()
    {
        var errors = Enumerable.Repeat(
            new Error("Test.BadRequest", "Bad request", 400),
            ResultConstant.Constraint.Errors.MaxCount + 1);

        var result = Result<string, Error>.Failure(errors);

        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldHaveSingleItem().Code.ShouldBe(ResultConstant.Failure.Errors.ExceedsMaxRange.Code);
    }

    #endregion
}
