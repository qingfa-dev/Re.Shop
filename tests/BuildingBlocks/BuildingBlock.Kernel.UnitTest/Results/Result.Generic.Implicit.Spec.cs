using BuildingBlock.Kernel.Errors;
using BuildingBlock.Kernel.Results;

namespace BuildingBlock.Kernel.UnitTest.Results;

/// <summary>Tests for implicit conversion operators on <see cref="Result{TValue}"/>.</summary>
[Trait("Category", "Unit")]
public class ResultGenericImplicitSpec
{
    #region Success Conversion

    /// <summary>TValue should implicitly convert to generic success.</summary>
    [Fact]
    public void TValue_Should_Implicitly_Convert_To_Generic_Success()
    {
        // Act
        Result<string> result = "value";

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("value");
        result.StatusCode.ShouldBe(ResultConstant.StatusCode.Ok);
        result.Errors.ShouldBeEmpty();
    }

    /// <summary>Value types should implicitly convert to generic success.</summary>
    [Fact]
    public void ValueType_Should_Implicitly_Convert_To_Generic_Success()
    {
        // Act
        Result<int> result = 42;

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(42);
        result.StatusCode.ShouldBe(ResultConstant.StatusCode.Ok);
    }

    /// <summary>A null reference value should throw when converted to generic success.</summary>
    [Fact]
    public void NullValue_Should_Throw_When_Converted_To_Generic_Success()
    {
        // Arrange
        string value = null!;

        // Act
        Action act = () =>
        {
            Result<string> result = value;
            _ = result.Value;
        };

        // Assert
        Should.Throw<ArgumentException>(act);
    }

    #endregion

    #region Failure Conversion

    /// <summary>Error should implicitly convert to generic failure.</summary>
    [Fact]
    public void Error_Should_Implicitly_Convert_To_Generic_Failure()
    {
        // Arrange
        var error = ResultStub.NotFoundError();

        // Act
        Result<string> result = error;

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldBe(new[] { error });
        result.StatusCode.ShouldBe(ResultConstant.StatusCode.NotFound);
        result.Value.ShouldBeNull();
    }

    /// <summary>Error array should implicitly convert to generic failure.</summary>
    [Fact]
    public void ErrorArray_Should_Implicitly_Convert_To_Generic_Failure()
    {
        // Arrange
        var errors = new[] { ResultStub.NotFoundError() };

        // Act
        Result<string> result = errors;

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldBe(errors);
        result.StatusCode.ShouldBe(ResultConstant.StatusCode.NotFound);
    }

    /// <summary>Error list should implicitly convert to generic failure.</summary>
    [Fact]
    public void ErrorList_Should_Implicitly_Convert_To_Generic_Failure()
    {
        // Arrange
        List<Error> errors = [ResultStub.NotFoundError()];

        // Act
        Result<int> result = errors;

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldBe(errors);
        result.StatusCode.ShouldBe(ResultConstant.StatusCode.NotFound);
    }

    /// <summary>Multiple errors should be preserved by implicit conversion.</summary>
    [Fact]
    public void MultipleErrors_Should_Be_Preserved_By_Implicit_Conversion()
    {
        // Arrange
        var errors = new[]
        {
            ResultStub.NotFoundError(),
            Error.NotFound("test.other", "Another missing resource")
        };

        // Act
        Result<int> result = errors;

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.Count.ShouldBe(2);
        result.Errors.ShouldBe(errors);
        result.StatusCode.ShouldBe(ResultConstant.StatusCode.NotFound);
    }

    #endregion

    #region Failure Conversion Guards

    /// <summary>An empty error array should throw when converted.</summary>
    [Fact]
    public void EmptyErrorArray_Should_Throw_When_Converted()
    {
        // Arrange
        Error[] errors = [];

        // Act
        Action act = () =>
        {
            Result<int> result = errors;
            _ = result.StatusCode;
        };

        // Assert
        Should.Throw<ArgumentException>(act);
    }

    /// <summary>An empty error list should throw when converted.</summary>
    [Fact]
    public void EmptyErrorList_Should_Throw_When_Converted()
    {
        // Arrange
        List<Error> errors = [];

        // Act
        Action act = () =>
        {
            Result<int> result = errors;
            _ = result.StatusCode;
        };

        // Assert
        Should.Throw<ArgumentException>(act);
    }

    /// <summary>A null error list should throw when converted.</summary>
    [Fact]
    public void NullErrorList_Should_Throw_When_Converted()
    {
        // Arrange
        List<Error> errors = null!;

        // Act
        Action act = () =>
        {
            Result<int> result = errors;
            _ = result.StatusCode;
        };

        // Assert
        var ex = Should.Throw<ArgumentNullException>(act);
        ex.ParamName.ShouldBe("errors");
    }

    /// <summary>Mixed status codes in an error array should throw when converted.</summary>
    [Fact]
    public void MixedStatusErrors_Should_Throw_When_Converted()
    {
        // Arrange
        var errors = new[] { ResultStub.NotFoundError(), ResultStub.ConflictError() };

        // Act
        Action act = () =>
        {
            Result<int> result = errors;
            _ = result.StatusCode;
        };

        // Assert
        Should.Throw<ArgumentException>(act);
    }

    #endregion

    #region Direct Returns

    /// <summary>Direct returns should convert values and errors to generic results.</summary>
    [Fact]
    public void Direct_Returns_Should_Convert_Values_And_Errors_To_Generic_Results()
    {
        // Act
        var success = ReturnValue();
        var failure = ReturnError();
        var listFailure = ReturnErrorList();

        // Assert
        success.IsSuccess.ShouldBeTrue();
        success.Value.ShouldBe(42);
        failure.IsFailure.ShouldBeTrue();
        failure.Errors.ShouldContain(error => error.Code == "test.not_found");
        listFailure.IsFailure.ShouldBeTrue();
        listFailure.Errors.ShouldContain(error => error.Code == "test.not_found");

        static Result<int> ReturnValue() => 42;
        static Result<int> ReturnError() => ResultStub.NotFoundError();
        static Result<int> ReturnErrorList() => new List<Error> { ResultStub.NotFoundError() };
    }

    #endregion
}
