namespace SharedKernel.Maybes;

public readonly partial struct Maybe<T> : IEquatable<Maybe<T>>
{
    private readonly bool _hasValue;
    private readonly T? _value;

    private Maybe(bool hasValue, T? value)
    {
        _hasValue = hasValue;
        _value = value;
    }

    public bool HasValue => _hasValue;

    public bool IsEmpty => !_hasValue;

    public T Value => _hasValue
        ? _value!
        : throw new InvalidOperationException(MaybeConstant.EmptyValueMessage);
}
