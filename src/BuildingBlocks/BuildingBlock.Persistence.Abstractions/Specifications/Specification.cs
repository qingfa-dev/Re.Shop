using System.Linq.Expressions;

namespace BuildingBlock.Persistence.Abstractions.Specifications;

public abstract class Specification<TEntity> : ISpecification<TEntity>
{
    protected Specification(
        Expression<Func<TEntity, bool>>? criteria = null,
        IEnumerable<Expression<Func<TEntity, object?>>>? includes = null,
        IEnumerable<OrderExpression<TEntity>>? orderExpressions = null)
    {
        Criteria = criteria;
        Includes = Array.AsReadOnly(includes?.ToArray() ?? []);
        OrderExpressions = Array.AsReadOnly(orderExpressions?.ToArray() ?? []);
    }

    public Expression<Func<TEntity, bool>>? Criteria { get; }

    public IReadOnlyList<Expression<Func<TEntity, object?>>> Includes { get; }

    public IReadOnlyList<OrderExpression<TEntity>> OrderExpressions { get; }
}
