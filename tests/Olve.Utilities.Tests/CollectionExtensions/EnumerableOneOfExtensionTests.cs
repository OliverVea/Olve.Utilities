using Olve.Utilities.CollectionExtensions;
using OneOf;

namespace Olve.Utilities.Tests.CollectionExtensions;

public class EnumerableOneOfExtensions
{
    [Test]
    public async Task WhereT0_OnT0OneOfEnumerable_ReturnsOnlyT0Items()
    {
        // Arrange
        var source = new List<OneOf<int>> { 1, 3 };

        // Act
        var result = source.OfT0();

        // Assert
        await Assert
            .That(result)
            .IsEquivalentTo([1, 3]);
    }

    [Test]
    public async Task WhereT0_OnT0T1OneOfEnumerable_ReturnsOnlyT0Items()
    {
        // Arrange
        var source = new List<OneOf<int, string>> { 1, "3" };

        // Act
        var result = source.OfT0();

        // Assert
        await Assert
            .That(result)
            .IsEquivalentTo([1]);
    }

    [Test]
    public async Task WhereT1_OnT0T1OneOfEnumerable_ReturnsOnlyT1Items()
    {
        // Arrange
        var source = new List<OneOf<int, string>> { 1, "3" };

        // Act
        var result = source.OfT1();

        // Assert
        await Assert
            .That(result)
            .IsEquivalentTo(["3"]);
    }
}