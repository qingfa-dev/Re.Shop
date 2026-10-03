using System.Linq.Expressions;

namespace BuildingBlock.Persistence.Abstractions.Specifications;

public sealed class OrderExpression<TEntity>
{
    private OrderExpression(LambdaExpression keySelector, bool descending)
    {
        KeySelector = keySelector;
        Descending = descending;
    }

    public LambdaExpression KeySelector { get; }

    public bool Descending { get; }

    public static OrderExpression<TEntity> AscendingBy<TKey>(
        Expression<Func<TEntity, TKey>> keySelector)
        => new(keySelector ?? throw new ArgumentNullException(nameof(keySelector)), descending: false);

    public static OrderExpression<TEntity> DescendingBy<TKey>(
        Expression<Func<TEntity, TKey>> keySelector)
        => new(keySelector ?? throw new ArgumentNullException(nameof(keySelector)), descending: true);
}
