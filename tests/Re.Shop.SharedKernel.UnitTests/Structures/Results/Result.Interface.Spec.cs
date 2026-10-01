namespace SharedKernel.UnitTests.Structures.Results;

public class ResultInterfaceSpec
{
    #region IResult

    [Fact]
    public void IResult_ShouldExposeIsSuccess()
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);

        result.IsSuccess.ShouldBeTrue();
        ((IResult<Unit, Error>)result).IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void IResult_ShouldExposeStatus()
    {
        var result = Result<Unit, Error>.Created(Unit.Value);

        result.Status.ShouldBe(201);
        ((IResult<Unit, Error>)result).Status.ShouldBe(201);
    }

    [Fact]
    public void IResult_ShouldExposeErrors()
    {
        var error = Error.BadRequest("Code", "Desc");
        var result = Result<Unit, Error>.Failure(error);

        result.Errors.Count.ShouldBe(1);
        ((IResult<Unit, Error>)result).Errors.ShouldBe(result.Errors);
    }

    [Fact]
    public void IResult_ShouldExposeMetadata()
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);

        result.Metadata.ShouldNotBeNull();
        ((IResult<Unit, Error>)result).Metadata.ShouldBeSameAs(result.Metadata);
    }

    [Fact]
    public void IResult_Implicts_ShouldBeAssignable()
    {
        var result = Result<Unit, Error>.Ok(Unit.Value);

        result.ShouldBeAssignableTo<IResult<Unit, Error>>();
    }

    [Fact]
    public void IResult_OnFailure_ShouldHaveCorrectProperties()
    {
        var error = Error.BadRequest("Code", "Desc");
        var result = Result<Unit, Error>.Failure(error);

        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        ((IResult<Unit, Error>)result).IsSuccess.ShouldBeFalse();
    }

    #endregion
}
