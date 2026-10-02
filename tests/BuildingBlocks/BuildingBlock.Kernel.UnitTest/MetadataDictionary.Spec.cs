using BuildingBlock.Kernel.Metadata;

namespace BuildingBlock.Kernel.UnitTest;

[Trait("Category", "Unit")]
public class MetadataDictionarySpec
{
    [Fact]
    public void New_With_No_Arguments_Should_Create_Empty_Case_Insensitive_Store()
    {
        // Arrange
        var dictionary = new MetadataDictionary();

        // Act
        dictionary["Alpha"] = 1;

        // Assert
        dictionary.Count.ShouldBe(1);
        dictionary.ContainsKey("ALPHA").ShouldBeTrue();
        dictionary["alpha"].ShouldBe(1);
    }

    [Fact]
    public void New_From_Dictionary_Should_Copy_Entries_Case_Insensitively()
    {
        // Arrange
        var source = new Dictionary<string, object> { ["CorrelationId"] = "abc" };

        // Act
        var dictionary = new MetadataDictionary(source);

        // Assert
        dictionary.Count.ShouldBe(1);
        dictionary["correlationid"].ShouldBe("abc");
    }

    [Fact]
    public void New_From_Dictionary_When_Null_Should_Throw_With_Dictionary_Pattern()
    {
        // Arrange
        IDictionary<string, object> source = null!;

        // Act
        var act = () => new MetadataDictionary(source);

        // Assert
        var ex = Should.Throw<ArgumentNullException>(act);
        ex.ParamName.ShouldBe("dictionary");
        ex.Message.ShouldContain(MetadataConstant.Result.Failure.Dictionary.Argument.Null.Pattern);
    }

    [Fact]
    public void New_From_Pairs_When_Null_Should_Throw_With_Dictionary_Pattern()
    {
        // Arrange
        IEnumerable<KeyValuePair<string, object>> pairs = null!;

        // Act
        var act = () => new MetadataDictionary(pairs);

        // Assert
        var ex = Should.Throw<ArgumentNullException>(act);
        ex.ParamName.ShouldBe("pairs");
        ex.Message.ShouldContain(MetadataConstant.Result.Failure.Dictionary.Argument.Null.Pattern);
    }

    [Fact]
    public void New_From_Pairs_When_Key_Null_Should_Throw_With_Key_Pattern()
    {
        // Arrange
        var pairs = new[] { new KeyValuePair<string, object>(null!, "value") };

        // Act
        var act = () => new MetadataDictionary(pairs);

        // Assert
        var ex = Should.Throw<ArgumentException>(act);
        ex.ParamName.ShouldBe("pairs");
        ex.Message.ShouldContain(MetadataConstant.Result.Failure.Key.Argument.Null.Pattern);
    }

    [Fact]
    public void New_From_Pairs_When_Key_Duplicated_Should_Keep_Last_Value()
    {
        // Arrange
        var pairs = new[]
        {
            new KeyValuePair<string, object>("a", 1),
            new KeyValuePair<string, object>("a", 2),
        };

        // Act
        var dictionary = new MetadataDictionary(pairs);

        // Assert
        dictionary.Count.ShouldBe(1);
        dictionary["a"].ShouldBe(2);
    }

    [Fact]
    public void New_From_Pairs_When_Empty_Should_Create_Empty_Store()
    {
        // Arrange
        var pairs = Array.Empty<KeyValuePair<string, object>>();

        // Act
        var dictionary = MetadataDictionary.Create(pairs);

        // Assert
        dictionary.ShouldBeEmpty();
    }

    [Fact]
    public void Create_With_No_Arguments_Should_Return_Fresh_Instance()
    {
        // Arrange
        // Act
        var first = MetadataDictionary.Create();
        var second = MetadataDictionary.Create();

        // Assert
        first.ShouldNotBeSameAs(second);
        first.ShouldNotBeSameAs(MetadataDictionary.Empty);
    }

    [Fact]
    public void Create_From_Dictionary_Should_Produce_Copied_Store()
    {
        // Arrange
        var source = new Dictionary<string, object> { ["RequestId"] = 1 };

        // Act
        var dictionary = MetadataDictionary.Create(source);

        // Assert
        dictionary.Count.ShouldBe(1);
        dictionary["requestid"].ShouldBe(1);
    }

    [Fact]
    public void Create_From_Dictionary_When_Null_Should_Throw_With_Dictionary_Pattern()
    {
        // Arrange
        IDictionary<string, object> source = null!;

        // Act
        var act = () => MetadataDictionary.Create(source);

        // Assert
        var ex = Should.Throw<ArgumentNullException>(act);
        ex.ParamName.ShouldBe("dictionary");
        ex.Message.ShouldContain(MetadataConstant.Result.Failure.Dictionary.Argument.Null.Pattern);
    }

    [Fact]
    public void Create_From_Pairs_When_Null_Should_Throw_With_Dictionary_Pattern()
    {
        // Arrange
        IEnumerable<KeyValuePair<string, object>> pairs = null!;

        // Act
        var act = () => MetadataDictionary.Create(pairs);

        // Assert
        var ex = Should.Throw<ArgumentNullException>(act);
        ex.ParamName.ShouldBe("pairs");
        ex.Message.ShouldContain(MetadataConstant.Result.Failure.Dictionary.Argument.Null.Pattern);
    }

    [Fact]
    public void Empty_Should_Be_Shared_Empty_Instance()
    {
        // Arrange
        // Act
        // Assert
        MetadataDictionary.Empty.ShouldBeSameAs(MetadataDictionary.Empty);
        MetadataDictionary.Empty.ShouldBeEmpty();
    }

    [Fact]
    public void With_Should_Return_Same_Instance_For_Chaining()
    {
        // Arrange
        var dictionary = MetadataDictionary.Create();

        // Act
        var chained = dictionary.With("a", 1);

        // Assert
        chained.ShouldBeSameAs(dictionary);
        dictionary["a"].ShouldBe(1);
    }

    [Fact]
    public void With_Should_Overwrite_Case_Insensitively()
    {
        // Arrange
        var dictionary = MetadataDictionary.Create().With("alpha", 1);

        // Act
        dictionary.With("ALPHA", 2);

        // Assert
        dictionary.Count.ShouldBe(1);
        dictionary["Alpha"].ShouldBe(2);
    }

    [Fact]
    public void With_When_Key_Null_Should_Throw_With_Key_Pattern()
    {
        // Arrange
        var dictionary = MetadataDictionary.Create();

        // Act
        var act = () => dictionary.With(null!, 1);

        // Assert
        var ex = Should.Throw<ArgumentNullException>(act);
        ex.ParamName.ShouldBe("key");
        ex.Message.ShouldContain(MetadataConstant.Result.Failure.Key.Argument.Null.Pattern);
    }

    [Fact]
    public void With_When_Value_Null_Should_Throw_With_Value_Param()
    {
        // Arrange
        var dictionary = MetadataDictionary.Create();

        // Act
        var act = () => dictionary.With("a", null!);

        // Assert
        var ex = Should.Throw<ArgumentNullException>(act);
        ex.ParamName.ShouldBe("value");
    }

    [Fact]
    public void Metadata_Should_Implement_Metadata_Dictionary_Contract()
    {
        // Arrange
        // Act
        // Assert
        new MetadataDictionary().ShouldBeAssignableTo<IMetadataDictionary>();
        new MetadataDictionary().ShouldBeAssignableTo<IReadOnlyDictionary<string, object>>();
    }
}
