namespace Olve.Utilities.Projects;

/// <summary>
///     Provides utility methods for managing project folders, including root directory configuration,
///     subfolder management, and file system searches.
/// </summary>
/// <param name="projectName">The name of the project.</param>
/// <param name="organization">The name of the organization. Defaults to "Olve" if not specified.</param>
/// <param name="specialFolder">
///     The special folder to use as the root. If <c>null</c>, defaults to the OS-specific folder
///     (e.g., CommonApplicationData on Windows, XDG_DATA_HOME on Linux).
/// </param>
public class ProjectFolderHelper(
    string projectName,
    string organization = ProjectFolderHelper.DefaultOrganization,
    Environment.SpecialFolder? specialFolder = null)
{
    private const string DefaultOrganization = "Olve";

    private string? _rootDirectoryOverride;

    /// <summary>
    ///     Gets the name of the project.
    /// </summary>
    public string ProjectName { get; } = projectName;

    /// <summary>
    ///     Gets the name of the organization.
    /// </summary>
    public string Organization { get; } = organization;

    /// <summary>
    ///     Gets the special folder used as the root directory, if specified.
    /// </summary>
    public Environment.SpecialFolder? SpecialFolder { get; } = specialFolder;

    /// <summary>
    ///     Gets or sets the root directory for the project.
    ///     If not explicitly set, it defaults to the specified <see cref="SpecialFolder"/> or an OS-specific folder.
    /// </summary>
    public string RootDirectory
    {
        get =>
            _rootDirectoryOverride ?? (SpecialFolder is { } specialFolder
                ? Environment.GetFolderPath(specialFolder)
                : GetDefaultFolderForOperatingSystem());
        set => _rootDirectoryOverride = value;
    }

    /// <summary>
    ///     Gets the full path to the project's root folder, combining the root directory, organization, and project name.
    /// </summary>
    public string ProjectRootFolder => Path.Combine(
        RootDirectory,
        Organization,
        ProjectName);

    /// <summary>
    ///     Indicates whether the project folder exists on the file system.
    /// </summary>
    public bool ProjectFolderExists => Directory.Exists(ProjectRootFolder);

    /// <summary>
    ///     Gets the full path to a subfolder within the project folder.
    /// </summary>
    /// <param name="subfolderName">The name of the subfolder.</param>
    /// <returns>The full path to the specified subfolder.</returns>
    public string GetSubfolder(string subfolderName) => Path.Combine(ProjectRootFolder, subfolderName);

    /// <summary>
    ///     Searches for files and directories within the project folder that match the specified search pattern.
    /// </summary>
    /// <param name="searchPattern">The search pattern to use (e.g., "*.txt" for text files).</param>
    /// <returns>An enumerable collection of matching file system entries. Returns an empty collection if the folder doesn't exist.</returns>
    public IEnumerable<string> Search(string searchPattern) => ProjectFolderExists
        ? Directory.GetFileSystemEntries(ProjectRootFolder, searchPattern)
        : Enumerable.Empty<string>();

    /// <summary>
    ///     Determines the default root folder based on the operating system.
    ///     On Linux/macOS, it uses XDG_DATA_HOME or falls back to ~/.local/share.
    ///     On Windows, it defaults to CommonApplicationData.
    /// </summary>
    /// <returns>The default root folder path for the current operating system.</returns>
    private string GetDefaultFolderForOperatingSystem()
    {
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            return Environment.GetEnvironmentVariable("XDG_DATA_HOME")
                   ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share");
        }

        return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
    }
}
