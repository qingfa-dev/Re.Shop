namespace SharedKernel.UnitTests.Structures.Results;

public class ResultUnitSpec
{
    #region Create

    [Theory]
    [InlineData(true, 200)]
    [InlineData(true, 201)]
    [InlineData(true, 202)]
    [InlineData(true, 204)]
    [InlineData(false, 400)]
    [InlineData(false, 404)]
    public void Create_ShouldHaveCorrectProperties(bool isSuccess, int status)
    {
        var errors = isSuccess
            ? []
            : new List<Error> { Error.BadRequest("Code", "Desc") };

        var result = Result<Unit, Error>.Create(isSuccess, status, errors);

        result.IsSuccess.ShouldBe(isSuccess);
        result.Status.ShouldBe(status);
        result.Errors.Count.ShouldBe(isSuccess ? 0 : 1);
        result.Metadata.ShouldBeEmpty();
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
    public void Success_ShouldHaveCorrectProperties(int status)
    {
        var result = Result<Unit, Error>.Success(Unit.Value, status);

        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Status.ShouldBe(status);
        result.Errors.ShouldBeEmpty();
        result.Metadata.ShouldBeEmpty();
    }

    [Fact]
    public void Success_Default_ShouldReturn200()
    {
        var result = Result<Unit, Error>.Success(Unit.Value);

        result.IsSuccess.ShouldBeTrue();
        result.Status.ShouldBe(200);
    }

    #endregion

    #region Failure Factories

    [Theory]
    [InlineData("Code1", "Desc1")]
    [InlineData("Test.Code", "Description")]
    [InlineData("X", "Y")]
    public void Failure_SingleError_ShouldHaveCorrectProperties(string code, string desc)
    {
        var error = Error.BadRequest(code, desc);
        var result = Result<Unit, Error>.Failure(error);

        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Status.ShouldBe(400);
        result.Errors.Count.ShouldBe(1);
        result.Errors.ShouldBe(new[] { error });
        result.Metadata.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]
    public void Failure_MultipleErrors_ShouldHaveCorrectProperties(int errorCount)
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
    public void Failure_NullOrEmpty_ShouldReturn500Status(int? errorCount)
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

    [Fact]
    public void Failure_ExceedsMaxErrors_ShouldReturnFailureWithExceedsMaxErrorsError()
    {
        var errors = Enumerable.Repeat(
            Error.BadRequest("Code", "Desc"),
            ResultConstant.Constraint.Errors.MaxCount + 1).ToArray();

        var result = Result<Unit, Error>.Failure(errors);

        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldHaveSingleItem().Code.ShouldBe(ResultConstant.Failure.Errors.ExceedsMaxRange.Code);
    }

    #endregion

    #region ToString

    [Fact]
    public void ToString_Success_ShouldReturnSuccessString()
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);
        result.ToString().ShouldBe($"Success({200})");
    }

    [Fact]
    public void ToString_Failure_ShouldReturnFailureString()
    {
        var error = Error.BadRequest("Test.Code", "Desc");
        var result = Result<Unit, Error>.Failure(error);
        result.ToString().ShouldContain("Failure");
        result.ToString().ShouldContain($"{400}");
        result.ToString().ShouldContain("Test.Code");
    }

    #endregion

    #region Interface & Metadata

    [Fact]
    public void Result_IResult_ShouldImplementInterface()
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);
        result.ShouldBeAssignableTo<IResult<Unit, Error>>();
    }

    [Theory]
    [InlineData("key1", "value1")]
    [InlineData("key2", 42)]
    public void Created_WithMetadata_ShouldContainMetadata(string key, object? value)
    {
        Dictionary<string, object?> metadata = new() { [key] = value };
        var result = Result<Unit, Error>.Created(Unit.Value).WithAdditionalMetadata(metadata);

        result.Status.ShouldBe(201);
        result.Metadata!.ShouldContainKey(key);
    }

    [Theory]
    [InlineData("key1", "value1")]
    [InlineData("key2", 42)]
    public void Accepted_WithMetadata_ShouldContainMetadata(string key, object? value)
    {
        Dictionary<string, object?> metadata = new() { [key] = value };
        var result = Result<Unit, Error>.Accepted(Unit.Value).WithAdditionalMetadata(metadata);

        result.Status.ShouldBe(202);
        result.Metadata!.ShouldContainKey(key);
    }

    [Theory]
    [InlineData("key1", "value1")]
    [InlineData("key2", 42)]
    public void NoContent_WithMetadata_ShouldContainMetadata(string key, object? value)
    {
        Dictionary<string, object?> metadata = new() { [key] = value };
        var result = Result<Unit, Error>.NoContent(Unit.Value).WithAdditionalMetadata(metadata);

        result.Status.ShouldBe(204);
        result.Metadata!.ShouldContainKey(key);
    }

    [Fact]
    public void WithAdditionalMetadata_ShouldSetMetadata()
    {
        Dictionary<string, object?> metadata = new() { ["key"] = "value" };
        var result = Result<Unit, Error>.Ok(Unit.Value).WithAdditionalMetadata(metadata);

        result.Metadata!.ShouldContainKey("key");
        result.Metadata!["key"].ShouldBe("value");
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void WithAdditionalMetadata_OnFailure_ShouldSetMetadata()
    {
        var error = Error.BadRequest("Code", "Desc");
        Dictionary<string, object?> metadata = new() { ["key"] = "value" };
        var result = Result<Unit, Error>.Failure(error).WithAdditionalMetadata(metadata);

        result.Metadata!.ShouldContainKey("key");
        result.IsSuccess.ShouldBeFalse();
    }

    #endregion
}
