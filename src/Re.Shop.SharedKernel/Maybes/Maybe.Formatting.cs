namespace SharedKernel.Maybes;

public readonly partial struct Maybe<T>
{
    public override string ToString() => _hasValue ? $"Some({_value})" : "None";
}
