using BuildingBlock.Application.Results;
using BuildingBlock.Kernel.Errors;

namespace BuildingBlock.Application.UnitTest.Results;

public sealed class ApplicationResultSpec
{
    [Fact]
    public void ValidationFailed_Should_Map_Property_And_Message_To_Unprocessable_Error()
    {
        var error = ApplicationResult.Failure.ValidationFailed("Value", "Value is required.");

        error.Code.ShouldBe("Application.Validation.Failed");
        error.Status.ShouldBe(ErrorConstant.StatusCode.UnprocessableEntity);
        error.Message.ShouldBe("Value: Value is required.");
    }

    [Fact]
    public void ValidationFailed_Should_Omit_Blank_Property_Path()
    {
        var error = ApplicationResult.Failure.ValidationFailed(" ", "Request is invalid.");

        error.Message.ShouldBe("Request is invalid.");
    }

    [Fact]
    public void ValidationFailed_Should_Respect_Kernel_Message_Length_Limit()
    {
        var message = new string('x', ErrorConstant.Constraint.Message.MaxLength + 100);

        var error = ApplicationResult.Failure.ValidationFailed("Value", message);

        error.Message.Length.ShouldBe(ErrorConstant.Constraint.Message.MaxLength);
    }
}
