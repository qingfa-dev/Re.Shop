using System.Text.Json;

namespace SharedKernel.UnitTests.Structures.Results;

public class ResultSpec
{
    #region Success

    [Fact]
    public void Success_WithValue_ExposesSuccessfulStateAndValue()
    {
        var result = Result<string, Error>.Success("saved");

        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Status.ShouldBe(200);
        result.HasValue.ShouldBeTrue();
        result.Value.ShouldBe("saved");
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Success_WithUnit_RepresentsNoPayloadSuccess()
    {
        var result = Result<Unit, Error>.Success(Unit.Value);

        result.IsSuccess.ShouldBeTrue();
        result.HasValue.ShouldBeTrue();
        result.Value.ShouldBe(Unit.Value);
    }

    #endregion

    #region Failure

    [Fact]
    public void Failure_WithErrorHasNoValueAndPreservesError()
    {
        var error = new Error("Test.NotFound", "Not found", 404);

        var result = Result<string, Error>.Failure(error);

        result.IsFailure.ShouldBeTrue();
        result.IsSuccess.ShouldBeFalse();
        result.Status.ShouldBe(404);
        result.HasValue.ShouldBeFalse();
        result.Value.ShouldBeNull();
        result.Errors.ShouldHaveSingleItem().ShouldBe(error);
    }

    #endregion

    #region Serialization

    [Fact]
    public void JsonSerialization_UsesSnakeCaseForResultAndErrorProperties()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
        var result = Result<string, Error>.Failure(
            new Error("Test.NotFound", "Not found", 404));

        var serialized = JsonSerializer.Serialize(result, options);
        using var document = JsonDocument.Parse(serialized);
        var root = document.RootElement;

        root.TryGetProperty("is_success", out _).ShouldBeTrue();
        root.TryGetProperty("is_failure", out _).ShouldBeTrue();
        root.TryGetProperty("has_value", out _).ShouldBeTrue();
        root.TryGetProperty("errors", out var errors).ShouldBeTrue();
        errors[0].TryGetProperty("code", out _).ShouldBeTrue();
        errors[0].TryGetProperty("message", out _).ShouldBeTrue();
    }

    [Fact]
    public void JsonSerialization_WithSnakeCaseOptionsRoundTripsResult()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
        var serialized = JsonSerializer.Serialize(
            Result<string, Error>.Success("saved").WithMetadata("trace", "abc"),
            options);

        var result = JsonSerializer.Deserialize<Result<string, Error>>(serialized, options);

        result.ShouldNotBeNull();
        result.Value.ShouldBe("saved");
        result.IsSuccess.ShouldBeTrue();
        result.Metadata!["trace"].ShouldBeOfType<JsonElement>().GetString().ShouldBe("abc");
    }

    #endregion

    #region PublicApi

    [Fact]
    public void PublicApi_HasNoOneArityOrValueResultType()
    {
        var types = typeof(Result<,>).Assembly.GetTypes();

        types.Any(type => type.IsPublic && type.Name == "Result`1").ShouldBeFalse();
        types.Any(type => type.IsPublic && type.Name.StartsWith("ValueResult", StringComparison.Ordinal))
            .ShouldBeFalse();
    }

    #endregion
}
