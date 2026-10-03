using BuildingBlock.Kernel.Errors;
using BuildingBlock.Kernel.Metadata;
using BuildingBlock.Kernel.Results;

namespace BuildingBlock.Kernel.UnitTest;

[Trait("Category", "Unit")]
public class ResultGenericSpec
{
    [Fact]
    public void CopyWith_Without_Changes_Should_Preserve_Value_And_Status()
    {
        var original = Result<int>.Ok(5);

        var copy = original.CopyWith();

        copy.Value.ShouldBe(5);
        copy.StatusCode.ShouldBe(ResultConstant.StatusCode.Ok);
        copy.Metadata.ShouldNotBeSameAs(original.Metadata);
    }

    [Fact]
    public void CopyWith_With_Explicit_Status_Should_Use_Provided_Status()
    {
        var copy = Result<int>.Ok(5).CopyWith(statusCode: ResultConstant.StatusCode.Created);

        copy.StatusCode.ShouldBe(ResultConstant.StatusCode.Created);
    }

    [Fact]
    public void CopyWith_Errors_On_Failure_Should_ReResolve_Status_From_New_Errors()
    {
        var failure = Result<int>.Fail(ResultStub.NotFoundError());

        var copy = failure.CopyWith(errors: new IError[] { ResultStub.ConflictError() });

        copy.StatusCode.ShouldBe(ResultConstant.StatusCode.Conflict);
        copy.Errors[0].Code.ShouldBe("test.conflict");
    }

    [Fact]
    public void CopyWith_Value_With_Explicit_Status_And_Metadata_Should_Apply_Them()
    {
        var original = Result<int>.Ok(5);
        var replacement = MetadataDictionary.Create().With("k", "v");

        var copy = original.CopyWith(
            value: 7,
            statusCode: ResultConstant.StatusCode.Created,
            metadata: replacement);

        copy.Value.ShouldBe(7);
        copy.StatusCode.ShouldBe(ResultConstant.StatusCode.Created);
        copy.Metadata.ShouldNotBeSameAs(replacement);
        copy.Metadata["k"].ShouldBe("v");
    }

    [Fact]
    public void CopyWith_Value_On_Failure_Should_Clear_Value_And_Use_New_Errors()
    {
        var success = Result<int>.Ok(5);

        var copy = success.CopyWith(value: 0, isSuccess: false, errors: new IError[] { ResultStub.NotFoundError() });

        copy.IsFailure.ShouldBeTrue();
        copy.Value.ShouldBe(default);
        copy.StatusCode.ShouldBe(ResultConstant.StatusCode.NotFound);
    }

    [Fact]
    public void CopyWith_Value_Errors_On_Failure_Should_ReResolve_Status()
    {
        var failure = Result<int>.Fail(ResultStub.NotFoundError());

        var copy = failure.CopyWith(value: 0, errors: new IError[] { ResultStub.ConflictError() });

        copy.IsFailure.ShouldBeTrue();
        copy.StatusCode.ShouldBe(ResultConstant.StatusCode.Conflict);
    }

    [Fact]
    public void Result_Generic_Equality_Should_Compare_State_And_Value()
    {
        var left = Result<int>.Ok(5);
        var right = Result<int>.Ok(5);

        left.Equals(right).ShouldBeTrue();
        (left == right).ShouldBeTrue();
        left.Equals(Result<int>.Ok(6)).ShouldBeFalse();
        left.Equals(Result<int>.Fail(ResultStub.NotFoundError())).ShouldBeFalse();
        left.Equals((Result<int>?)null).ShouldBeFalse();
    }

    [Fact]
    public void Result_Generic_GetHashCode_Same_State_Should_Match()
    {
        var left = Result<int>.Fail(ResultStub.NotFoundError());
        var right = Result<int>.Fail(ResultStub.NotFoundError());

        left.GetHashCode().ShouldBe(right.GetHashCode());
    }

    [Fact]
    public void CopyWith_Errors_On_Success_Should_Preserve_Status_And_Value()
    {
        var copy = Result<int>.Ok(5).CopyWith(errors: new IError[] { ResultStub.NotFoundError() });

        copy.IsSuccess.ShouldBeTrue();
        copy.Value.ShouldBe(5);
        copy.StatusCode.ShouldBe(ResultConstant.StatusCode.Ok);
        copy.Errors.Count.ShouldBe(1);
        copy.Errors[0].Code.ShouldBe("test.not_found");
    }

    [Fact]
    public void CopyWith_Value_Errors_On_Success_Should_Preserve_Status()
    {
        var copy = Result<int>.Ok(5).CopyWith(value: 7, errors: new IError[] { ResultStub.NotFoundError() });

        copy.IsSuccess.ShouldBeTrue();
        copy.Value.ShouldBe(7);
        copy.StatusCode.ShouldBe(ResultConstant.StatusCode.Ok);
        copy.Errors.Count.ShouldBe(1);
    }

    [Fact]
    public void CopyWith_Value_On_Failure_Without_Errors_Should_Preserve_Errors_And_Status()
    {
        var failure = Result<int>.Fail(ResultStub.NotFoundError());

        var copy = failure.CopyWith(value: 0);

        copy.IsFailure.ShouldBeTrue();
        copy.Value.ShouldBe(default);
        copy.StatusCode.ShouldBe(ResultConstant.StatusCode.NotFound);
        copy.Errors.Count.ShouldBe(1);
        copy.Errors[0].Code.ShouldBe("test.not_found");
    }

    [Fact]
    public void CopyWith_With_Metadata_Should_Clone_Provided_Bag()
    {
        var replacement = MetadataDictionary.Create().With("k", "v");

        var copy = Result<int>.Ok(5).CopyWith(metadata: replacement);

        copy.Metadata.ShouldNotBeSameAs(replacement);
        copy.Metadata["k"].ShouldBe("v");
    }

    [Fact]
    public void CopyWith_Should_Replace_Value_And_Clone_Metadata()
    {
        var original = Result<int>.Ok(5).WithMetadata("source", "original");

        var copy = original.CopyWith(value: 10);

        copy.Value.ShouldBe(10);
        copy.StatusCode.ShouldBe(original.StatusCode);
        copy.Metadata.ShouldNotBeSameAs(original.Metadata);
        copy.Metadata["source"].ShouldBe("original");
    }

    [Fact]
    public void CopyWith_Should_Clear_Value_When_Changing_To_Failure()
    {
        var original = Result<int>.Ok(5);
        var errors = new[] { ResultStub.NotFoundError() };

        var copy = original.CopyWith(isSuccess: false, errors: errors);

        copy.IsFailure.ShouldBeTrue();
        copy.Value.ShouldBe(default);
        copy.StatusCode.ShouldBe(ResultConstant.StatusCode.NotFound);
        copy.TryGetValue(out _).ShouldBeFalse();
    }

    [Fact]
    public void CopyWith_Should_Require_Replacement_Value_When_Changing_Failure_To_Success()
    {
        var failure = Result<int>.Fail(ResultStub.NotFoundError());

        var act = () => failure.CopyWith(isSuccess: true);

        Should.Throw<ArgumentException>(act);
    }

    [Fact]
    public void CopyWith_With_Replacement_Value_Should_Allow_Failure_To_Success_Transition()
    {
        var failure = Result<int>.Fail(ResultStub.NotFoundError());

        var copy = failure.CopyWith(value: 42, isSuccess: true);

        copy.IsSuccess.ShouldBeTrue();
        copy.Value.ShouldBe(42);
        copy.Errors.ShouldBeEmpty();
        copy.StatusCode.ShouldBe(ResultConstant.StatusCode.Ok);
    }

    [Fact]
    public void TryGetValue_And_ToString_Should_Reflect_Result_State()
    {
        var success = Result<int>.Ok(42);
        var failure = Result<int>.Fail(ResultStub.NotFoundError());

        success.TryGetValue(out var value).ShouldBeTrue();
        value.ShouldBe(42);
        success.ToString().ShouldBe("Success: 42");

        failure.TryGetValue(out _).ShouldBeFalse();
        failure.ToString().ShouldContain("test.not_found");
        failure.ToString().ShouldContain("test.not_found");
    }

    [Fact]
    public void Constructor_Should_Reject_A_Success_Without_A_Value()
    {
        var act = () => new Result<string>(isSuccess: true, value: null!);

        Should.Throw<ArgumentException>(act);
    }
}
