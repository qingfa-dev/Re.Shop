namespace SharedKernel.Concerns.Lifecycle;

public static class LifecycleConstant
{
    public static class Constraints
    {
        public const int MaxActorLength = 256;
    }

    public static class Patterns
    {
        public const string Actor = @"^\S(?:.*\S)?$";
    }
}
