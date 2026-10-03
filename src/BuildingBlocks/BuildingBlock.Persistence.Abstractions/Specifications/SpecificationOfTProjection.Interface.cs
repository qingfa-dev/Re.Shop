using System.Linq.Expressions;

namespace BuildingBlock.Persistence.Abstractions.Specifications;

public interface ISpecification<TEntity, TProjection> : ISpecification<TEntity>
{
    Expression<Func<TEntity, TProjection>> Selector { get; }
}
