using Shouldly;
namespace SharedKernel.UnitTests.Results;

public class ResultValidatorSpec
{
    #region ValidateErrors

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void ValidateErrors_ValidErrors_ShouldReturnNull(int count)
    {
        var errors = Enumerable.Range(0, count)
            .Select(i => Error.BadRequest($"Code{i}", $"Desc{i}"))
            .ToList();

        ResultValidator.ValidateErrors(errors).ShouldBeNull();
    }

    [Fact]
    public void ValidateErrors_EmptyList_ShouldReturnEmptyErrors()
    {
        var errors = new List<Error>();

        var error = ResultValidator.ValidateErrors(errors);

        error!.Value.Code.ShouldBe(ResultResult.EmptyErrors.Code);
    }

    [Fact]
    public void ValidateErrors_ExceedsMaxErrors_ShouldReturnExceedsMaxErrors()
    {
        var errors = Enumerable.Repeat(
            Error.BadRequest("Code", "Desc"),
            ResultConstant.Constraint.Errors.MaxCount + 1).ToList();

        var error = ResultValidator.ValidateErrors(errors);

        error!.Value.Code.ShouldBe(ResultResult.ExceedsMaxErrors.Code);
    }

    [Fact]
    public void ValidateErrors_MixedStatusCodes_ShouldReturnMixedStatusCodes()
    {
        var errors = new List<Error>
        {
            Error.BadRequest("Code1", "Desc1"),
            Error.NotFound("Code2", "Desc2")
        };

        var error = ResultValidator.ValidateErrors(errors);

        error!.Value.Code.ShouldBe(ResultResult.MixedStatusCodes.Code);
    }

    #endregion

    #region ValidateFailureHasErrors

    [Fact]
    public void ValidateFailureHasErrors_Success_ShouldReturnNull()
    {
        ResultValidator.ValidateFailureHasErrors<Error>(true, []).ShouldBeNull();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    public void ValidateFailureHasErrors_FailureWithErrors_ShouldReturnNull(int errorCount)
    {
        var errors = Enumerable.Range(0, errorCount)
            .Select(i => Error.BadRequest($"Code{i}", $"Desc{i}"))
            .ToList();

        ResultValidator.ValidateFailureHasErrors(false, errors).ShouldBeNull();
    }

    [Fact]
    public void ValidateFailureHasErrors_FailureWithEmpty_ShouldReturnFailureWithoutErrors()
    {
        var errors = new List<Error>();

        var error = ResultValidator.ValidateFailureHasErrors(false, errors);

        error!.Value.Code.ShouldBe(ResultResult.FailureWithoutErrors.Code);
    }

    [Fact]
    public void ValidateFailureHasErrors_FailureWithNull_ShouldReturnFailureWithoutErrors()
    {
        var error = ResultValidator.ValidateFailureHasErrors<Error>(false, null);

        error!.Value.Code.ShouldBe(ResultResult.FailureWithoutErrors.Code);
    }

    #endregion
}