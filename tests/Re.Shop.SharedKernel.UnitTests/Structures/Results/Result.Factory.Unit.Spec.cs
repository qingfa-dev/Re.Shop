namespace SharedKernel.UnitTests.Structures.Results;

public class ResultFactoryUnitSpec
{
    #region Create

    [Theory]
    [InlineData(true, 200)]
    [InlineData(false, 400)]
    public void Create_ShouldHaveCorrectProperties(bool isSuccess, int status)
    {
        var errors = isSuccess
            ? []
            : new List<Error> { Error.BadRequest("Code", "Desc") };

        var result = Result<Unit, Error>.Create(isSuccess, status, errors);

        result.IsSuccess.ShouldBe(isSuccess);
        result.Status.ShouldBe(status);
        result.Errors.Count.ShouldBe(isSuccess ? 0 : 1);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    public void Create_WithNullOrEmptyErrors_ShouldDefaultToEmpty(int? errorCount)
    {
        var errors = errorCount == 0
            ? new List<Error>()
            : null;

        var result = Result<Unit, Error>.Create(true, 200, errors);

        result.IsSuccess.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    #endregion

    #region Success Factories

    [Theory]
    [InlineData(200)]
    [InlineData(201)]
    [InlineData(202)]
    [InlineData(204)]
    public void Success_ShouldReturnCorrectStatus(int status)
    {
        var result = status switch
        {
            200 => Result<Unit, Error>.Success(Unit.Value),
            201 => Result<Unit, Error>.Success(Unit.Value, 201),
            202 => Result<Unit, Error>.Accepted(Unit.Value),
            204 => Result<Unit, Error>.NoContent(Unit.Value),
            _ => throw new InvalidOperationException()
        };

        result.IsSuccess.ShouldBeTrue();
        result.Status.ShouldBe(status);
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Ok_ShouldReturn200()
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);

        result.IsSuccess.ShouldBeTrue();
        result.Status.ShouldBe(200);
    }

    [Fact]
    public void Created_ShouldReturn201()
    {
        var result = Result<Unit, Error>.Created(Unit.Value);

        result.IsSuccess.ShouldBeTrue();
        result.Status.ShouldBe(201);
    }

    #endregion

    #region Failure Factories

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void Failure_WithErrors_ShouldReturnFailure(int errorCount)
    {
        var errors = Enumerable.Range(0, errorCount == 0 ? 0 : errorCount)
            .Select(i => Error.BadRequest($"Code{i}", $"Desc{i}"))
            .ToArray();

        var result = Result<Unit, Error>.Failure(errors);

        result.IsSuccess.ShouldBeFalse();
        result.Status.ShouldBe(errorCount == 0 ? 500 : 400);
        result.Errors.Count.ShouldBe(errorCount == 0 ? 0 : errorCount);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    public void Failure_NullOrEmpty_ShouldReturn500(int? errorCount)
    {
        Result<Unit, Error> result = errorCount switch
        {
            _ when errorCount is null => Result<Unit, Error>.Failure((Error[])null!),
            0 => Result<Unit, Error>.Failure(Array.Empty<Error>()),
            _ => throw new InvalidOperationException()
        };

        result.IsSuccess.ShouldBeFalse();
        result.Status.ShouldBe(500);
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Failure_MixedStatusCodes_ShouldReturnFailureWithMixedStatusCodesError()
    {
        var error1 = Error.BadRequest("Code1", "Desc1");
        var error2 = Error.NotFound("Code2", "Desc2");

        var result = Result<Unit, Error>.Failure(error1, error2);

        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldHaveSingleItem().Code.ShouldBe(ResultConstant.Failure.Result.MixedStatusCodes.Code);
    }

    #endregion
}
