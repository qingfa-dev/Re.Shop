using System.Linq.Expressions;

namespace BuildingBlock.Persistence.Abstractions.Specifications;

public abstract class Specification<TEntity, TProjection> : Specification<TEntity>,
    ISpecification<TEntity, TProjection>
{
    protected Specification(
        Expression<Func<TEntity, bool>>? criteria,
        Expression<Func<TEntity, TProjection>> selector,
        IEnumerable<Expression<Func<TEntity, object?>>>? includes = null,
        IEnumerable<OrderExpression<TEntity>>? orderExpressions = null)
        : base(criteria, includes, orderExpressions)
    {
        Selector = selector ?? throw new ArgumentNullException(nameof(selector));
    }

    public Expression<Func<TEntity, TProjection>> Selector { get; }
}
