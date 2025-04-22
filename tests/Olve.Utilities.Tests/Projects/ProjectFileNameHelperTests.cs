using Olve.Utilities.Projects;
using TUnit.Assertions.Enums;

namespace Olve.Utilities.Tests.Projects;

public class ProjectFileNameHelperTests
{
    [Test]
    public async Task GetFileName_WithDelimiterAndExtension_ReturnsCorrectFileName()
    {
        // Arrange
        const string delimiter = "_";
        const string extension = ".txt";
        const string expected = "a_b_c.txt";
        var helper = new ProjectFileNameHelper(delimiter, extension);

        string[] elements = ["a", "b", "c"];

        // Act
        var fileName = helper.GetFileName(elements);

        // Assert
        await Assert.That(fileName).IsEqualTo(expected);
    }

    [Test]
    public async Task GetFileName_WithNoDelimiterAndExtension_ReturnsCorrectFileName()
    {
        // Arrange
        const string extension = ".txt";
        const string expected = "abc.txt";
        var helper = new ProjectFileNameHelper(extension: extension);

        string[] elements = ["a", "b", "c"];

        // Act
        var fileName = helper.GetFileName(elements);

        // Assert
        await Assert.That(fileName).IsEqualTo(expected);
    }

    [Test]
    public async Task GetFileName_WithDelimiterAndNoExtension_ReturnsCorrectFileName()
    {
        // Arrange
        const string delimiter = "_";
        const string expected = "a_b_c";
        var helper = new ProjectFileNameHelper(delimiter);

        string[] elements = ["a", "b", "c"];

        // Act
        var fileName = helper.GetFileName(elements);

        // Assert
        await Assert.That(fileName).IsEqualTo(expected);
    }

    [Test]
    public async Task GetFileName_WithNoDelimiterAndNoExtension_ReturnsCorrectFileName()
    {
        // Arrange
        const string expected = "abc";
        var helper = new ProjectFileNameHelper();

        string[] elements = ["a", "b", "c"];

        // Act
        var fileName = helper.GetFileName(elements);

        // Assert
        await Assert.That(fileName).IsEqualTo(expected);
    }

    [Test]
    public async Task GetFileName_WithEmptyElements_ReturnsEmptyString()
    {
        // Arrange
        var helper = new ProjectFileNameHelper();

        string[] elements = [];

        // Act
        var fileName = helper.GetFileName(elements);

        // Assert
        await Assert.That(fileName).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task GetElements_WithDelimiterAndExtension_ReturnsCorrectElements()
    {
        // Arrange
        const string delimiter = "_";
        const string extension = ".txt";
        const string fileName = "a_b_c.txt";
        var helper = new ProjectFileNameHelper(delimiter, extension);

        string[] expected = ["a", "b", "c"];

        // Act
        var elements = helper.GetElements(fileName);

        // Assert
        await Assert.That(elements).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task GetElements_WithNoDelimiterAndExtension_ReturnsCorrectElements()
    {
        // Arrange
        const string extension = ".txt";
        const string fileName = "abc.txt";
        var helper = new ProjectFileNameHelper(extension: extension);

        string[] expected = ["abc"];

        // Act
        var elements = helper.GetElements(fileName);

        // Assert
        await Assert.That(elements).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task GetElements_WithDelimiterAndNoExtension_ReturnsCorrectElements()
    {
        // Arrange
        const string delimiter = "_";
        const string fileName = "a_b_c";
        var helper = new ProjectFileNameHelper(delimiter);

        string[] expected = ["a", "b", "c"];

        // Act
        var elements = helper.GetElements(fileName);

        // Assert
        await Assert.That(elements).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task GetElements_WithNoDelimiterAndNoExtension_ReturnsCorrectElements()
    {
        // Arrange
        const string fileName = "abc";
        var helper = new ProjectFileNameHelper();

        string[] expected = ["abc"];

        // Act
        var elements = helper.GetElements(fileName);

        // Assert
        await Assert.That(elements).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task GetElements_WithEmptyFileName_ReturnsEmptyArray()
    {
        // Arrange
        var helper = new ProjectFileNameHelper();

        var fileName = string.Empty;

        // Act
        var elements = helper.GetElements(fileName);

        // Assert
        await Assert.That(elements).IsEmpty();
    }
}
