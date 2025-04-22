using Olve.Utilities.Collections;

namespace Olve.Utilities.Tests.Collections;

public class ManyToManyLookupTests
{
    private static IManyToManyLookup<T1, T2> GetNewBidirectionalDictionary<T1, T2>()
        where T1 : notnull
        where T2 : notnull => new ManyToManyLookup<T1, T2>();

    [Test]
    public async Task Set_AddsNewPair_ReturnsTrue()
    {
        // Arrange
        var lookup = GetNewBidirectionalDictionary<int, string>();

        // Act
        var result = lookup.Set(1, "one", true);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(lookup.Contains(1, "one")).IsTrue();
    }

    [Test]
    public async Task Set_AddsExistingPair_ReturnsFalse()
    {
        // Arrange
        var lookup = GetNewBidirectionalDictionary<int, string>();
        lookup.Set(1, "one", true);

        // Act
        var result = lookup.Set(1, "one", true);

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Set_RemovesExistingPair_ReturnsTrue()
    {
        // Arrange
        var lookup = GetNewBidirectionalDictionary<int, string>();
        lookup.Set(1, "one", true);

        // Act
        var result = lookup.Set(1, "one", false);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(lookup.Contains(1, "one")).IsFalse();
    }

    [Test]
    public async Task Get_LeftExists_ReturnsAssociatedRights()
    {
        // Arrange
        var lookup = GetNewBidirectionalDictionary<int, string>();
        lookup.Set(1, new HashSet<string> { "one", "uno" });

        // Act
        var result = lookup.Get(1);

        // Assert
        await Assert.That(result.IsT0).IsTrue();
        await Assert.That(result.AsT0).Contains("one");
        await Assert.That(result.AsT0).Contains("uno");
    }

    [Test]
    public async Task Get_LeftDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var lookup = GetNewBidirectionalDictionary<int, string>();

        // Act
        var result = lookup.Get(1);

        // Assert
        await Assert.That(result.IsT1).IsTrue();
    }

    [Test]
    public async Task Remove_LeftRemovesMappings_ReturnsTrue()
    {
        // Arrange
        var lookup = GetNewBidirectionalDictionary<int, string>();
        lookup.Set(1, "one", true);

        // Act
        var result = lookup.Remove(1);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(lookup.Contains(1, "one")).IsFalse();
    }

    [Test]
    public async Task Clear_RemovesAllMappings()
    {
        // Arrange
        var lookup = GetNewBidirectionalDictionary<int, string>();
        lookup.Set(1, "one", true);
        lookup.Set(2, "two", true);

        // Act
        lookup.Clear();

        // Assert
        await Assert.That(lookup.Contains(1, "one")).IsFalse();
        await Assert.That(lookup.Contains(2, "two")).IsFalse();
    }

    [Test]
    public async Task Set_OverwritesExistingMappingsForLeft()
    {
        // Arrange
        var lookup = GetNewBidirectionalDictionary<int, string>();
        lookup.Set(1, new HashSet<string> { "one", "uno" });

        // Act
        lookup.Set(1, new HashSet<string> { "two" });

        // Assert
        var result = lookup.Get(1);
        await Assert.That(result.IsT0).IsTrue();
        await Assert.That(result.AsT0).Contains("two");
        await Assert.That(result.AsT0).DoesNotContain("one");
    }

    [Test]
    public async Task Set_OverwritesExistingMappingsForRight()
    {
        // Arrange
        var lookup = GetNewBidirectionalDictionary<int, string>();
        lookup.Set("one", new HashSet<int> { 1, 2 });

        // Act
        lookup.Set("one", new HashSet<int> { 3 });

        // Assert
        var result = lookup.Get("one");
        await Assert.That(result.IsT0).IsTrue();
        await Assert.That(result.AsT0).Contains(3);
        await Assert.That(result.AsT0).DoesNotContain(1);
    }

    [Test]
    public async Task Set_RemovingNonexistentPair_ReturnsFalse()
    {
        // Arrange
        var lookup = GetNewBidirectionalDictionary<int, string>();

        // Act
        var result = lookup.Set(1, "one", false);

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Remove_RightRemovesMappings_ReturnsTrue()
    {
        // Arrange
        var lookup = GetNewBidirectionalDictionary<int, string>();
        lookup.Set(1, "one", true);

        // Act
        var result = lookup.Remove("one");

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(lookup.Contains(1, "one")).IsFalse();
    }

    [Test]
    public async Task Contains_NonExistentPair_ReturnsFalse()
    {
        // Arrange
        var lookup = GetNewBidirectionalDictionary<int, string>();

        // Act
        var result = lookup.Contains(1, "one");

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Get_RightExists_ReturnsAssociatedLefts()
    {
        // Arrange
        var lookup = GetNewBidirectionalDictionary<int, string>();
        lookup.Set(1, "one", true);
        lookup.Set(2, "one", true);

        // Act
        var result = lookup.Get("one");

        // Assert
        await Assert.That(result.IsT0).IsTrue();
        await Assert.That(result.AsT0).Contains(1);
        await Assert.That(result.AsT0).Contains(2);
    }

    [Test]
    public async Task Get_RightDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var lookup = GetNewBidirectionalDictionary<int, string>();

        // Act
        var result = lookup.Get("one");

        // Assert
        await Assert.That(result.IsT1).IsTrue();
    }

    [Test]
    public Task Clear_EmptyLookup_DoesNothing()
    {
        // Arrange
        var lookup = GetNewBidirectionalDictionary<int, string>();

        // Act
        lookup.Clear();

        // Assert
        return Task.CompletedTask;
    }

    [Test]
    public async Task Remove_NonexistentLeft_ReturnsFalse()
    {
        // Arrange
        var lookup = GetNewBidirectionalDictionary<int, string>();

        // Act
        var result = lookup.Remove(1);

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Remove_NonexistentRight_ReturnsFalse()
    {
        // Arrange
        var lookup = GetNewBidirectionalDictionary<int, string>();

        // Act
        var result = lookup.Remove("one");

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Set_AddingDuplicateRightToLeft_IsIdempotent()
    {
        // Arrange
        var lookup = GetNewBidirectionalDictionary<int, string>();
        lookup.Set(1, "one", true);

        // Act
        var result = lookup.Set(1, "one", true);

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Set_AddingDuplicateLeftToRight_IsIdempotent()
    {
        // Arrange
        var lookup = GetNewBidirectionalDictionary<int, string>();
        lookup.Set("one", 1, true);

        // Act
        var result = lookup.Set("one", 1, true);

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Set_EmptyMappingsForLeft_RemovesPreviousMappings()
    {
        // Arrange
        var lookup = GetNewBidirectionalDictionary<int, string>();
        lookup.Set(1, new HashSet<string> { "one", "uno" });

        // Act
        lookup.Set(1, new HashSet<string>());

        // Assert
        var result = lookup.Get(1);
        await Assert.That(result.IsT1).IsTrue();
    }

    [Test]
    public async Task Set_EmptyMappingsForRight_RemovesPreviousMappings()
    {
        // Arrange
        var lookup = GetNewBidirectionalDictionary<int, string>();
        lookup.Set("one", new HashSet<int> { 1, 2 });

        // Act
        lookup.Set("one", new HashSet<int>());

        // Assert
        var result = lookup.Get("one");
        await Assert.That(result.IsT1).IsTrue();
    }

    [Test]
    public async Task Enumeration_EmptyLookup_YieldsNoResults()
    {
        // Arrange
        var lookup = GetNewBidirectionalDictionary<int, string>();

        // Act
        var result = lookup.ToList();

        // Assert
        await Assert.That(result).IsEmpty();
    }

    [Test]
    public async Task Enumeration_SingleMapping_YieldsCorrectPair()
    {
        // Arrange
        var lookup = GetNewBidirectionalDictionary<int, string>();
        lookup.Set(1, "one", true);

        // Act
        var result = lookup.ToList();

        // Assert
        await Assert.That(result.Count).IsEqualTo(1);
        await Assert.That(result[0].Key).IsEqualTo(1);
        await Assert.That(result[0].Value).IsEqualTo("one");
    }

    [Test]
    public async Task Enumeration_MultipleMappings_YieldsAllPairs()
    {
        // Arrange
        var lookup = GetNewBidirectionalDictionary<int, string>();
        lookup.Set(1, "one", true);
        lookup.Set(2, "two", true);
        lookup.Set(3, "three", true);

        // Act
        var result = lookup.ToList();

        // Assert
        await Assert.That(result.Count).IsEqualTo(3);
        await Assert.That(result).Contains(new KeyValuePair<int, string>(1, "one"));
        await Assert.That(result).Contains(new KeyValuePair<int, string>(2, "two"));
        await Assert.That(result).Contains(new KeyValuePair<int, string>(3, "three"));
    }

    [Test]
    public async Task Enumeration_AfterRemove_DoesNotYieldRemovedPairs()
    {
        // Arrange
        var lookup = GetNewBidirectionalDictionary<int, string>();
        lookup.Set(1, "one", true);
        lookup.Set(2, "two", true);

        // Act
        lookup.Remove(1);
        var result = lookup.ToList();

        // Assert
        await Assert.That(result.Count).IsEqualTo(1);
        await Assert.That(result).Contains(new KeyValuePair<int, string>(2, "two"));
        await Assert.That(result).DoesNotContain(new KeyValuePair<int, string>(1, "one"));
    }
}
