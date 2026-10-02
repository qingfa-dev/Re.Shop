using System.Reflection;
using System.Text.RegularExpressions;
using BuildingBlock.Kernel.Metadata;

namespace BuildingBlock.Kernel.UnitTest;

[Trait("Category", "Unit")]
public class MetadataConstantSpec
{
    [Theory]
    [InlineData(MetadataConstant.Keyword.Timestamp, "Timestamp")]
    [InlineData(MetadataConstant.Keyword.MemberType, "MemberType")]
    [InlineData(MetadataConstant.Keyword.CorrelationId, "CorrelationId")]
    [InlineData(MetadataConstant.Keyword.RequestId, "RequestId")]
    [InlineData(MetadataConstant.Keyword.CausationId, "CausationId")]
    [InlineData(MetadataConstant.Keyword.EventId, "EventId")]
    [InlineData(MetadataConstant.Keyword.AggregateVersion, "AggregateVersion")]
    public void Keyword_Should_Round_Trip_As_Stable_Wire_Key(string keyword, string stableKey)
    {
        // Arrange
        // Act
        // Assert
        keyword.ShouldBe(stableKey);
    }

    [Fact]
    public void Failure_Rules_Should_Be_Discoverable()
    {
        // Arrange
        // Act
        var rules = CollectRuleTypes(typeof(MetadataConstant.Result.Failure));

        // Assert
        rules.ShouldNotBeEmpty();
    }

    [Theory]
    [MemberData(nameof(FailureRules))]
    public void Failure_Rule_Pattern_Should_Compose_Code_And_Message(
        string typeName,
        string code,
        string message,
        string pattern)
    {
        // Arrange
        // Act
        // Assert
        pattern.ShouldBe($"{code}:{message}");
        message.ShouldNotBeNullOrWhiteSpace();
        code.ShouldMatch(@"^[a-z][a-z0-9_]*(\.[a-z][a-z0-9_]+)+$");
        typeName.ShouldNotBeNullOrWhiteSpace();
    }

    public static TheoryData<string, string, string, string> FailureRules()
    {
        var data = new TheoryData<string, string, string, string>();

        foreach (var rule in CollectRuleTypes(typeof(MetadataConstant.Result.Failure)))
        {
            data.Add(
                rule.FullName!,
                GetConstant(rule, "Code"),
                GetConstant(rule, "Message"),
                GetConstant(rule, "Pattern"));
        }

        return data;
    }

    private static IEnumerable<Type> CollectRuleTypes(Type type)
    {
        foreach (var nested in type.GetNestedTypes())
        {
            if (nested.GetField("Code") is not null)
            {
                yield return nested;
            }

            foreach (var deeper in CollectRuleTypes(nested))
            {
                yield return deeper;
            }
        }
    }

    private static string GetConstant(Type type, string name)
        => (string)type.GetField(name, BindingFlags.Public | BindingFlags.Static)!.GetValue(null)!;
}
