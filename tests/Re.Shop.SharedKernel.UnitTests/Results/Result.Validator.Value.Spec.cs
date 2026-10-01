using Shouldly;
namespace SharedKernel.UnitTests.Results;

public class ResultValueValidatorSpec
{
    #region ValidateValueForSuccess

    [Theory]
    [InlineData("hello")]
    [InlineData("")]
    public void ValidateValueForSuccess_SuccessWithNonNull_ShouldReturnNull(string value)
    {
        ResultValidator.ValidateValueForSuccess(value, true).ShouldBeNull();
    }

    [Fact]
    public void ValidateValueForSuccess_FailureWithNull_ShouldReturnNull()
    {
        ResultValidator.ValidateValueForSuccess<string>(null, false).ShouldBeNull();
    }

    [Fact]
    public void ValidateValueForSuccess_SuccessWithNull_ShouldReturnValueRequired()
    {
        var error = ResultValidator.ValidateValueForSuccess<string>(null, true);

        error!.Value.Code.ShouldBe(ResultResult.Failure.ValueRequired.Code);
    }

    #endregion

    #region ValidateValueForFailure

    [Fact]
    public void ValidateValueForFailure_FailureWithNull_ShouldReturnNull()
    {
        ResultValidator.ValidateValueForFailure<string>(null, false).ShouldBeNull();
    }

    [Fact]
    public void ValidateValueForFailure_SuccessWithNonNull_ShouldReturnNull()
    {
        ResultValidator.ValidateValueForFailure("hello", true).ShouldBeNull();
    }

    [Fact]
    public void ValidateValueForFailure_FailureWithNonNull_ShouldReturnValueNotAllowed()
    {
        var error = ResultValidator.ValidateValueForFailure("hello", false);

        error!.Value.Code.ShouldBe(ResultResult.Failure.ValueNotAllowed.Code);
    }

    #endregion

    #region ValidateStatusCode

    [Theory]
    [InlineData(100)]
    [InlineData(200)]
    [InlineData(201)]
    [InlineData(202)]
    [InlineData(204)]
    [InlineData(300)]
    [InlineData(400)]
    [InlineData(404)]
    [InlineData(500)]
    [InlineData(503)]
    [InlineData(599)]
    public void ValidateStatusCode_ValidStatus_ShouldReturnNull(int status)
    {
        ResultValidator.ValidateStatusCode(status).ShouldBeNull();
    }

    [Theory]
    [InlineData(99)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(600)]
    [InlineData(999)]
    public void ValidateStatusCode_InvalidStatus_ShouldReturnInvalidStatusCode(int status)
    {
        var error = ResultValidator.ValidateStatusCode(status);

        error.ShouldNotBeNull();
        error!.Value.Code.ShouldBe(ResultConstant.Failure.Status.InvalidStatusCode.Code);
        error!.Value.Message.ShouldBe($"Status code must be between 100 and 599, but was {status}.");
    }

    #endregion

    #region ValidateSuccessStatus

    [Theory]
    [InlineData(200)]
    [InlineData(201)]
    [InlineData(202)]
    [InlineData(204)]
    public void ValidateSuccessStatus_SuccessWith2xx_ShouldReturnNull(int status)
    {
        ResultValidator.ValidateSuccessStatus(status, true).ShouldBeNull();
    }

    [Theory]
    [InlineData(400)]
    [InlineData(404)]
    [InlineData(500)]
    public void ValidateSuccessStatus_FailureWith4xx_ShouldReturnNull(int status)
    {
        ResultValidator.ValidateSuccessStatus(status, false).ShouldBeNull();
    }

    [Theory]
    [InlineData(400)]
    [InlineData(404)]
    [InlineData(500)]
    public void ValidateSuccessStatus_SuccessWithNon2xx_ShouldReturnInvalidSuccessStatus(int status)
    {
        var error = ResultValidator.ValidateSuccessStatus(status, true);

        error.ShouldNotBeNull();
        error!.Value.Code.ShouldBe(ResultConstant.Failure.Status.InvalidSuccessStatus.Code);
        error!.Value.Message.ShouldBe($"Success result must have a status code between 200 and 299, but was {status}.");
    }

    #endregion

    #region ValidateFailureStatus

    [Theory]
    [InlineData(400)]
    [InlineData(404)]
    [InlineData(500)]
    [InlineData(503)]
    public void ValidateFailureStatus_FailureWith4xx5xx_ShouldReturnNull(int status)
    {
        ResultValidator.ValidateFailureStatus(status, false).ShouldBeNull();
    }

    [Theory]
    [InlineData(200)]
    [InlineData(201)]
    public void ValidateFailureStatus_SuccessWith2xx_ShouldReturnNull(int status)
    {
        ResultValidator.ValidateFailureStatus(status, true).ShouldBeNull();
    }

    [Theory]
    [InlineData(200)]
    [InlineData(201)]
    [InlineData(300)]
    public void ValidateFailureStatus_FailureWithNon4xx5xx_ShouldReturnInvalidFailureStatus(int status)
    {
        var error = ResultValidator.ValidateFailureStatus(status, false);

        error.ShouldNotBeNull();
        error!.Value.Code.ShouldBe(ResultConstant.Failure.Status.InvalidFailureStatus.Code);
        error!.Value.Message.ShouldBe($"Failure result must have a status code between 400 and 599, but was {status}.");
    }

    #endregion

    #region ValidateResult

    [Theory]
    [InlineData("hello", true, 200)]
    [InlineData("hello", true, 201)]
    [InlineData(null, false, 400)]
    [InlineData(null, false, 404)]
    public void ValidateResult_ValidState_ShouldReturnNull(string? value, bool isSuccess, int status)
    {
        ResultValidator.ValidateResult(value, isSuccess, status).ShouldBeNull();
    }

    [Fact]
    public void ValidateResult_SuccessWithNullValue_ShouldReturnValueRequired()
    {
        var error = ResultValidator.ValidateResult<string>(null, true, 200);

        error!.Value.Code.ShouldBe(ResultResult.Failure.ValueRequired.Code);
    }

    [Theory]
    [InlineData(99)]
    [InlineData(600)]
    public void ValidateResult_InvalidStatusCode_ShouldReturnInvalidStatusCode(int status)
    {
        var error = ResultValidator.ValidateResult("hello", true, status);

        error.ShouldNotBeNull();
        error!.Value.Code.ShouldBe(ResultConstant.Failure.Status.InvalidStatusCode.Code);
    }

    [Fact]
    public void ValidateResult_SuccessWithNon2xxStatus_ShouldReturnInvalidSuccessStatus()
    {
        var error = ResultValidator.ValidateResult("hello", true, 400);

        error.ShouldNotBeNull();
        error!.Value.Code.ShouldBe(ResultConstant.Failure.Status.InvalidSuccessStatus.Code);
    }

    [Fact]
    public void ValidateResult_FailureWithNon4xx5xxStatus_ShouldReturnInvalidFailureStatus()
    {
        var error = ResultValidator.ValidateResult<string>(null, false, 200);

        error.ShouldNotBeNull();
        error!.Value.Code.ShouldBe(ResultConstant.Failure.Status.InvalidFailureStatus.Code);
    }

    [Fact]
    public void ValidateResult_ValueTypeFailure_ShouldReturnNull()
    {
        ResultValidator.ValidateResult(0, false, 400).ShouldBeNull();
    }

    #endregion

}
