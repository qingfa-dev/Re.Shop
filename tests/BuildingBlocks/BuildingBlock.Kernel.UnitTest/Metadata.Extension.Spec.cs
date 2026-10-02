using BuildingBlock.Kernel.Metadata;

namespace BuildingBlock.Kernel.UnitTest;

[Trait("Category", "Unit")]
public class MetadataExtensionSpec
{
    public enum StubMetadataKind
    {
        None = 0,
        Alpha = 1,
        Beta = 2,
    }

    private static MetadataStub StubWith(string key, object value)
        => new() { Metadata = MetadataDictionary.Create().With(key, value) };

    private static TValue? Get<TValue>(MetadataStub stub, string key)
        => stub.GetValueOrDefault<MetadataStub, TValue>(key);

    private static TValue GetRequired<TValue>(MetadataStub stub, string key)
        => stub.GetRequiredValue<MetadataStub, TValue>(key);

    // ------------------------------------------------------------------
    // Raw access
    // ------------------------------------------------------------------

    [Fact]
    public void GetValueOrDefault_When_Key_Present_Should_Return_Raw_Value()
    {
        // Arrange
        var stub = StubWith("k", 42);

        // Act
        var actual = stub.GetValueOrDefault("k");

        // Assert
        actual.ShouldBe(42);
    }

    [Fact]
    public void GetValueOrDefault_When_Key_Missing_Should_Return_Null()
    {
        // Arrange
        var stub = new MetadataStub();

        // Act
        var actual = stub.GetValueOrDefault("k");

        // Assert
        actual.ShouldBeNull();
    }

    [Fact]
    public void GetValueOrDefault_When_Request_Null_Should_Throw_Request_Pattern()
    {
        // Arrange
        MetadataStub stub = null!;

        // Act
        var act = () => stub.GetValueOrDefault("k");

        // Assert
        var ex = Should.Throw<ArgumentNullException>(act);
        ex.ParamName.ShouldBe("request");
        ex.Message.ShouldContain(MetadataConstant.Result.Failure.Request.Argument.Null.Pattern);
    }

    [Fact]
    public void GetValueOrDefault_When_Key_Null_Should_Throw_Key_Pattern()
    {
        // Arrange
        var stub = new MetadataStub();

        // Act
        var act = () => stub.GetValueOrDefault(null!);

        // Assert
        var ex = Should.Throw<ArgumentNullException>(act);
        ex.ParamName.ShouldBe("key");
        ex.Message.ShouldContain(MetadataConstant.Result.Failure.Key.Argument.Null.Pattern);
    }

    // ------------------------------------------------------------------
    // Typed access
    // ------------------------------------------------------------------

    [Theory]
    [InlineData("42", 42)]
    [InlineData("0", 0)]
    public void GetValueOrDefault_Convert_String_Number_Should_Return_Int(string stored, int expected)
    {
        // Arrange
        var stub = StubWith("k", stored);

        // Act
        var actual = Get<int>(stub, "k");

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void GetValueOrDefault_Convert_Long_To_Int_Should_Return_Converted()
    {
        // Arrange
        var stub = StubWith("k", 42L);

        // Act
        var actual = Get<int>(stub, "k");

        // Assert
        actual.ShouldBe(42);
    }

    [Fact]
    public void GetValueOrDefault_Incompatible_Value_Should_Return_Default()
    {
        // Arrange
        var stub = StubWith("k", "abc");

        // Act
        var actual = Get<int>(stub, "k");

        // Assert
        actual.ShouldBe(0);
    }

    [Fact]
    public void GetValueOrDefault_Convert_Int_To_String_Should_Render_Invariant()
    {
        // Arrange
        var stub = StubWith("k", 42);

        // Act
        var actual = Get<string>(stub, "k");

        // Assert
        actual.ShouldBe("42");
    }

    [Fact]
    public void GetValueOrDefault_Missing_Key_Should_Return_Null_For_Reference_Target()
    {
        // Arrange
        var stub = new MetadataStub();

        // Act
        var actual = Get<string>(stub, "k");

        // Assert
        actual.ShouldBeNull();
    }

    [Fact]
    public void GetValueOrDefault_Missing_Key_Should_Return_Null_For_Nullable_Target()
    {
        // Arrange
        var stub = new MetadataStub();

        // Act
        var actual = Get<int?>(stub, "k");

        // Assert
        actual.ShouldBeNull();
    }

    [Fact]
    public void GetValueOrDefault_Convert_String_To_Nullable_Int_Should_Return_Converted()
    {
        // Arrange
        var stub = StubWith("k", "5");

        // Act
        var actual = Get<int?>(stub, "k");

        // Assert
        actual.ShouldBe(5);
    }

    [Fact]
    public void GetValueOrDefault_With_Fallback_When_Key_Missing_Should_Return_Fallback()
    {
        // Arrange
        var stub = new MetadataStub();

        // Act
        var actual = stub.GetValueOrDefault("version", 7);

        // Assert
        actual.ShouldBe(7);
    }

    [Fact]
    public void GetValueOrDefault_With_Fallback_When_Conversion_Fails_Should_Return_Fallback()
    {
        // Arrange
        var stub = StubWith("version", "abc");

        // Act
        var actual = stub.GetValueOrDefault("version", 7);

        // Assert
        actual.ShouldBe(7);
    }

    [Fact]
    public void GetValueOrDefault_With_Fallback_When_Convertible_Should_Return_Converted()
    {
        // Arrange
        var stub = StubWith("version", "42");

        // Act
        var actual = stub.GetValueOrDefault("version", 7);

        // Assert
        actual.ShouldBe(42);
    }

    [Fact]
    public void GetValueOrDefault_Guid_From_String_Should_Parse()
    {
        // Arrange
        var stub = StubWith("k", "11111111-2222-3333-4444-555555555555");

        // Act
        var actual = Get<Guid>(stub, "k");

        // Assert
        actual.ShouldBe(Guid.Parse("11111111-2222-3333-4444-555555555555"));
    }

    [Fact]
    public void GetValueOrDefault_Guid_From_Guid_Should_Return_Same()
    {
        // Arrange
        var expected = Guid.Parse("11111111-2222-3333-4444-555555555555");
        var stub = StubWith("k", expected);

        // Act
        var actual = Get<Guid>(stub, "k");

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void GetValueOrDefault_Guid_From_Invalid_String_Should_Return_Default()
    {
        // Arrange
        var stub = StubWith("k", "not-a-guid");

        // Act
        var actual = Get<Guid>(stub, "k");

        // Assert
        actual.ShouldBe(Guid.Empty);
    }

    [Fact]
    public void GetValueOrDefault_Guid_From_Numeric_Value_Should_Return_Default()
    {
        // Arrange
        var stub = StubWith("k", 42);

        // Act
        var actual = Get<Guid>(stub, "k");

        // Assert
        actual.ShouldBe(Guid.Empty);
    }

    [Fact]
    public void GetValueOrDefault_DateTime_From_String_Should_Parse()
    {
        // Arrange
        var stub = StubWith("k", "2024-06-15T10:30:00Z");

        // Act
        var actual = Get<DateTime>(stub, "k");

        // Assert
        actual.ShouldBe(new DateTime(2024, 6, 15, 10, 30, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void GetValueOrDefault_DateTime_From_DateTimeOffset_Should_Return_Utc()
    {
        // Arrange
        var source = new DateTimeOffset(2024, 6, 15, 12, 30, 0, TimeSpan.FromHours(2));
        var stub = StubWith("k", source);

        // Act
        var actual = Get<DateTime>(stub, "k");

        // Assert
        actual.ShouldBe(source.UtcDateTime);
    }

    [Fact]
    public void GetValueOrDefault_DateTime_When_Unparseable_Should_Return_Default()
    {
        // Arrange
        var stub = StubWith("k", "not-a-date");

        // Act
        var actual = Get<DateTime>(stub, "k");

        // Assert
        actual.ShouldBe(DateTime.MinValue);
    }

    [Fact]
    public void GetValueOrDefault_DateTimeOffset_From_String_Should_Parse()
    {
        // Arrange
        var stub = StubWith("k", "2024-06-15T10:30:00+02:00");

        // Act
        var actual = Get<DateTimeOffset>(stub, "k");

        // Assert
        actual.ShouldBe(new DateTimeOffset(2024, 6, 15, 10, 30, 0, TimeSpan.FromHours(2)));
    }

    [Fact]
    public void GetValueOrDefault_DateTimeOffset_From_DateTime_Should_Apply_Zero_Offset()
    {
        // Arrange
        var source = new DateTime(2024, 6, 15, 10, 30, 0, DateTimeKind.Utc);
        var stub = StubWith("k", source);

        // Act
        var actual = Get<DateTimeOffset>(stub, "k");

        // Assert
        actual.ShouldBe(new DateTimeOffset(source, TimeSpan.Zero));
    }

    [Fact]
    public void GetValueOrDefault_DateTimeOffset_When_Unparseable_Should_Return_Default()
    {
        // Arrange
        var stub = StubWith("k", "not-a-date");

        // Act
        var actual = Get<DateTimeOffset>(stub, "k");

        // Assert
        actual.ShouldBe(DateTimeOffset.MinValue);
    }

    [Theory]
    [InlineData("alpha", StubMetadataKind.Alpha)]
    [InlineData("BETA", StubMetadataKind.Beta)]
    [InlineData("2", StubMetadataKind.Beta)]
    public void GetValueOrDefault_Enum_Should_Parse_Name_Or_Numeric(string stored, StubMetadataKind expected)
    {
        // Arrange
        var stub = StubWith("k", stored);

        // Act
        var actual = Get<StubMetadataKind>(stub, "k");

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void GetValueOrDefault_Enum_From_Numeric_Value_Should_Return_Converted()
    {
        // Arrange
        var stub = StubWith("k", 2);

        // Act
        var actual = Get<StubMetadataKind>(stub, "k");

        // Assert
        actual.ShouldBe(StubMetadataKind.Beta);
    }

    [Fact]
    public void GetValueOrDefault_Enum_When_Invalid_Name_Should_Return_Default()
    {
        // Arrange
        var stub = StubWith("k", "Nope");

        // Act
        var actual = Get<StubMetadataKind>(stub, "k");

        // Assert
        actual.ShouldBe(StubMetadataKind.None);
    }

    [Fact]
    public void GetValueOrDefault_Enum_From_Incompatible_Value_Should_Return_Default()
    {
        // Arrange
        var stub = StubWith("k", DateTime.UnixEpoch);

        // Act
        var actual = Get<StubMetadataKind>(stub, "k");

        // Assert
        actual.ShouldBe(StubMetadataKind.None);
    }

    [Fact]
    public void GetValueOrDefault_TimeSpan_From_String_Should_Use_Type_Converter()
    {
        // Arrange
        var stub = StubWith("k", "01:30:00");

        // Act
        var actual = Get<TimeSpan>(stub, "k");

        // Assert
        actual.ShouldBe(TimeSpan.FromMinutes(90));
    }

    [Fact]
    public void GetValueOrDefault_Opaque_Target_When_Not_Convertible_Should_Return_Default()
    {
        // Arrange
        var stub = StubWith("k", "x");

        // Act
        var actual = Get<StubOpaque>(stub, "k");

        // Assert
        actual.ShouldBeNull();
    }

    [Fact]
    public void GetValueOrDefault_Opaque_Source_To_Int_Should_Return_Default()
    {
        // Arrange
        var stub = StubWith("k", new StubOpaque());

        // Act
        var actual = Get<int>(stub, "k");

        // Assert
        actual.ShouldBe(0);
    }

    // ------------------------------------------------------------------
    // Required access
    // ------------------------------------------------------------------

    [Fact]
    public void GetRequiredValue_When_Present_Should_Return_Value()
    {
        // Arrange
        var stub = StubWith("k", 42);

        // Act
        var actual = stub.GetRequiredValue("k");

        // Assert
        actual.ShouldBe(42);
    }

    [Fact]
    public void GetRequiredValue_When_Missing_Should_Throw_Key_Not_Found()
    {
        // Arrange
        var stub = new MetadataStub();

        // Act
        var act = () => stub.GetRequiredValue("k");

        // Assert
        var ex = Should.Throw<KeyNotFoundException>(act);
        ex.Message.ShouldContain(MetadataConstant.Result.Failure.Key.Metadata.NotFound.Pattern);
    }

    [Fact]
    public void GetRequiredValue_When_Stored_Value_Null_Should_Throw_Key_Not_Found()
    {
        // Arrange
        var source = new Dictionary<string, object> { ["k"] = null! };
        var stub = new MetadataStub { Metadata = new MetadataDictionary(source) };

        // Act
        var act = () => stub.GetRequiredValue("k");

        // Assert
        Should.Throw<KeyNotFoundException>(act);
    }

    [Fact]
    public void GetRequiredValue_Convert_String_Number_Should_Return_Int()
    {
        // Arrange
        var stub = StubWith("k", "42");

        // Act
        var actual = GetRequired<int>(stub, "k");

        // Assert
        actual.ShouldBe(42);
    }

    [Fact]
    public void GetRequiredValue_When_Missing_For_Typed_Should_Throw_Key_Not_Found()
    {
        // Arrange
        var stub = new MetadataStub();

        // Act
        Action act = () => _ = GetRequired<int>(stub, "k");

        // Assert
        var ex = Should.Throw<KeyNotFoundException>(act);
        ex.Message.ShouldContain(MetadataConstant.Result.Failure.Key.Metadata.NotFound.Pattern);
    }

    [Fact]
    public void GetRequiredValue_When_Not_Convertible_Should_Throw_Invalid_Cast()
    {
        // Arrange
        var stub = StubWith("k", "abc");

        // Act
        Action act = () => _ = GetRequired<int>(stub, "k");

        // Assert
        var ex = Should.Throw<InvalidCastException>(act);
        ex.Message.ShouldContain(MetadataConstant.Result.Failure.Value.Metadata.InvalidCast.Pattern);
    }

    [Fact]
    public void GetRequiredValue_When_Conversion_Yields_Null_Should_Throw_Invalid_Cast()
    {
        // Arrange
        var stub = StubWith("k", new StubOpaque());

        // Act
        var act = () => GetRequired<string>(stub, "k");

        // Assert
        Should.Throw<InvalidCastException>(act);
    }

    // ------------------------------------------------------------------
    // TryGet
    // ------------------------------------------------------------------

    [Fact]
    public void TryGetValue_When_Present_And_Convertible_Should_Return_True()
    {
        // Arrange
        var stub = StubWith("k", 42);

        // Act
        var found = stub.TryGetValue("k", out int value);

        // Assert
        found.ShouldBeTrue();
        value.ShouldBe(42);
    }

    [Fact]
    public void TryGetValue_When_Present_But_Incompatible_Should_Return_False()
    {
        // Arrange
        var stub = StubWith("k", "abc");

        // Act
        var found = stub.TryGetValue("k", out int value);

        // Assert
        found.ShouldBeFalse();
        value.ShouldBe(0);
    }

    [Fact]
    public void TryGetValue_When_Missing_For_Value_Target_Should_Return_False()
    {
        // Arrange
        var stub = new MetadataStub();

        // Act
        var found = stub.TryGetValue("k", out int value);

        // Assert
        found.ShouldBeFalse();
        value.ShouldBe(0);
    }

    [Fact]
    public void TryGetValue_When_Missing_For_Reference_Target_Should_Return_False()
    {
        // Arrange
        var stub = new MetadataStub();

        // Act
        var found = stub.TryGetValue("k", out string? value);

        // Assert
        found.ShouldBeFalse();
        value.ShouldBeNull();
    }

    // ------------------------------------------------------------------
    // Presence / mutation
    // ------------------------------------------------------------------

    [Fact]
    public void Contains_When_Key_Present_Different_Case_Should_Return_True()
    {
        // Arrange
        var stub = StubWith("CorrelationId", "abc");

        // Act
        // Assert
        stub.Contains("correlationid").ShouldBeTrue();
    }

    [Fact]
    public void Contains_When_Key_Missing_Should_Return_False()
    {
        // Arrange
        var stub = new MetadataStub();

        // Act
        // Assert
        stub.Contains("k").ShouldBeFalse();
    }

    [Fact]
    public void Contains_When_Request_Null_Should_Throw_Request_Pattern()
    {
        // Arrange
        MetadataStub stub = null!;

        // Act
        Action act = () => _ = stub.Contains("k");

        // Assert
        var ex = Should.Throw<ArgumentNullException>(act);
        ex.ParamName.ShouldBe("request");
        ex.Message.ShouldContain(MetadataConstant.Result.Failure.Request.Argument.Null.Pattern);
    }

    [Fact]
    public void Contains_When_Key_Null_Should_Throw_Key_Pattern()
    {
        // Arrange
        var stub = new MetadataStub();

        // Act
        Action act = () => _ = stub.Contains(null!);

        // Assert
        var ex = Should.Throw<ArgumentNullException>(act);
        ex.ParamName.ShouldBe("key");
        ex.Message.ShouldContain(MetadataConstant.Result.Failure.Key.Argument.Null.Pattern);
    }

    [Fact]
    public void SetValue_Should_Add_Or_Overwrite_Entry()
    {
        // Arrange
        var stub = new MetadataStub();

        // Act
        stub.SetValue("k", 1);
        stub.SetValue("K", 2);

        // Assert
        stub.Metadata.Count.ShouldBe(1);
        stub.Metadata["k"].ShouldBe(2);
    }

    [Fact]
    public void SetValue_When_Request_Null_Should_Throw_Request_Pattern()
    {
        // Arrange
        MetadataStub stub = null!;

        // Act
        var act = () => stub.SetValue("k", 1);

        // Assert
        var ex = Should.Throw<ArgumentNullException>(act);
        ex.ParamName.ShouldBe("request");
        ex.Message.ShouldContain(MetadataConstant.Result.Failure.Request.Argument.Null.Pattern);
    }

    [Fact]
    public void SetValue_When_Key_Null_Should_Throw_Key_Pattern()
    {
        // Arrange
        var stub = new MetadataStub();

        // Act
        var act = () => stub.SetValue(null!, 1);

        // Assert
        var ex = Should.Throw<ArgumentNullException>(act);
        ex.ParamName.ShouldBe("key");
        ex.Message.ShouldContain(MetadataConstant.Result.Failure.Key.Argument.Null.Pattern);
    }

    [Fact]
    public void SetValue_When_Value_Null_Should_Throw_Value_Param()
    {
        // Arrange
        var stub = new MetadataStub();

        // Act
        var act = () => stub.SetValue("k", null!);

        // Assert
        var ex = Should.Throw<ArgumentNullException>(act);
        ex.ParamName.ShouldBe("value");
    }

    [Fact]
    public void SetValue_When_Metadata_Not_Mutable_Should_Throw_Not_Supported()
    {
        // Arrange
        var stub = new NullMetadataStub();

        // Act
        var act = () => stub.SetValue("k", 1);

        // Assert
        var ex = Should.Throw<NotSupportedException>(act);
        ex.Message.ShouldContain(MetadataConstant.Result.Failure.Dictionary.Mutation.NotSupported.Pattern);
    }

    [Fact]
    public void Remove_When_Key_Present_Should_Remove_And_Return_True()
    {
        // Arrange
        var stub = StubWith("k", 1);

        // Act
        var removed = stub.Remove("k");

        // Assert
        removed.ShouldBeTrue();
        stub.Metadata.ContainsKey("k").ShouldBeFalse();
    }

    [Fact]
    public void Remove_When_Key_Missing_Should_Return_False()
    {
        // Arrange
        var stub = new MetadataStub();

        // Act
        // Assert
        stub.Remove("k").ShouldBeFalse();
    }

    [Fact]
    public void Remove_When_Metadata_Not_Mutable_Should_Return_False()
    {
        // Arrange
        var stub = new NullMetadataStub();

        // Act
        // Assert
        stub.Remove("k").ShouldBeFalse();
    }

    [Fact]
    public void Remove_When_Request_Null_Should_Throw_Request_Pattern()
    {
        // Arrange
        MetadataStub stub = null!;

        // Act
        Action act = () => _ = stub.Remove("k");

        // Assert
        var ex = Should.Throw<ArgumentNullException>(act);
        ex.ParamName.ShouldBe("request");
        ex.Message.ShouldContain(MetadataConstant.Result.Failure.Request.Argument.Null.Pattern);
    }

    [Fact]
    public void Remove_When_Key_Null_Should_Throw_Key_Pattern()
    {
        // Arrange
        var stub = new MetadataStub();

        // Act
        Action act = () => _ = stub.Remove(null!);

        // Assert
        var ex = Should.Throw<ArgumentNullException>(act);
        ex.ParamName.ShouldBe("key");
        ex.Message.ShouldContain(MetadataConstant.Result.Failure.Key.Argument.Null.Pattern);
    }

    // ------------------------------------------------------------------
    // Well-known key accessors
    // ------------------------------------------------------------------

    [Fact]
    public void Well_Known_Accessors_When_Present_Should_Return_Stored_Values()
    {
        // Arrange
        var correlationId = Guid.Parse("11111111-2222-3333-4444-555555555555");
        var requestId = Guid.Parse("22222222-3333-4444-5555-666666666666");
        var causationId = Guid.Parse("33333333-4444-5555-6666-777777777777");
        var eventId = Guid.Parse("44444444-5555-6666-7777-888888888888");
        var timestamp = new DateTimeOffset(2024, 6, 15, 10, 30, 0, TimeSpan.Zero);
        var stub = new MetadataStub
        {
            Metadata = MetadataDictionary.Create()
                .With(MetadataConstant.Keyword.CorrelationId, correlationId)
                .With(MetadataConstant.Keyword.RequestId, requestId)
                .With(MetadataConstant.Keyword.CausationId, causationId)
                .With(MetadataConstant.Keyword.EventId, eventId)
                .With(MetadataConstant.Keyword.AggregateVersion, 9)
                .With(MetadataConstant.Keyword.Timestamp, timestamp)
                .With(MetadataConstant.Keyword.MemberType, "Order"),
        };

        // Act
        // Assert
        stub.GetCorrelationId().ShouldBe(correlationId);
        stub.GetRequestId().ShouldBe(requestId);
        stub.GetCausationId().ShouldBe(causationId);
        stub.GetEventId().ShouldBe(eventId);
        stub.GetAggregateVersion().ShouldBe(9L);
        stub.GetTimestamp().ShouldBe(timestamp);
        stub.GetMemberType().ShouldBe("Order");
    }

    [Fact]
    public void Well_Known_Accessors_When_Missing_Should_Return_Null()
    {
        // Arrange
        var stub = new MetadataStub();

        // Act
        // Assert
        stub.GetCorrelationId().ShouldBeNull();
        stub.GetRequestId().ShouldBeNull();
        stub.GetCausationId().ShouldBeNull();
        stub.GetEventId().ShouldBeNull();
        stub.GetAggregateVersion().ShouldBeNull();
        stub.GetTimestamp().ShouldBeNull();
        stub.GetMemberType().ShouldBeNull();
    }

    [Fact]
    public void Well_Known_Accessors_When_Present_As_Native_Types_Should_Return_Stored_Values()
    {
        // Arrange: native long probes the nullable-wrapper path for AggregateVersion.
        var timestamp = new DateTimeOffset(2024, 6, 15, 10, 30, 0, TimeSpan.Zero);
        var correlationId = Guid.Parse("11111111-2222-3333-4444-555555555555");
        var stub = new MetadataStub
        {
            Metadata = MetadataDictionary.Create()
                .With(MetadataConstant.Keyword.AggregateVersion, 9L)
                .With(MetadataConstant.Keyword.Timestamp, timestamp)
                .With(MetadataConstant.Keyword.CorrelationId, correlationId),
        };

        // Act
        // Assert
        stub.GetAggregateVersion().ShouldBe(9L);
        stub.GetTimestamp().ShouldBe(timestamp);
        stub.GetCorrelationId().ShouldBe(correlationId);
    }

    [Fact]
    public void GetValueOrDefault_Version_From_Empty_String_Should_Return_Default()
    {
        // Arrange
        var stub = StubWith("k", string.Empty);

        // Act
        var actual = Get<Version>(stub, "k");

        // Assert
        actual.ShouldBeNull();
    }

    [Fact]
    public void GetValueOrDefault_Uri_From_Empty_String_Should_Return_Default()
    {
        // Arrange: UriConverter converts "" to null — exercises the null-result guard.
        var stub = StubWith("k", string.Empty);

        // Act
        var actual = Get<Uri>(stub, "k");

        // Assert
        actual.ShouldBeNull();
    }

    [Fact]
    public void GetValueOrDefault_Uri_From_String_Should_Parse()
    {
        // Arrange
        var stub = StubWith("k", "https://example.com/orders/42");

        // Act
        var actual = Get<Uri>(stub, "k");

        // Assert
        actual.ShouldBe(new Uri("https://example.com/orders/42"));
    }
}
