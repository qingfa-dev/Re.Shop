using BuildingBlock.Kernel.Errors;
using BuildingBlock.Kernel.Metadata;
using BuildingBlock.Kernel.Results;

namespace BuildingBlock.Kernel.UnitTest.Results;

[Trait("Category", "Unit")]
public class ResultSpec
{
    [Fact]
    public void Constructor_With_Null_Errors_And_Status_Should_Default_To_Success_Ok()
    {
        var result = new Result(isSuccess: true, errors: null!, statusCode: null);

        result.IsSuccess.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
        result.StatusCode.ShouldBe(ResultConstant.StatusCode.Ok);
    }

    [Fact]
    public void Constructor_Failure_With_Null_Status_Should_Default_To_Internal_Server_Error()
    {
        var errors = new List<Error> { ResultStub.NotFoundError() };

        var result = new Result(isSuccess: false, errors: errors, statusCode: null);

        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldBe(errors);
        result.StatusCode.ShouldBe(ResultConstant.StatusCode.InternalServerError);
    }

    [Fact]
    public void Result_Equality_Same_State_Should_Be_Equal()
    {
        var left = Result.Fail(ResultStub.NotFoundError());
        var right = Result.Fail(ResultStub.NotFoundError());

        left.Equals(right).ShouldBeTrue();
        (left == right).ShouldBeTrue();
        (left != right).ShouldBeFalse();
        left.Equals((Result?)null).ShouldBeFalse();
    }

    [Fact]
    public void Result_Equality_When_State_Differs_Should_Not_Be_Equal()
    {
        var failure = Result.Fail(ResultStub.NotFoundError());
        var single = Result.Fail(ResultStub.ConflictError());
        var doubled = Result.Fail(ResultStub.ConflictError(), ResultStub.ConflictError());

        failure.Equals(Result.Ok()).ShouldBeFalse();
        failure.Equals(single).ShouldBeFalse();
        single.Equals(doubled).ShouldBeFalse();
    }

    [Fact]
    public void Result_Equality_Should_Exclude_Status_And_Metadata()
    {
        var left = Result.Fail(
                new List<Error> { ResultStub.NotFoundError() },
                ResultConstant.StatusCode.InternalServerError)
            .WithMetadata("k", "v");
        var right = Result.Fail(ResultStub.NotFoundError());

        left.StatusCode.ShouldNotBe(right.StatusCode);
        left.Metadata.ShouldNotBe(right.Metadata);
        left.Equals(right).ShouldBeTrue();
    }

    [Fact]
    public void Result_GetHashCode_Same_State_Should_Match()
    {
        var left = Result.Fail(ResultStub.NotFoundError());
        var right = Result.Fail(ResultStub.NotFoundError());

        left.GetHashCode().ShouldBe(right.GetHashCode());
    }

    [Fact]
    public void CopyWith_Without_Changes_Should_Preserve_State()
    {
        var original = Result.Ok();

        var copy = original.CopyWith();

        copy.IsSuccess.ShouldBeTrue();
        copy.StatusCode.ShouldBe(original.StatusCode);
        copy.Metadata.ShouldNotBeSameAs(original.Metadata);
    }

    [Fact]
    public void CopyWith_With_Explicit_Status_Should_Use_Provided_Status()
    {
        var original = Result.Ok();

        var copy = original.CopyWith(statusCode: ResultConstant.StatusCode.Created);

        copy.StatusCode.ShouldBe(ResultConstant.StatusCode.Created);
    }

    [Fact]
    public void CopyWith_To_Success_On_Failure_Should_Reset_Status_To_Ok()
    {
        var failure = Result.Fail(ResultStub.NotFoundError());

        var copy = failure.CopyWith(isSuccess: true);

        copy.IsSuccess.ShouldBeTrue();
        copy.Errors.ShouldBeEmpty();
        copy.StatusCode.ShouldBe(ResultConstant.StatusCode.Ok);
    }

    [Fact]
    public void CopyWith_Errors_On_Failure_Should_ReResolve_Status_From_New_Errors()
    {
        var failure = Result.Fail(ResultStub.NotFoundError());

        var copy = failure.CopyWith(errors: new List<Error> { ResultStub.ConflictError() });

        copy.StatusCode.ShouldBe(ResultConstant.StatusCode.Conflict);
        copy.Errors[0].Code.ShouldBe("test.conflict");
    }

    [Fact]
    public void CopyWith_Errors_On_Success_Should_Preserve_Status()
    {
        var copy = Result.Ok().CopyWith(errors: new List<Error> { ResultStub.NotFoundError() });

        copy.IsSuccess.ShouldBeTrue();
        copy.StatusCode.ShouldBe(ResultConstant.StatusCode.Ok);
        copy.Errors.Count.ShouldBe(1);
        copy.Errors[0].Code.ShouldBe("test.not_found");
    }

    [Fact]
    public void CopyWith_With_Metadata_Should_Use_Provided_Bag()
    {
        var original = Result.Ok().WithMetadata("source", "original");
        var replacement = MetadataDictionary.Create().With("source", "replacement");

        var copy = original.CopyWith(metadata: replacement);

        copy.Metadata.ShouldNotBeSameAs(replacement);
        copy.Metadata["source"].ShouldBe("replacement");
    }

    [Fact]
    public void CopyWith_Should_Copy_Failure_State_And_Clone_Errors_And_Metadata()
    {
        var original = Result.Ok().WithMetadata("source", "original");
        var errors = new List<Error> { ResultStub.NotFoundError() };

        var copy = original.CopyWith(isSuccess: false, errors: errors);

        copy.IsFailure.ShouldBeTrue();
        copy.StatusCode.ShouldBe(ResultConstant.StatusCode.NotFound);
        copy.Errors.ShouldBe(errors);
        copy.Errors.ShouldNotBeSameAs(errors);
        copy.Metadata.ShouldNotBeSameAs(original.Metadata);
        copy.Metadata["source"].ShouldBe("original");

        errors[0] = ResultStub.ConflictError();
        copy.Errors[0].Code.ShouldBe("test.not_found");
    }

    [Fact]
    public void Result_Should_Expose_State_And_Render_Success_And_Failure()
    {
        var success = Result.Ok();
        var failure = Result.Fail(ResultStub.NotFoundError());

        success.HasValue.ShouldBeTrue();
        success.IsSuccess.ShouldBeTrue();
        success.IsFailure.ShouldBeFalse();
        success.ToString().ShouldBe("Success");

        failure.HasValue.ShouldBeFalse();
        failure.IsSuccess.ShouldBeFalse();
        failure.IsFailure.ShouldBeTrue();
        failure.ToString().ShouldContain("test.not_found");
    }
}
