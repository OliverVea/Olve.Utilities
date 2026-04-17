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
}