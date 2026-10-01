using SharedKernel.Structures.Maybes;

namespace SharedKernel.UnitTests.Structures.Maybes;

public class MaybeSpec
{
    [Fact]
    public void Some_NonNullValue_HasValue()
    {
        var maybe = Maybe<string>.Some("value");

        maybe.HasValue.ShouldBeTrue();
        maybe.IsEmpty.ShouldBeFalse();
        maybe.Value.ShouldBe("value");
    }

    [Fact]
    public void Some_NullValue_ThrowsArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => Maybe<string>.Some(null!));
    }

    [Fact]
    public void None_IsEmpty()
    {
        var maybe = Maybe<string>.None;

        maybe.HasValue.ShouldBeFalse();
        maybe.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public void Value_WhenEmpty_ThrowsInvalidOperationException()
    {
        Should.Throw<InvalidOperationException>(() => _ = Maybe<string>.None.Value)
            .Message.ShouldBe("Maybe is empty.");
    }

    [Fact]
    public void GetOrElse_WhenHasValue_ReturnsValue()
    {
        Maybe<string>.Some("value").GetOrElse("fallback").ShouldBe("value");
    }

    [Fact]
    public void GetOrElse_WhenEmpty_ReturnsFallback()
    {
        Maybe<string>.None.GetOrElse("fallback").ShouldBe("fallback");
    }

    [Fact]
    public void GetOrElseFactory_WhenHasValue_DoesNotInvokeFactory()
    {
        var invoked = false;

        var result = Maybe<string>.Some("value").GetOrElse(() =>
        {
            invoked = true;
            return "fallback";
        });

        result.ShouldBe("value");
        invoked.ShouldBeFalse();
    }

    [Fact]
    public void GetOrElseFactory_WhenEmpty_InvokesFactory()
    {
        Maybe<string>.None.GetOrElse(() => "fallback").ShouldBe("fallback");
    }

    [Fact]
    public void Equality_UsesValueAndEmptyState()
    {
        (Maybe<string>.Some("value") == Maybe<string>.Some("value")).ShouldBeTrue();
        (Maybe<string>.Some("value") != Maybe<string>.Some("other")).ShouldBeTrue();
        (Maybe<string>.None == Maybe<string>.None).ShouldBeTrue();
        Maybe<string>.Some("value").Equals((object)Maybe<string>.Some("value")).ShouldBeTrue();
        Maybe<string>.Some("value").Equals((object)"value").ShouldBeFalse();
    }

    [Fact]
    public void HashCode_EqualMaybesHaveEqualHashCodes()
    {
        Maybe<string>.Some("value").GetHashCode()
            .ShouldBe(Maybe<string>.Some("value").GetHashCode());
        Maybe<string>.None.GetHashCode().ShouldBe(0);
    }

    [Fact]
    public void ToString_ShowsSomeValueOrNone()
    {
        Maybe<string>.Some("value").ToString().ShouldBe("Some(value)");
        Maybe<string>.None.ToString().ShouldBe("None");
    }
}
