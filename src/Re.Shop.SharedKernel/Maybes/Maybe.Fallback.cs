namespace SharedKernel.Maybes;

public readonly partial struct Maybe<T>
{
    public T GetOrElse(T fallback) => _hasValue ? _value! : fallback;

    public T GetOrElse(Func<T> fallback) => _hasValue ? _value! : fallback();
}
