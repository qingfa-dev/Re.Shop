using System.Linq.Expressions;

namespace BuildingBlock.Persistence.Abstractions.Specifications;

public interface ISpecification<TEntity>
{
    Expression<Func<TEntity, bool>>? Criteria { get; }

    IReadOnlyList<Expression<Func<TEntity, object?>>> Includes { get; }

    IReadOnlyList<OrderExpression<TEntity>> OrderExpressions { get; }
}
