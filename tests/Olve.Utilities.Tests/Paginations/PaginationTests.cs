using Olve.Results.TUnit;
using Olve.Utilities.Paginations;
using OneOf.Types;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace Olve.Utilities.Tests.Paginations;

public class PaginationTests
{
    [Test]
    [Arguments(0, 0, 0, false)]
    [Arguments(0, 0, 1, true)]
    [Arguments(0, 1, 1, false)]
    [Arguments(0, 1, 2, true)]
    [Arguments(1, 1, 2, false)]
    public async Task HasNextPage_VariousPages_ReturnsCorrectValue(int pageNumber,
        int pageSize,
        int total,
        bool expected)
    {
        // Arrange
        var page = new Page<None>([], pageNumber, pageSize, total);

        // Act
        var actual = page.HasNextPage;

        // Assert
        await Assert
            .That(actual)
            .IsEqualTo(expected);
    }

    [Test]
    [Arguments(0, 1)]
    [Arguments(3, 20)]
    [Arguments(0, Pagination.DefaultMaxPageSize)]
    public async Task Validate_ValidValues_Succeeds(int page, int pageSize)
    {
        var result = new Pagination(page, pageSize).Validate();

        await Assert.That(result).Succeeded();
    }

    [Test]
    [Arguments(-1, 20)]
    [Arguments(0, 0)]
    [Arguments(0, -1)]
    [Arguments(0, Pagination.DefaultMaxPageSize + 1)]
    public async Task Validate_InvalidValues_Fails(int page, int pageSize)
    {
        var result = new Pagination(page, pageSize).Validate();

        await Assert.That(result).Failed();
    }

    [Test]
    public async Task Validate_BothValuesInvalid_ReportsBothProblems()
    {
        var result = new Pagination(-1, 0).Validate();

        await Assert.That(result.Problems).IsNotNull();
        await Assert.That(result.Problems!.Count()).IsEqualTo(2);
    }

    [Test]
    public async Task Validate_CustomMaxPageSize_UsesIt()
    {
        var pagination = new Pagination(0, 50);

        await Assert.That(pagination.Validate(maxPageSize: 50)).Succeeded();
        await Assert.That(pagination.Validate(maxPageSize: 49)).Failed();
    }

    [Test]
    [Arguments(3, 20, 3, 20)]
    [Arguments(-5, 20, 0, 20)]
    [Arguments(1, 0, 1, Pagination.DefaultPageSize)]
    [Arguments(1, -3, 1, Pagination.DefaultPageSize)]
    [Arguments(1, 1000, 1, Pagination.DefaultMaxPageSize)]
    public async Task Clamp_DefaultBounds_ReturnsValidPagination(int page,
        int pageSize,
        int expectedPage,
        int expectedPageSize)
    {
        var clamped = new Pagination(page, pageSize).Clamp();

        await Assert.That(clamped).IsEqualTo(new Pagination(expectedPage, expectedPageSize));
        await Assert.That(clamped.Validate()).Succeeded();
    }

    [Test]
    public async Task Clamp_Default_UsesDefaultPageSize()
    {
        var clamped = default(Pagination).Clamp(defaultPageSize: 5, maxPageSize: 10);

        await Assert.That(clamped).IsEqualTo(new Pagination(0, 5));
    }

    [Test]
    public async Task Clamp_CustomMaxPageSize_CapsPageSize()
    {
        var clamped = new Pagination(0, 50).Clamp(defaultPageSize: 5, maxPageSize: 10);

        await Assert.That(clamped).IsEqualTo(new Pagination(0, 10));
    }

    [Test]
    [Arguments(0, 0)]
    [Arguments(5, 0)]
    [Arguments(11, 10)]
    public async Task Clamp_InvalidBounds_Throws(int defaultPageSize, int maxPageSize)
    {
        await Assert
            .That(() => new Pagination(0, 1).Clamp(defaultPageSize, maxPageSize))
            .Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task ToOffsetPagination_ReturnsOffsetAndPageSize()
    {
        var offsetPagination = new Pagination(3, 20).ToOffsetPagination();

        await Assert.That(offsetPagination).IsEqualTo(new OffsetPagination(60, 20));
    }
}
