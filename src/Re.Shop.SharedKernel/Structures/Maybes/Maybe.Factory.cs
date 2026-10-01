namespace SharedKernel.Structures.Maybes;

public readonly partial struct Maybe<T>
{
    public static Maybe<T> Some(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new Maybe<T>(true, value);
    }

    public static Maybe<T> None => default;
}
