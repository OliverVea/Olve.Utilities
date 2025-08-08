using Olve.Utilities.Projects;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

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
            var userHome = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var expected = System.IO.Path.Combine(userHome, ".local", "share", organization, projectName);
            await Assert.That(rootFolder).IsEqualTo(expected);
        }
    }
}