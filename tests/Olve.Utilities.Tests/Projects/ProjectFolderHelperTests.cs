using Olve.Utilities.Projects;

namespace Olve.Utilities.Tests.Projects;

public class ProjectFolderHelperTests
{
    [Test]
    public async Task ProjectRootFolder_DefaultRootPath_ReturnsValidOSPath()
    {
        // Arrange
        const string organization = "TestOrganization";
        const string projectName = "TestProject";
        var helper = new ProjectFolderHelper(organization: organization, projectName: projectName);

        // Act
        var rootFolderPath = helper.ProjectRootFolder;
        var rootFolder = rootFolderPath.Path;

        // Assert
        if (Environment.OSVersion.Platform == PlatformID.Unix)
        {
            await Assert.That(rootFolder).StartsWith("/home/");
            var elements = rootFolder
                .Split("/")
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();
            await Assert.That(elements.Length).IsEqualTo(6);
            await Assert.That(elements[0]).IsEqualTo("home");
            await Assert.That(elements[1]).IsEqualTo(Environment.UserName);
            await Assert.That(elements[2]).IsEqualTo(".local");
            await Assert.That(elements[3]).IsEqualTo("share");
            await Assert.That(elements[4]).IsEqualTo(organization);
            await Assert.That(elements[5]).IsEqualTo(projectName);
        }
    }
}
