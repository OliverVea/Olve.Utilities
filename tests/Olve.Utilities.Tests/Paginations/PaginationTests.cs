using Olve.Utilities.Paginations;
using OneOf.Types;

namespace Olve.Utilities.Tests.Paginations;

public class PaginationTests
{
    [Test]
    [Arguments(0, 0, 0, false)]
    [Arguments(0, 0, 1, true)]
    [Arguments(0, 1, 1, false)]
    [Arguments(0, 1, 2, true)]
    [Arguments(1, 1, 2, false)]
    public async Task HasNextPage_VariousPaginatedResults_ReturnsCorrectValue(
        int page,
        int pageSize,
        int total,
        bool expected
    )
    {
        // Arrange
        var pagination = new Pagination(page, pageSize);

        var paginatedResult = new PaginatedResult<None>([], pagination, total);

        // Act
        var actual = paginatedResult.HasNextPage;

        // Assert
        await Assert.That(actual).IsEqualTo(expected);
    }
}
