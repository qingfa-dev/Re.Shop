using SharedKernel.Errors;
using SharedKernel.Meta;
using Shouldly;

namespace SharedKernel.UnitTests.Errors;

public class ErrorFactorySpec
{
    public static TheoryData<Func<Error>, int, string, string, string> Factories =>
        new()
        {
            { () => Error.Unexpected(), 500, ErrorConstant.ErrorTypes.ServerError.Type, ErrorConstant.Default.Code, ErrorConstant.Default.Message },
            { () => Error.ServerError(), 500, ErrorConstant.ErrorTypes.ServerError.Type, ErrorConstant.Default.Code, ErrorConstant.Default.Message },
            { () => Error.Failure(), 500, ErrorConstant.ErrorTypes.Failure.Type, ErrorConstant.ErrorTypes.Failure.Code, ErrorConstant.ErrorTypes.Failure.Message },
            { () => Error.BadRequest(), 400, ErrorConstant.ErrorTypes.BadRequest.Type, ErrorConstant.ErrorTypes.BadRequest.Code, ErrorConstant.ErrorTypes.BadRequest.Message },
            { () => Error.Unauthorized(), 401, ErrorConstant.ErrorTypes.Unauthorized.Type, ErrorConstant.ErrorTypes.Unauthorized.Code, ErrorConstant.ErrorTypes.Unauthorized.Message },
            { () => Error.Forbidden(), 403, ErrorConstant.ErrorTypes.Forbidden.Type, ErrorConstant.ErrorTypes.Forbidden.Code, ErrorConstant.ErrorTypes.Forbidden.Message },
            { () => Error.NotFound(), 404, ErrorConstant.ErrorTypes.NotFound.Type, ErrorConstant.ErrorTypes.NotFound.Code, ErrorConstant.ErrorTypes.NotFound.Message },
            { () => Error.AlreadyExists(), 409, ErrorConstant.ErrorTypes.AlreadyExists.Type, ErrorConstant.ErrorTypes.AlreadyExists.Code, ErrorConstant.ErrorTypes.AlreadyExists.Message },
            { () => Error.Validation(), 422, ErrorConstant.ErrorTypes.Validation.Type, ErrorConstant.ErrorTypes.Validation.Code, ErrorConstant.ErrorTypes.Validation.Message },
            { () => Error.BusinessRuleViolation(), 422, ErrorConstant.ErrorTypes.BusinessRuleViolation.Type, ErrorConstant.ErrorTypes.BusinessRuleViolation.Code, ErrorConstant.ErrorTypes.BusinessRuleViolation.Message },
            { () => Error.InvalidParameters(), 400, ErrorConstant.ErrorTypes.InvalidParameters.Type, ErrorConstant.ErrorTypes.InvalidParameters.Code, ErrorConstant.ErrorTypes.InvalidParameters.Message },
            { () => Error.ServiceUnavailable(), 503, ErrorConstant.ErrorTypes.ServiceUnavailable.Type, ErrorConstant.ErrorTypes.ServiceUnavailable.Code, ErrorConstant.ErrorTypes.ServiceUnavailable.Message },
            { () => Error.LicenseExpired(), 503, ErrorConstant.ErrorTypes.LicenseExpired.Type, ErrorConstant.ErrorTypes.LicenseExpired.Code, ErrorConstant.ErrorTypes.LicenseExpired.Message },
            { () => Error.LicenseCancelled(), 503, ErrorConstant.ErrorTypes.LicenseCancelled.Type, ErrorConstant.ErrorTypes.LicenseCancelled.Code, ErrorConstant.ErrorTypes.LicenseCancelled.Message }
        };

    [Theory]
    [MemberData(nameof(Factories))]
    public void Factory_UsesExpectedDefaultsAndTypeMetadata(
        Func<Error> factory,
        int expectedStatus,
        string expectedType,
        string expectedCode,
        string expectedMessage)
    {
        var error = factory();

        error.Status.ShouldBe(expectedStatus);
        error.Code.ShouldBe(expectedCode);
        error.Message.ShouldBe(expectedMessage);
        error.GetMetadata(MetadataConstant.MetadataKey.Type).ShouldBe(expectedType);
    }

    [Fact]
    public void Create_ValidValues_CreatesErrorWithoutMetadata()
    {
        var error = Error.Create("order.invalid", "Invalid order.", 400);

        error.Code.ShouldBe("order.invalid");
        error.Message.ShouldBe("Invalid order.");
        error.Status.ShouldBe(400);
        error.Metadata.ShouldBeNull();
    }

    [Fact]
    public void Factories_DoNotAcceptMetadataDictionary()
    {
        var factories = typeof(Error)
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(method => method.ReturnType == typeof(Error));

        factories.ShouldAllBe(method =>
            !method.GetParameters().Any(parameter =>
                parameter.ParameterType == typeof(Dictionary<string, object?>)));
    }

    [Fact]
    public void Factory_CustomMetadata_IsAddedByFluentChain()
    {
        var error = Error.NotFound()
            .WithTarget("/orders/1")
            .WithMetadata("requestId", "req-1");

        error.GetMetadata(MetadataConstant.MetadataKey.Type)
            .ShouldBe(ErrorConstant.ErrorTypes.NotFound.Type);
        error.GetMetadata(MetadataConstant.MetadataKey.Target).ShouldBe("/orders/1");
        error.GetMetadata("requestId").ShouldBe("req-1");
    }

    [Theory]
    [InlineData("email", "The body property 'email' is required.")]
    public void MissingBodyProperty_FormatsPropertyMessage(string property, string expectedMessage)
    {
        var error = Error.MissingBodyProperty(property);

        error.Message.ShouldBe(expectedMessage);
        error.GetMetadata(MetadataConstant.MetadataKey.Type)
            .ShouldBe(ErrorConstant.ErrorTypes.MissingBodyProperty.Type);
    }

    [Fact]
    public void MissingBodyProperty_WhitespaceProperty_ThrowsArgumentException()
    {
        Should.Throw<ArgumentException>(() => Error.MissingBodyProperty(" "));
    }
}
