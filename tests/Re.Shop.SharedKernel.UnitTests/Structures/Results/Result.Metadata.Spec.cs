namespace SharedKernel.UnitTests.Structures.Results;

public class ResultMetadataSpec
{
    #region WithAdditionalMetadata

    [Fact]
    public void WithAdditionalMetadata_PreservesResultStateAndAddsMetadata()
    {
        var metadata = new Dictionary<string, object?> { ["trace"] = "abc" };
        var result = Result<string, Error>.Success("saved", 201);

        var updated = result.WithAdditionalMetadata(metadata);

        updated.IsSuccess.ShouldBeTrue();
        updated.Status.ShouldBe(201);
        updated.Value.ShouldBe("saved");
        updated.Metadata!.ShouldContainKey("trace");
        updated.Metadata!["trace"].ShouldBe("abc");
    }

    [Fact]
    public void WithAdditionalMetadata_PreservesFailureStateAndErrors()
    {
        var error = new Error("Test.NotFound", "Not found", 404);
        var metadata = new Dictionary<string, object?> { ["trace"] = "abc" };
        var result = Result<string, Error>.Failure(error);

        var updated = result.WithAdditionalMetadata(metadata);

        updated.IsFailure.ShouldBeTrue();
        updated.Status.ShouldBe(404);
        updated.HasValue.ShouldBeFalse();
        updated.Errors.ShouldHaveSingleItem().ShouldBe(error);
        updated.Metadata!["trace"].ShouldBe("abc");
    }

    [Fact]
    public void WithAdditionalMetadata_AcceptsUnitResult()
    {
        var metadata = new Dictionary<string, object?> { ["trace"] = "abc" };
        var result = Result<Unit, Error>.Success(Unit.Value);

        var updated = result.WithAdditionalMetadata(metadata);

        updated.IsSuccess.ShouldBeTrue();
        updated.Value.ShouldBe(Unit.Value);
        updated.Metadata!["trace"].ShouldBe("abc");
    }

    [Fact]
    public void WithAdditionalMetadata_WithNull_ReturnsSameResult()
    {
        var result = Result<string, Error>.Success("saved");

        result.WithAdditionalMetadata(null).ShouldBeSameAs(result);
    }

    [Fact]
    public void WithAdditionalMetadata_WithNullValue_UsesMetadataGuard()
    {
        var metadata = new Dictionary<string, object?> { ["trace"] = null };
        var result = Result<string, Error>.Success("saved");

        Should.Throw<ArgumentNullException>(() => result.WithAdditionalMetadata(metadata));
    }

    [Fact]
    public void WithAdditionalMetadata_WithBlankKey_UsesMetadataGuard()
    {
        var metadata = new Dictionary<string, object?> { [" "] = "value" };

        var result = Result<string, Error>.Success("saved");

        Should.Throw<ArgumentNullException>(() => result.WithAdditionalMetadata(metadata));
    }

    [Fact]
    public void ResultConstructorsAndFactories_DoNotAcceptMetadataDictionary()
    {
        var resultType = typeof(Result<string, Error>);
        var constructors = resultType.GetConstructors(
            System.Reflection.BindingFlags.Instance
            | System.Reflection.BindingFlags.Public
            | System.Reflection.BindingFlags.NonPublic);
        var factories = resultType.GetMethods(
            System.Reflection.BindingFlags.Static
            | System.Reflection.BindingFlags.Public
            | System.Reflection.BindingFlags.NonPublic);

        constructors.ShouldAllBe(constructor =>
            !constructor.GetParameters().Any(parameter =>
                parameter.ParameterType == typeof(Dictionary<string, object?>)));
        factories.ShouldAllBe(method =>
            !method.GetParameters().Any(parameter =>
                parameter.ParameterType == typeof(Dictionary<string, object?>)));
    }

    #endregion
}
