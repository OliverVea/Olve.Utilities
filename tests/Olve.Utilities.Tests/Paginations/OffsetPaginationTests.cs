using Olve.Results.TUnit;
using Olve.Utilities.Paginations;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace Olve.Utilities.Tests.Paginations;

public class OffsetPaginationTests
{
    [Test]
    [Arguments(0, 1)]
    [Arguments(37, 20)]
    [Arguments(0, OffsetPagination.DefaultMaxLimit)]
    public async Task Validate_ValidValues_Succeeds(int offset, int limit)
    {
        var result = new OffsetPagination(offset, limit).Validate();

        await Assert.That(result).Succeeded();
    }

    [Test]
    [Arguments(-1, 20)]
    [Arguments(0, 0)]
    [Arguments(0, -1)]
    [Arguments(0, OffsetPagination.DefaultMaxLimit + 1)]
    public async Task Validate_InvalidValues_Fails(int offset, int limit)
    {
        var result = new OffsetPagination(offset, limit).Validate();

        await Assert.That(result).Failed();
    }

    [Test]
    public async Task Validate_BothValuesInvalid_ReportsBothProblems()
    {
        var result = new OffsetPagination(-1, 0).Validate();

        await Assert.That(result.Problems).IsNotNull();
        await Assert.That(result.Problems!.Count()).IsEqualTo(2);
    }

    [Test]
    public async Task Validate_CustomMaxLimit_UsesIt()
    {
        var pagination = new OffsetPagination(0, 50);

        await Assert.That(pagination.Validate(maxLimit: 50)).Succeeded();
        await Assert.That(pagination.Validate(maxLimit: 49)).Failed();
    }

    [Test]
    [Arguments(37, 20, 37, 20)]
    [Arguments(-5, 20, 0, 20)]
    [Arguments(10, 0, 10, OffsetPagination.DefaultLimit)]
    [Arguments(10, -3, 10, OffsetPagination.DefaultLimit)]
    [Arguments(10, 1000, 10, OffsetPagination.DefaultMaxLimit)]
    public async Task Clamp_DefaultBounds_ReturnsValidPagination(int offset,
        int limit,
        int expectedOffset,
        int expectedLimit)
    {
        var clamped = new OffsetPagination(offset, limit).Clamp();

        await Assert.That(clamped).IsEqualTo(new OffsetPagination(expectedOffset, expectedLimit));
        await Assert.That(clamped.Validate()).Succeeded();
    }

    [Test]
    public async Task Clamp_Default_UsesDefaultLimit()
    {
        var clamped = default(OffsetPagination).Clamp(defaultLimit: 5, maxLimit: 10);

        await Assert.That(clamped).IsEqualTo(new OffsetPagination(0, 5));
    }

    [Test]
    public async Task Clamp_CustomMaxLimit_CapsLimit()
    {
        var clamped = new OffsetPagination(0, 50).Clamp(defaultLimit: 5, maxLimit: 10);

        await Assert.That(clamped).IsEqualTo(new OffsetPagination(0, 10));
    }

    [Test]
    [Arguments(0, 0)]
    [Arguments(5, 0)]
    [Arguments(11, 10)]
    public async Task Clamp_InvalidBounds_Throws(int defaultLimit, int maxLimit)
    {
        await Assert
            .That(() => new OffsetPagination(0, 1).Clamp(defaultLimit, maxLimit))
            .Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    [Arguments(0, 20, 0)]
    [Arguments(40, 20, 2)]
    [Arguments(7, 1, 7)]
    public async Task TryToPagination_OffsetIsMultipleOfLimit_ReturnsPagination(int offset,
        int limit,
        int expectedPage)
    {
        var success = new OffsetPagination(offset, limit).TryToPagination(out var pagination);

        await Assert.That(success).IsTrue();
        await Assert.That(pagination).IsEqualTo(new Pagination(expectedPage, limit));
    }

    [Test]
    [Arguments(37, 20)]
    [Arguments(0, 0)]
    [Arguments(10, 0)]
    [Arguments(-20, 20)]
    [Arguments(0, -1)]
    public async Task TryToPagination_NoEquivalentPage_ReturnsFalse(int offset, int limit)
    {
        var success = new OffsetPagination(offset, limit).TryToPagination(out var pagination);

        await Assert.That(success).IsFalse();
        await Assert.That(pagination).IsEqualTo(default(Pagination));
    }

    [Test]
    [Arguments(0, 20)]
    [Arguments(3, 20)]
    [Arguments(5, 1)]
    public async Task PaginationToOffsetPagination_RoundTrips(int page, int pageSize)
    {
        var pagination = new Pagination(page, pageSize);

        var offsetPagination = pagination.ToOffsetPagination();
        var success = offsetPagination.TryToPagination(out var roundTripped);

        await Assert.That(offsetPagination).IsEqualTo(new OffsetPagination(page * pageSize, pageSize));
        await Assert.That(success).IsTrue();
        await Assert.That(roundTripped).IsEqualTo(pagination);
    }
}
