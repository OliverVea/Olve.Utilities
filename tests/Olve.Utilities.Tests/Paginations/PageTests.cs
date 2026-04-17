using System.Text.Json;
using Olve.Utilities.Paginations;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace Olve.Utilities.Tests.Paginations;

public class PageTests
{
    [Test]
    public async Task TotalPages_DividesTotalByPageSizeRoundedUp()
    {
        var page = new Page<int>([1, 2, 3], PageNumber: 0, PageSize: 3, TotalCount: 10);

        await Assert.That(page.TotalPages).IsEqualTo(4);
    }

    [Test]
    public async Task TotalPages_PageSizeZero_ReturnsZero()
    {
        var page = new Page<int>([], PageNumber: 0, PageSize: 0, TotalCount: 5);

        await Assert.That(page.TotalPages).IsEqualTo(0);
    }

    [Test]
    public async Task Next_OnIntermediatePage_IsNextPagination()
    {
        var page = new Page<int>([1, 2], PageNumber: 0, PageSize: 2, TotalCount: 5);

        await Assert.That(page.Next).IsEqualTo(new Pagination(1, 2));
    }

    [Test]
    public async Task Next_OnLastPage_IsNull()
    {
        var page = new Page<int>([5, 6], PageNumber: 2, PageSize: 2, TotalCount: 6);

        await Assert.That(page.Next).IsNull();
    }

    [Test]
    public async Task Serialize_DefaultOptions_ProducesObjectWithItemsAndMetadata()
    {
        var page = new Page<string>(["a", "b"], PageNumber: 0, PageSize: 2, TotalCount: 5);
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        var json = JsonSerializer.Serialize(page, options);

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        await Assert.That(root.ValueKind).IsEqualTo(JsonValueKind.Object);
        await Assert.That(root.GetProperty("items").GetArrayLength()).IsEqualTo(2);
        await Assert.That(root.GetProperty("items")[0].GetString()).IsEqualTo("a");
        await Assert.That(root.GetProperty("pageNumber").GetInt32()).IsEqualTo(0);
        await Assert.That(root.GetProperty("pageSize").GetInt32()).IsEqualTo(2);
        await Assert.That(root.GetProperty("totalCount").GetInt32()).IsEqualTo(5);
        await Assert.That(root.GetProperty("totalPages").GetInt32()).IsEqualTo(3);
        await Assert.That(root.GetProperty("hasNextPage").GetBoolean()).IsTrue();
    }

    [Test]
    public async Task RoundTrip_PreservesItemsAndMetadata()
    {
        var original = new Page<string>(["x", "y", "z"], PageNumber: 2, PageSize: 3, TotalCount: 20);
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        var json = JsonSerializer.Serialize(original, options);
        var restored = JsonSerializer.Deserialize<Page<string>>(json, options);

        await Assert.That(restored).IsNotNull();
        await Assert.That(restored!.Items.Count).IsEqualTo(3);
        await Assert.That(restored.Items[0]).IsEqualTo("x");
        await Assert.That(restored.PageNumber).IsEqualTo(2);
        await Assert.That(restored.PageSize).IsEqualTo(3);
        await Assert.That(restored.TotalCount).IsEqualTo(20);
    }
}
