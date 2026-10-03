namespace BuildingBlocks.Domain.Concerns.Organization.Categorizable;

public interface ICategorizable<TCategory>
{
    ICollection<TCategory> Categories { get; }
}
