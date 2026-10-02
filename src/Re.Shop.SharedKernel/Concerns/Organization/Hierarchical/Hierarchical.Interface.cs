namespace SharedKernel.Concerns.Organization.Hierarchical;

public interface IHierarchical<TKey>
    where TKey : struct
{
    TKey? ParentId { get; set; }
}
