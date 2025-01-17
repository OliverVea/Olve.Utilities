namespace Olve.Utilities.Projects;

/// <summary>
///     Helper class for managing project folders.
/// </summary>
/// <param name="projectName">The name of the project.</param>
/// <param name="organization">The name of the organization. Default is "Olve".</param>
/// <param name="specialFolder">
///     The special folder to use. Default is
///     <see cref="Environment.SpecialFolder.CommonApplicationData" />.
/// </param>
public class ProjectFolderHelper(
    string projectName,
    string organization = ProjectFolderHelper.DefaultOrganization,
    Environment.SpecialFolder specialFolder = Environment.SpecialFolder.CommonApplicationData)
{
    private const string DefaultOrganization = "Olve";

    /// <summary>
    ///     The name of the project.
    /// </summary>
    public string ProjectName { get; } = projectName;

    /// <summary>
    ///     The name of the organization.
    /// </summary>
    public string Organization { get; } = organization;

    /// <summary>
    ///     The special folder to use.
    /// </summary>
    public Environment.SpecialFolder SpecialFolder { get; } = specialFolder;

    /// <summary>
    ///     The root folder of the project.
    /// </summary>
    public string ProjectRootFolder => Path.Combine(
        Environment.GetFolderPath(SpecialFolder),
        Organization,
        ProjectName);

    /// <summary>
    ///     Checks if the project folder exists.
    /// </summary>
    public bool ProjectFolderExists => Directory.Exists(ProjectRootFolder);

    /// <summary>
    ///     Gets a subfolder of the project.
    /// </summary>
    /// <param name="subfolderName">The name of the subfolder.</param>
    /// <returns>The path to the subfolder.</returns>
    public string GetSubfolder(string subfolderName) => Path.Combine(ProjectRootFolder, subfolderName);

    /// <summary>
    ///     Searches for elements in the project folder.
    /// </summary>
    /// <param name="searchPatten">The search pattern to use.</param>
    public IEnumerable<string> Search(string searchPatten) => ProjectFolderExists
        ? Directory.GetFileSystemEntries(ProjectRootFolder, searchPatten)
        : Enumerable.Empty<string>();
}