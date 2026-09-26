using System.Text.Json;
using Olve.Utilities.Paginations;
using OneOf.Types;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace Olve.Utilities.Tests.Paginations;

public class SliceTests
{
    [Test]
    [Arguments(0, 0, 0, false)]
    [Arguments(0, 0, 1, true)]
    [Arguments(0, 1, 1, false)]
    [Arguments(0, 1, 2, true)]
    [Arguments(1, 1, 2, false)]
    [Arguments(37, 20, 57, false)]
    [Arguments(37, 20, 58, true)]
    public async Task HasMore_VariousSlices_ReturnsCorrectValue(int offset,
        int limit,
        int total,
        bool expected)
    {
        var slice = new Slice<None>([], offset, limit, total);

        await Assert.That(slice.HasMore).IsEqualTo(expected);
    }

    [Test]
    public async Task Next_OnIntermediateSlice_AdvancesOffsetByLimit()
    {
        var slice = new Slice<int>([38, 39], Offset: 37, Limit: 2, TotalCount: 50);

        await Assert.That(slice.Next).IsEqualTo(new OffsetPagination(39, 2));
    }

    [Test]
    public async Task Next_OnLastSlice_IsNull()
    {
        var slice = new Slice<int>([5, 6], Offset: 4, Limit: 2, TotalCount: 6);

        await Assert.That(slice.Next).IsNull();
    }

    [Test]
    public async Task TryToPage_OffsetIsMultipleOfLimit_ReturnsEquivalentPage()
    {
        var slice = new Slice<int>([5, 6], Offset: 4, Limit: 2, TotalCount: 10);

        var success = slice.TryToPage(out var page);

        await Assert.That(success).IsTrue();
        await Assert.That(page).IsNotNull();
        await Assert.That(page!.PageNumber).IsEqualTo(2);
        await Assert.That(page.PageSize).IsEqualTo(2);
        await Assert.That(page.TotalCount).IsEqualTo(10);
        await Assert.That(page.Items).IsEquivalentTo([5, 6]);
    }

    [Test]
    public async Task TryToPage_OffsetIsNotMultipleOfLimit_ReturnsFalse()
    {
        var slice = new Slice<int>([4, 5], Offset: 3, Limit: 2, TotalCount: 10);

        var success = slice.TryToPage(out var page);

        await Assert.That(success).IsFalse();
        await Assert.That(page).IsNull();
    }

    [Test]
    public async Task Serialize_DefaultOptions_ProducesObjectWithItemsAndMetadata()
    {
        var slice = new Slice<string>(["a", "b"], Offset: 3, Limit: 2, TotalCount: 6);
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        var json = JsonSerializer.Serialize(slice, options);

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        await Assert.That(root.ValueKind).IsEqualTo(JsonValueKind.Object);
        await Assert.That(root.GetProperty("items").GetArrayLength()).IsEqualTo(2);
        await Assert.That(root.GetProperty("items")[0].GetString()).IsEqualTo("a");
        await Assert.That(root.GetProperty("offset").GetInt32()).IsEqualTo(3);
        await Assert.That(root.GetProperty("limit").GetInt32()).IsEqualTo(2);
        await Assert.That(root.GetProperty("totalCount").GetInt32()).IsEqualTo(6);
        await Assert.That(root.GetProperty("hasMore").GetBoolean()).IsTrue();
        await Assert.That(root.GetProperty("next").GetProperty("offset").GetInt32()).IsEqualTo(5);
        await Assert.That(root.GetProperty("next").GetProperty("limit").GetInt32()).IsEqualTo(2);
    }

    [Test]
    public async Task RoundTrip_PreservesItemsAndMetadata()
    {
        var original = new Slice<string>(["x", "y", "z"], Offset: 37, Limit: 3, TotalCount: 50);
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        var json = JsonSerializer.Serialize(original, options);
        var restored = JsonSerializer.Deserialize<Slice<string>>(json, options);

        await Assert.That(restored).IsNotNull();
        await Assert.That(restored!.Items.Count).IsEqualTo(3);
        await Assert.That(restored.Items[0]).IsEqualTo("x");
        await Assert.That(restored.Offset).IsEqualTo(37);
        await Assert.That(restored.Limit).IsEqualTo(3);
        await Assert.That(restored.TotalCount).IsEqualTo(50);
    }
}
