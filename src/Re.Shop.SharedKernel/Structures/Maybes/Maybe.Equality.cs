namespace SharedKernel.Structures.Maybes;

public readonly partial struct Maybe<T>
{
    public bool Equals(Maybe<T> other) =>
        _hasValue == other._hasValue &&
        (!_hasValue || EqualityComparer<T>.Default.Equals(_value!, other._value!));

    public override bool Equals(object? obj) => obj is Maybe<T> maybe && Equals(maybe);

    public override int GetHashCode() => _hasValue ? HashCode.Combine(0, _value) : 0;

    public static bool operator ==(Maybe<T> left, Maybe<T> right) => left.Equals(right);

    public static bool operator !=(Maybe<T> left, Maybe<T> right) => !left.Equals(right);
}
