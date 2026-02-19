using Olve.Utilities.Collections;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace Olve.Utilities.Tests.Collections;

public class OneToManyLookupTests
{
    private static IOneToManyLookup<TLeft, TRight> GetNewOneToManyLookup<TLeft, TRight>()
        where TLeft : notnull where TRight : notnull => new OneToManyLookup<TLeft, TRight>();

    [Test]
    public async Task Set_AddsNewPair_ReturnsTrue()
    {
        // Arrange
        var lookup = GetNewOneToManyLookup<int, string>();

        // Act
        var result = lookup.Set(1, "one", true);

        // Assert
        await Assert
            .That(result)
            .IsTrue();
        await Assert
            .That(lookup.Contains(1, "one"))
            .IsTrue();
    }

    [Test]
    public async Task Set_AddsExistingPair_ReturnsFalse()
    {
        // Arrange
        var lookup = GetNewOneToManyLookup<int, string>();
        lookup.Set(1, "one", true);

        // Act
        var result = lookup.Set(1, "one", true);

        // Assert
        await Assert
            .That(result)
            .IsFalse();
    }

    [Test]
    public async Task Set_RemovesExistingPair_ReturnsTrue()
    {
        // Arrange
        var lookup = GetNewOneToManyLookup<int, string>();
        lookup.Set(1, "one", true);

        // Act
        var result = lookup.Set(1, "one", false);

        // Assert
        await Assert
            .That(result)
            .IsTrue();
        await Assert
            .That(lookup.Contains(1, "one"))
            .IsFalse();
    }

    [Test]
    public async Task TryGet_LeftExists_ReturnsAssociatedRights()
    {
        // Arrange
        var lookup = GetNewOneToManyLookup<int, string>();
        lookup.Set(1, new HashSet<string> { "one", "uno" });

        // Act
        var found = lookup.TryGet(1, out var rights);

        // Assert
        await Assert
            .That(found)
            .IsTrue();
        await Assert
            .That(rights!)
            .Contains("one");
        await Assert
            .That(rights!)
            .Contains("uno");
    }

    [Test]
    public async Task TryGet_LeftDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var lookup = GetNewOneToManyLookup<int, string>();

        // Act
        var found = lookup.TryGet(1, out _);

        // Assert
        await Assert
            .That(found)
            .IsFalse();
    }

    [Test]
    public async Task Remove_LeftRemovesMappings_ReturnsTrue()
    {
        // Arrange
        var lookup = GetNewOneToManyLookup<int, string>();
        lookup.Set(1, "one", true);

        // Act
        var result = lookup.Remove(1);

        // Assert
        await Assert
            .That(result)
            .IsTrue();
        await Assert
            .That(lookup.Contains(1, "one"))
            .IsFalse();
    }

    [Test]
    public async Task Clear_RemovesAllMappings()
    {
        // Arrange
        var lookup = GetNewOneToManyLookup<int, string>();
        lookup.Set(1, "one", true);
        lookup.Set(2, "two", true);

        // Act
        lookup.Clear();

        // Assert
        await Assert
            .That(lookup.Contains(1, "one"))
            .IsFalse();
        await Assert
            .That(lookup.Contains(2, "two"))
            .IsFalse();
    }

    [Test]
    public async Task Set_OverwritesExistingMappingsForLeft()
    {
        // Arrange
        var lookup = GetNewOneToManyLookup<int, string>();
        lookup.Set(1, new HashSet<string> { "one", "uno" });

        // Act
        lookup.Set(1, new HashSet<string> { "two" });

        // Assert
        var found = lookup.TryGet(1, out var rights);
        await Assert
            .That(found)
            .IsTrue();
        await Assert
            .That(rights!)
            .Contains("two");
        await Assert
            .That(rights!)
            .DoesNotContain("one");
    }

    [Test]
    public async Task Set_OverwritesExistingMappingsForRight()
    {
        // Arrange
        var lookup = GetNewOneToManyLookup<string, int>();
        lookup.Set("one", new HashSet<int> { 1, 2 });

        // Act
        lookup.Set("one", new HashSet<int> { 3 });

        // Assert
        var found = lookup.TryGet("one", out var rights);
        await Assert
            .That(found)
            .IsTrue();
        await Assert
            .That(rights!)
            .Contains(3);
        await Assert
            .That(rights!)
            .DoesNotContain(1);
    }

    [Test]
    public async Task Set_RemovingNonexistentPair_ReturnsFalse()
    {
        // Arrange
        var lookup = GetNewOneToManyLookup<int, string>();

        // Act
        var result = lookup.Set(1, "one", false);

        // Assert
        await Assert
            .That(result)
            .IsFalse();
    }

    [Test]
    public async Task Remove_RightRemovesMappings_ReturnsTrue()
    {
        // Arrange
        var lookup = GetNewOneToManyLookup<int, string>();
        lookup.Set(1, "one", true);

        // Act
        var result = lookup.Remove("one");

        // Assert
        await Assert
            .That(result)
            .IsTrue();
        await Assert
            .That(lookup.Contains(1, "one"))
            .IsFalse();
    }

    [Test]
    public async Task Contains_NonExistentPair_ReturnsFalse()
    {
        // Arrange
        var lookup = GetNewOneToManyLookup<int, string>();

        // Act
        var result = lookup.Contains(1, "one");

        // Assert
        await Assert
            .That(result)
            .IsFalse();
    }

    [Test]
    public async Task TryGet_RightExists_ReturnsAssociatedLeft()
    {
        // Arrange
        var lookup = GetNewOneToManyLookup<int, string>();
        lookup.Set(1, "one", true);

        // Act
        var found = lookup.TryGet("one", out var left);

        // Assert
        await Assert
            .That(found)
            .IsTrue();
        await Assert
            .That(left)
            .IsEqualTo(1);
    }

    [Test]
    public async Task TryGet_RightDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var lookup = GetNewOneToManyLookup<int, string>();

        // Act
        var found = lookup.TryGet("one", out _);

        // Assert
        await Assert
            .That(found)
            .IsFalse();
    }

    [Test]
    public Task Clear_EmptyLookup_DoesNothing()
    {
        // Arrange
        var lookup = GetNewOneToManyLookup<int, string>();

        // Act
        lookup.Clear();

        // Assert
        return Task.CompletedTask;
    }

    [Test]
    public async Task Remove_NonexistentLeft_ReturnsFalse()
    {
        // Arrange
        var lookup = GetNewOneToManyLookup<int, string>();

        // Act
        var result = lookup.Remove(1);

        // Assert
        await Assert
            .That(result)
            .IsFalse();
    }

    [Test]
    public async Task Remove_NonexistentRight_ReturnsFalse()
    {
        // Arrange
        var lookup = GetNewOneToManyLookup<int, string>();

        // Act
        var result = lookup.Remove("one");

        // Assert
        await Assert
            .That(result)
            .IsFalse();
    }

    [Test]
    public async Task Set_AddingDuplicateRightToLeft_IsIdempotent()
    {
        // Arrange
        var lookup = GetNewOneToManyLookup<int, string>();
        lookup.Set(1, "one", true);

        // Act
        var result = lookup.Set(1, "one", true);

        // Assert
        await Assert
            .That(result)
            .IsFalse();
    }

    [Test]
    public async Task Set_AddingDuplicateLeftToRight_IsIdempotent()
    {
        // Arrange
        var lookup = GetNewOneToManyLookup<string, int>();
        lookup.Set("one", 1, true);

        // Act
        var result = lookup.Set("one", 1, true);

        // Assert
        await Assert
            .That(result)
            .IsFalse();
    }

    [Test]
    public async Task Set_EmptyMappingsForLeft_RemovesPreviousMappings()
    {
        // Arrange
        var lookup = GetNewOneToManyLookup<int, string>();
        lookup.Set(1, new HashSet<string> { "one", "uno" });

        // Act
        lookup.Set(1, new HashSet<string>());

        // Assert
        var found = lookup.TryGet(1, out _);
        await Assert
            .That(found)
            .IsFalse();
    }

    [Test]
    public async Task Set_EmptyMappingsForRight_RemovesPreviousMappings()
    {
        // Arrange
        var lookup = GetNewOneToManyLookup<string, int>();
        lookup.Set("one", new HashSet<int> { 1, 2 });

        // Act
        lookup.Set("one", new HashSet<int>());

        // Assert
        var found = lookup.TryGet("one", out _);
        await Assert
            .That(found)
            .IsFalse();
    }
}
