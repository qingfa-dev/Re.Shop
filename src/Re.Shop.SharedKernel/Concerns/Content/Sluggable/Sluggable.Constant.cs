namespace SharedKernel.Concerns.Content.Sluggable;

public static class SluggableConstant
{
    public static class Constraints
    {
        public const int MaxSlugLength = 200;
    }

    public static class Patterns
    {
        public const string Slug = @"^[a-z0-9]+(?:-[a-z0-9]+)*$";
    }
}
