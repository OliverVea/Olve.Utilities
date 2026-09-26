using System.Collections;
using System.Linq.Expressions;
using Olve.Utilities.Paginations;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace Olve.Utilities.Tests.Paginations;

public class PaginationExtensionsTests
{
    private static readonly int[] Numbers = Enumerable.Range(0, 10).ToArray();

    private static IEnumerable<int> LazyNumbers()
    {
        foreach (var number in Numbers)
        {
            yield return number;
        }
    }

    [Test]
    public async Task Paginate_EnumerableWithPagination_ReturnsPage()
    {
        var page = LazyNumbers().Paginate(new Pagination(1, 3));

        await Assert.That(page.Items).IsEquivalentTo([3, 4, 5]);
        await Assert.That(page.PageNumber).IsEqualTo(1);
        await Assert.That(page.PageSize).IsEqualTo(3);
        await Assert.That(page.TotalCount).IsEqualTo(10);
        await Assert.That(page.Next).IsEqualTo(new Pagination(2, 3));
    }

    [Test]
    public async Task Paginate_EnumerableWithPaginationPastEnd_ReturnsEmptyPage()
    {
        var page = Numbers.Paginate(new Pagination(5, 3));

        await Assert.That(page.Items).IsEmpty();
        await Assert.That(page.TotalCount).IsEqualTo(10);
        await Assert.That(page.HasNextPage).IsFalse();
    }

    [Test]
    public async Task Paginate_EnumerableWithOffsetPagination_ReturnsSlice()
    {
        var slice = LazyNumbers().Paginate(new OffsetPagination(7, 2));

        await Assert.That(slice.Items).IsEquivalentTo([7, 8]);
        await Assert.That(slice.Offset).IsEqualTo(7);
        await Assert.That(slice.Limit).IsEqualTo(2);
        await Assert.That(slice.TotalCount).IsEqualTo(10);
        await Assert.That(slice.Next).IsEqualTo(new OffsetPagination(9, 2));
    }

    [Test]
    public async Task Paginate_EnumerableWithOffsetPaginationAtEnd_ReturnsPartialLastSlice()
    {
        var slice = Numbers.Paginate(new OffsetPagination(8, 5));

        await Assert.That(slice.Items).IsEquivalentTo([8, 9]);
        await Assert.That(slice.HasMore).IsFalse();
        await Assert.That(slice.Next).IsNull();
    }

    [Test]
    public async Task Paginate_QueryableWithPagination_ComposesCountSkipTakeOntoQuery()
    {
        var source = new TrackingQueryable<int>(Numbers.AsQueryable());

        var page = source.Paginate(new Pagination(1, 3));

        await Assert.That(page.Items).IsEquivalentTo([3, 4, 5]);
        await Assert.That(page.TotalCount).IsEqualTo(10);
        await Assert.That(source.Methods).Contains(nameof(Queryable.Count));
        await Assert.That(source.Methods).Contains(nameof(Queryable.Skip));
        await Assert.That(source.Methods).Contains(nameof(Queryable.Take));
    }

    [Test]
    public async Task Paginate_QueryableWithOffsetPagination_ComposesCountSkipTakeOntoQuery()
    {
        var source = new TrackingQueryable<int>(Numbers.AsQueryable());

        var slice = source.Paginate(new OffsetPagination(7, 2));

        await Assert.That(slice.Items).IsEquivalentTo([7, 8]);
        await Assert.That(slice.TotalCount).IsEqualTo(10);
        await Assert.That(source.Methods).Contains(nameof(Queryable.Count));
        await Assert.That(source.Methods).Contains(nameof(Queryable.Skip));
        await Assert.That(source.Methods).Contains(nameof(Queryable.Take));
    }

    [Test]
    [Arguments(-1, 3)]
    [Arguments(0, 0)]
    [Arguments(0, -1)]
    public async Task Paginate_InvalidPagination_Throws(int position, int size)
    {
        await Assert.That(() => Numbers.Paginate(new Pagination(position, size)))
            .Throws<ArgumentOutOfRangeException>();
        await Assert.That(() => Numbers.AsQueryable().Paginate(new Pagination(position, size)))
            .Throws<ArgumentOutOfRangeException>();
        await Assert.That(() => Numbers.Paginate(new OffsetPagination(position, size)))
            .Throws<ArgumentOutOfRangeException>();
        await Assert.That(() => Numbers.AsQueryable().Paginate(new OffsetPagination(position, size)))
            .Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task Paginate_PageAndEquivalentOffsetPagination_ReturnSameItems()
    {
        var pagination = new Pagination(2, 3);

        var page = Numbers.Paginate(pagination);
        var slice = Numbers.Paginate(pagination.ToOffsetPagination());

        await Assert.That(slice.Items).IsEquivalentTo(page.Items);
        await Assert.That(page.ToSlice()).IsEqualTo(slice with { Items = page.Items });
    }

    /// <summary>Records the <see cref="Queryable" /> methods composed onto or executed against a query.</summary>
    private sealed class TrackingQueryable<T>(IQueryable<T> inner, List<string>? methods = null) : IQueryable<T>
    {
        public List<string> Methods { get; } = methods ?? [];

        public Type ElementType => inner.ElementType;
        public Expression Expression => inner.Expression;
        public IQueryProvider Provider => new TrackingProvider(inner.Provider, Methods);

        public IEnumerator<T> GetEnumerator() => inner.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private sealed class TrackingProvider(IQueryProvider inner, List<string> methods) : IQueryProvider
    {
        public IQueryable CreateQuery(Expression expression) => throw new NotSupportedException();

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            Record(expression);
            return new TrackingQueryable<TElement>(inner.CreateQuery<TElement>(expression), methods);
        }

        public object? Execute(Expression expression) => throw new NotSupportedException();

        public TResult Execute<TResult>(Expression expression)
        {
            Record(expression);
            return inner.Execute<TResult>(expression);
        }

        private void Record(Expression expression)
        {
            if (expression is MethodCallExpression call)
            {
                methods.Add(call.Method.Name);
            }
        }
    }
}
