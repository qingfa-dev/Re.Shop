using Shouldly;
namespace SharedKernel.UnitTests.Results;

public class ResultValueSpec
{
    #region Create

    [Theory]
    [InlineData("hello", true, 200)]
    [InlineData("hello", true, 201)]
    [InlineData(null, false, 400)]
    [InlineData(null, false, 404)]
    public void Create_ShouldHaveCorrectProperties(string? value, bool isSuccess, int status)
    {
        var errors = isSuccess
            ? []
            : new List<Error> { Error.BadRequest("Code", "Desc") };

        var result = Result<string, Error>.Create(value, isSuccess, status, errors);

        result.IsSuccess.ShouldBe(isSuccess);
        result.Status.ShouldBe(status);
        result.Value.ShouldBe(value);
        result.Errors.Count.ShouldBe(isSuccess ? 0 : 1);
        result.Metadata.ShouldBeEmpty();
    }

    [Fact]
    public void Create_WithErrors_ShouldHaveCorrectProperties()
    {
        var errors = new List<Error> { Error.BadRequest("Code", "Desc") };

        var result = Result<string, Error>.Create(null, false, 400, errors);

        result.IsSuccess.ShouldBeFalse();
        result.Status.ShouldBe(400);
        result.Errors.Count.ShouldBe(1);
        result.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_NullErrors_ShouldDefaultToEmpty()
    {
        var result = Result<string, Error>.Create("hello", true, 200, null);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("hello");
        result.Errors.ShouldBeEmpty();
    }

    #endregion

    #region Success

    [Theory]
    [InlineData("hello", 200)]
    [InlineData("hello", 201)]
    [InlineData("hello", 202)]
    [InlineData("hello", 204)]
    public void Success_ShouldHaveCorrectProperties(string value, int status)
    {
        var result = Result<string, Error>.Success(value, status);

        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Status.ShouldBe(status);
        result.Value.ShouldBe(value);
        result.HasValue.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
        result.Metadata.ShouldBeEmpty();
    }

    [Fact]
    public void Success_NullValue_ShouldReturnFailureWithValueRequired()
    {
        var result = Result<string, Error>.Success(null!);

        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldHaveSingleItem().Code.ShouldBe(ResultConstant.Failure.Value.Required.Code);
    }

    [Fact]
    public void Ok_ShouldReturn200()
    {
        var result = Result<string, Error>.Ok("hello");

        result.IsSuccess.ShouldBeTrue();
        result.Status.ShouldBe(200);
        result.Value.ShouldBe("hello");
    }

    [Fact]
    public void Created_ShouldReturn201()
    {
        var result = Result<string, Error>.Created("hello");

        result.IsSuccess.ShouldBeTrue();
        result.Status.ShouldBe(201);
        result.Value.ShouldBe("hello");
    }

    [Fact]
    public void Created_NullValue_ShouldReturnFailureWithValueRequired()
    {
        var result = Result<string, Error>.Created(null!);

        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldHaveSingleItem().Code.ShouldBe(ResultConstant.Failure.Value.Required.Code);
    }

    [Fact]
    public void Accepted_ShouldReturn202()
    {
        var result = Result<string, Error>.Accepted("hello");

        result.IsSuccess.ShouldBeTrue();
        result.Status.ShouldBe(202);
        result.Value.ShouldBe("hello");
    }

    [Fact]
    public void Accepted_NullValue_ShouldReturnFailureWithValueRequired()
    {
        var result = Result<string, Error>.Accepted(null!);

        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldHaveSingleItem().Code.ShouldBe(ResultConstant.Failure.Value.Required.Code);
    }

    [Fact]
    public void NoContent_ShouldReturn204()
    {
        var result = Result<string, Error>.NoContent("hello");

        result.IsSuccess.ShouldBeTrue();
        result.Status.ShouldBe(204);
        result.Value.ShouldBe("hello");
    }

    [Fact]
    public void NoContent_NullValue_ShouldReturnFailureWithValueRequired()
    {
        var result = Result<string, Error>.NoContent(null!);

        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldHaveSingleItem().Code.ShouldBe(ResultConstant.Failure.Value.Required.Code);
    }

    #endregion

    #region Failure

    [Fact]
    public void Failure_SingleError_ShouldHaveCorrectProperties()
    {
        var error = Error.BadRequest("Code", "Desc");
        var result = Result<string, Error>.Failure(error);

        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Status.ShouldBe(400);
        result.Value.ShouldBeNull();
        result.HasValue.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
        result.Errors.ShouldBe(new[] { error });
        result.Metadata.ShouldBeEmpty();
    }

    [Fact]
    public void Failure_MultipleErrors_ShouldHaveCorrectProperties()
    {
        var error1 = Error.BadRequest("Code1", "Desc1");
        var error2 = Error.BadRequest("Code2", "Desc2");
        var result = Result<string, Error>.Failure(error1, error2);

        result.IsSuccess.ShouldBeFalse();
        result.Status.ShouldBe(400);
        result.Errors.Count.ShouldBe(2);
        result.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void Failure_IEnumerableErrors_ShouldHaveCorrectProperties()
    {
        var error = Error.BadRequest("Code", "Desc");
        var result = Result<string, Error>.Failure(new List<Error> { error });

        result.IsSuccess.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
    }

    [Fact]
    public void Failure_NullErrorsArray_ShouldReturn500Status()
    {
        var result = Result<string, Error>.Failure((Error[])null!);

        result.IsSuccess.ShouldBeFalse();
        result.Status.ShouldBe(500);
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Failure_NullErrorsEnumerable_ShouldReturn500Status()
    {
        var result = Result<string, Error>.Failure((IEnumerable<Error>)null!);

        result.IsSuccess.ShouldBeFalse();
        result.Status.ShouldBe(500);
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Failure_EmptyArray_ShouldReturn500Status()
    {
        var result = Result<string, Error>.Failure(Array.Empty<Error>());

        result.IsSuccess.ShouldBeFalse();
        result.Status.ShouldBe(500);
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Failure_EmptyList_ShouldReturn500Status()
    {
        var result = Result<string, Error>.Failure(new List<Error>());

        result.IsSuccess.ShouldBeFalse();
        result.Status.ShouldBe(500);
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Failure_MixedStatusCodes_ShouldReturnFailureWithMixedStatusCodesError()
    {
        var error1 = Error.BadRequest("Code1", "Desc1");
        var error2 = Error.NotFound("Code2", "Desc2");

        var result = Result<string, Error>.Failure(error1, error2);

        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldHaveSingleItem().Code.ShouldBe(ResultConstant.Failure.Result.MixedStatusCodes.Code);
    }

    #endregion

    #region From

    [Fact]
    public void From_Value_ShouldCreateSuccessResult()
    {
        var result = Result<string, Error>.From("hello");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("hello");
    }

    [Fact]
    public void From_Error_ShouldCreateFailureResult()
    {
        var error = Error.BadRequest("Code", "Desc");
        var result = Result<string, Error>.From(error);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldBe(new[] { error });
    }

    #endregion

    #region ToString

    [Fact]
    public void ToString_Success_ShouldIncludeValue()
    {
        var result = Result<string, Error>.Ok("hello");
        result.ToString().ShouldContain("Success");
        result.ToString().ShouldContain("hello");
    }

    [Fact]
    public void ToString_Failure_ShouldIncludeErrorCode()
    {
        var error = Error.BadRequest("Test.Code", "Desc");
        var result = Result<string, Error>.Failure(error);
        result.ToString().ShouldContain("Failure");
        result.ToString().ShouldContain("Test.Code");
    }

    #endregion

    #region Interface & Metadata

    [Fact]
    public void ValueResult_IResult_ShouldImplementInterface()
    {
        var result = Result<string, Error>.Ok("hello");
        result.ShouldBeAssignableTo<IResult<string, Error>>();
        result.ShouldBeAssignableTo<IValueOf<string>>();
    }

    [Fact]
    public void WithMetadata_ShouldSetMetadata()
    {
        var metadata = new Dictionary<string, object?> { ["key"] = "value" };
        var result = Result<string, Error>.Ok("hello").WithAdditionalMetadata(metadata);

        result.Metadata!.ShouldContainKey("key");
        result.Metadata!["key"].ShouldBe("value");
        result.Value.ShouldBe("hello");
    }

    [Fact]
    public void WithAdditionalMetadata_OnFailure_ShouldSetMetadata()
    {
        var error = Error.BadRequest("Code", "Desc");
        var metadata = new Dictionary<string, object?> { ["key"] = "value" };
        var result = Result<string, Error>.Failure(error).WithAdditionalMetadata(metadata);

        result.Metadata!.ShouldContainKey("key");
        result.IsSuccess.ShouldBeFalse();
    }

    [Theory]
    [InlineData("key1", "value1")]
    [InlineData("key2", 42)]
    public void Created_WithMetadata_ShouldContainMetadata(string key, object? value)
    {
        var metadata = new Dictionary<string, object?> { [key] = value };
        var result = Result<string, Error>.Created("hello").WithAdditionalMetadata(metadata);

        result.Status.ShouldBe(201);
        result.Metadata!.ShouldContainKey(key);
    }

    [Theory]
    [InlineData("key1", "value1")]
    [InlineData("key2", 42)]
    public void Accepted_WithMetadata_ShouldContainMetadata(string key, object? value)
    {
        var metadata = new Dictionary<string, object?> { [key] = value };
        var result = Result<string, Error>.Accepted("hello").WithAdditionalMetadata(metadata);

        result.Status.ShouldBe(202);
        result.Metadata!.ShouldContainKey(key);
    }

    [Theory]
    [InlineData("key1", "value1")]
    [InlineData("key2", 42)]
    public void NoContent_WithMetadata_ShouldContainMetadata(string key, object? value)
    {
        var metadata = new Dictionary<string, object?> { [key] = value };
        var result = Result<string, Error>.NoContent("hello").WithAdditionalMetadata(metadata);

        result.Status.ShouldBe(204);
        result.Metadata!.ShouldContainKey(key);
    }

    #endregion
}