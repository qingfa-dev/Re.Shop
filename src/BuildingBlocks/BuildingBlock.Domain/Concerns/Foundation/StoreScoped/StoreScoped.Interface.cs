namespace BuildingBlocks.Domain.Concerns.Foundation.StoreScoped;

public interface IStoreScoped<TStoreKey>
{
    TStoreKey StoreId { get; set; }
}
