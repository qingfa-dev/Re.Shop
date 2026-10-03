using BuildingBlock.Kernel.Errors;
using BuildingBlock.Kernel.Results;

namespace BuildingBlock.Kernel.UnitTest.Results;

/// <summary>Tests for <see cref="Result{TValue}"/> factory methods.</summary>
[Trait("Category", "Unit")]
public class ResultGenericFactorySpec
{
    #region Success

    /// <summary>Success factories should use their standard status.</summary>
    [Fact]
    public void Success_Factories_Should_Use_Their_Standard_Status()
    {
        // Act & Assert
        Result<int>.Ok(1).StatusCode.ShouldBe(ResultConstant.StatusCode.Ok);
        Result<int>.Created(2).StatusCode.ShouldBe(ResultConstant.StatusCode.Created);
        Result<int>.Accepted(3).StatusCode.ShouldBe(ResultConstant.StatusCode.Accepted);
    }

    /// <summary>Success with explicit status should use the provided status.</summary>
    [Fact]
    public void Success_With_Explicit_Status_Should_Use_Provided_Status()
    {
        // Act
        var result = Result<int>.Ok(1, ResultConstant.StatusCode.Accepted);

        // Assert
        result.StatusCode.ShouldBe(ResultConstant.StatusCode.Accepted);
        result.Value.ShouldBe(1);
    }

    #endregion

    #region Failure

    /// <summary>Failure without explicit status should resolve status from error.</summary>
    [Fact]
    public void Failure_Without_Explicit_Status_Should_Resolve_Status_From_Error()
    {
        // Arrange
        var result = Result<int>.Fail(ResultStub.NotFoundError());

        // Act & Assert
        result.IsFailure.ShouldBeTrue();
        result.StatusCode.ShouldBe(ResultConstant.StatusCode.NotFound);
        result.TryGetValue(out _).ShouldBeFalse();
    }

    /// <summary>Failure with explicit status should use the provided status.</summary>
    [Fact]
    public void Failure_With_Explicit_Status_Should_Use_Provided_Status()
    {
        // Arrange
        var result = Result<int>.Fail(
            new[] { ResultStub.NotFoundError() },
            ResultConstant.StatusCode.InternalServerError);

        // Assert
        result.StatusCode.ShouldBe(ResultConstant.StatusCode.InternalServerError);
    }

    /// <summary>Success factory should reject failure status code.</summary>
    [Fact]
    public void Success_Factory_Should_Reject_Failure_Status_Code()
    {
        // Act
        var act = () => Result<int>.Success(1, ResultConstant.StatusCode.NotFound);

        // Assert
        Should.Throw<ArgumentException>(act);
    }

    /// <summary>Failure factory should reject success status code.</summary>
    [Fact]
    public void Failure_Factory_Should_Reject_Success_Status_Code()
    {
        // Act
        var act = () => Result<int>.Fail(
            new[] { ResultStub.NotFoundError() },
            ResultConstant.StatusCode.Ok);

        // Assert
        Should.Throw<ArgumentException>(act);
    }

    /// <summary>Failure factory should reject empty error list.</summary>
    [Fact]
    public void Failure_Factory_Should_Reject_Empty_Error_List()
    {
        // Act
        var act = () => Result.Fail(Array.Empty<Error>());

        // Assert
        Should.Throw<ArgumentException>(act);
    }

    /// <summary>Failure enumerable overloads should create failures.</summary>
    [Fact]
    public void Failure_Enumerable_Overloads_Should_Create_Failures()
    {
        // Arrange
        var errors = new Error[] { ResultStub.NotFoundError() };

        // Act
        var resolved = Result<string>.Failure(errors);
        var explicitStatus = Result<string>.Failure(errors, ResultConstant.StatusCode.ServiceUnavailable);

        // Assert
        resolved.IsFailure.ShouldBeTrue();
        resolved.StatusCode.ShouldBe(ResultConstant.StatusCode.NotFound);
        explicitStatus.StatusCode.ShouldBe(ResultConstant.StatusCode.ServiceUnavailable);
        explicitStatus.Errors.ShouldBe(errors);
    }

    /// <summary>Generic fail with non-list enumerable should materialize errors.</summary>
    [Fact]
    public void Generic_Fail_With_Non_List_Enumerable_Should_Materialize_Errors()
    {
        // Act
        var result = Result<string>.Fail(Enumerate(ResultStub.NotFoundError()));

        // Assert
        result.Errors.Count.ShouldBe(1);
        result.Errors[0].Code.ShouldBe("test.not_found");
    }

    #endregion

    private static IEnumerable<Error> Enumerate(Error error)
    {
        yield return error;
    }
}