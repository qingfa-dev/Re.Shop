using System.Linq.Expressions;
using BuildingBlock.Persistence.Abstractions.Specifications;

namespace BuildingBlock.Foundation.Abstractions.UnitTest.Specifications;

public sealed class SpecificationSpec
{
    [Fact]
    public void Specification_Should_Expose_Criteria_Includes_And_Ordered_Sorts()
    {
        var specification = new ActiveOrdersSpecification();

        specification.Criteria!.Compile()(new Order(true, 5, "A")).ShouldBeTrue();
        specification.Criteria.Compile()(new Order(false, 5, "B")).ShouldBeFalse();
        specification.Includes.Count.ShouldBe(1);
        specification.Includes[0].Compile()(new Order(true, 5, "A")).ShouldBe(5);
        specification.OrderExpressions.Count.ShouldBe(2);
        specification.OrderExpressions[0].Descending.ShouldBeFalse();
        specification.OrderExpressions[0].KeySelector.Compile()
            .DynamicInvoke(new Order(true, 5, "A")).ShouldBe(5);
        specification.OrderExpressions[1].Descending.ShouldBeTrue();
        specification.OrderExpressions[1].KeySelector.Compile()
            .DynamicInvoke(new Order(true, 5, "A")).ShouldBe("A");
    }

    [Fact]
    public void Projected_Specification_Should_Expose_Criteria_And_Selector()
    {
        var specification = new OrderNumberSpecification();

        specification.Criteria!.Compile()(new Order(true, 7, "A")).ShouldBeTrue();
        specification.Selector.Compile()(new Order(true, 7, "A")).ShouldBe("A");
    }

    [Fact]
    public void Specification_Should_Snapshot_Supplied_Include_And_Order_Sequences()
    {
        var includes = new List<Expression<Func<Order, object?>>>
        {
            order => order.Number
        };
        var orders = new List<OrderExpression<Order>>
        {
            OrderExpression<Order>.AscendingBy(order => order.Number)
        };
        var specification = new SnapshotSpecification(includes, orders);

        includes.Clear();
        orders.Clear();

        specification.Includes.Count.ShouldBe(1);
        specification.OrderExpressions.Count.ShouldBe(1);
    }

    private sealed class ActiveOrdersSpecification()
        : Specification<Order>(
            order => order.IsActive,
            [order => order.Number],
            [
                OrderExpression<Order>.AscendingBy(order => order.Number),
                OrderExpression<Order>.DescendingBy(order => order.Customer)
            ]);

    private sealed class OrderNumberSpecification()
        : Specification<Order, string>(
            order => order.IsActive,
            order => order.Customer);

    private sealed class SnapshotSpecification(
        IEnumerable<Expression<Func<Order, object?>>> includes,
        IEnumerable<OrderExpression<Order>> orderExpressions)
        : Specification<Order>(includes: includes, orderExpressions: orderExpressions);

    private sealed record Order(bool IsActive, int Number, string Customer);
}
