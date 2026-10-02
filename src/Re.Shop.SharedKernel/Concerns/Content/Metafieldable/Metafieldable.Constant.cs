namespace SharedKernel.Concerns.Content.Metafieldable;

public static class MetafieldableConstant
{
    public static class Constraints
    {
        public const int MaxNamespaceLength = 64;
        public const int MaxKeyLength = 64;
    }

    public static class Patterns
    {
        public const string SnakeCase = @"^[a-z0-9_]+$";
    }
}
