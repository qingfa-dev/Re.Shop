namespace SharedKernel.Concerns.Foundation.Identifiable;

public interface IIdentifiable<TKey>
{
    TKey Id { get; }
}
