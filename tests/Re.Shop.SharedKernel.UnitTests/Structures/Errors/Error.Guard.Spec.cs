namespace SharedKernel.UnitTests.Structures.Errors;

public class ErrorGuardSpec
{
    [Theory]
    [InlineData(99)]
    [InlineData(600)]
    public void Create_InvalidStatus_ThrowsArgumentOutOfRangeException(int status)
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
            Error.Create("order.invalid", "Invalid order.", status));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_InvalidCode_ThrowsArgumentNullException(string? code)
    {
        Should.Throw<ArgumentNullException>(() =>
            Error.Create(code!, "Invalid order.", 400));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_InvalidMessage_ThrowsArgumentNullException(string? message)
    {
        Should.Throw<ArgumentNullException>(() =>
            Error.Create("order.invalid", message!, 400));
    }

    [Fact]
    public void ValidateStatus_ValidStatus_DoesNotThrow()
    {
        Should.NotThrow(() => ErrorGuard.ValidateStatus(200));
    }
}
