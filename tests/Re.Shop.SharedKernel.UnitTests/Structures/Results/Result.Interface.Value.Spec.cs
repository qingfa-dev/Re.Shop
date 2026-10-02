namespace SharedKernel.UnitTests.Structures.Results;

public class ResultInterfaceValueSpec
{
    #region IValueOf

    [Fact]
    public void IValueOf_ShouldExposeValue()
    {
        var result = Result<string, Error>.Ok("hello");

        ((IValueOf<string>)result).Value.ShouldBe("hello");
    }

    [Fact]
    public void IValueOf_Value_OnFailure_ShouldReturnDefault()
    {
        var error = Error.BadRequest("Code", "Desc");
        var result = Result<string, Error>.Failure(error);

        ((IValueOf<string>)result).Value.ShouldBeNull();
    }

    #endregion

    #region IResult

    [Fact]
    public void IResult_ShouldBeAssignableToIResultTError()
    {
        var result = Result<string, Error>.Ok("hello");

        result.ShouldBeAssignableTo<IResult<string, Error>>();
    }

    [Fact]
    public void IResult_ShouldBeAssignableToIValueOfTValue()
    {
        var result = Result<string, Error>.Ok("hello");

        result.ShouldBeAssignableTo<IValueOf<string>>();
    }

    [Fact]
    public void IResult_GenericConstraint_ShouldRequireIError()
    {
        typeof(IError).IsAssignableFrom(
            typeof(IResult<string, Error>).GetGenericArguments()[1]).ShouldBeTrue();
    }

    [Fact]
    public void IResult_InheritanceChain_ShouldBeCorrect()
    {
        typeof(IResult<string, Error>)
            .GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericArguments().Length == 1)
            .ShouldBeTrue();

        typeof(IValueOf<string>)
            .IsAssignableFrom(typeof(IResult<string, Error>))
            .ShouldBeTrue();
    }

    #endregion
}
