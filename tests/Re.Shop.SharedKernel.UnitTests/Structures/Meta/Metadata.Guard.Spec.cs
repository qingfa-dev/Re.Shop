using SharedKernel.Structures.Meta;

namespace SharedKernel.UnitTests.Structures.Meta;

public class MetadataGuardSpec
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ValidateKey_InvalidKey_ShouldThrowWithKeyError(string? key)
    {
        var exception = Should.Throw<ArgumentNullException>(
            () => MetadataGuard.ValidateKey(key));

        exception.ParamName.ShouldBe("key");
        exception.Message.ShouldContain(
            MetadataConstant.Errors.Key.NullOrWhitespace.Code);
        exception.Message.ShouldContain(
            MetadataConstant.Errors.Key.NullOrWhitespace.Message);
    }

    [Fact]
    public void ValidateKey_ValidKey_ShouldNotThrow()
    {
        Should.NotThrow(() => MetadataGuard.ValidateKey("eventId"));
    }

    [Fact]
    public void ValidateValue_NullValue_ShouldThrowWithValueError()
    {
        var exception = Should.Throw<ArgumentNullException>(
            () => MetadataGuard.ValidateValue(null));

        exception.ParamName.ShouldBe("value");
        exception.Message.ShouldContain(MetadataConstant.Errors.Value.Null.Code);
        exception.Message.ShouldContain(MetadataConstant.Errors.Value.Null.Message);
    }

    [Fact]
    public void ValidateValue_NonNullValue_ShouldNotThrow()
    {
        Should.NotThrow(() => MetadataGuard.ValidateValue("event-id"));
    }
}
