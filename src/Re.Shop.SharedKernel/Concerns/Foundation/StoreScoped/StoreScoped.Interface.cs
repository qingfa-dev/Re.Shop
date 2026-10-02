namespace SharedKernel.Concerns.Foundation.StoreScoped;

public interface IStoreScoped<TStoreKey>
{
    TStoreKey StoreId { get; set; }
}
