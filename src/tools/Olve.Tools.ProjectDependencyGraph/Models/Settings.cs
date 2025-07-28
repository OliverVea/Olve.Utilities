using Spectre.Console.Cli;
using System.ComponentModel;

namespace Olve.Tools.ProjectDependencyGraph.Models
{
    public sealed class Settings : CommandSettings
    {
        [CommandOption("--repo-root <PATH>")]
        [Description("Path to the repository root")]
        public string RepoRoot { get; set; } = ".";

        [CommandOption("--common-paths <FILE>")]
        [Description("Static paths file to prepend")]
        public string? CommonPathsFile { get; set; }

        [CommandOption("--project <NAME>")]
        [Description("Name of the project to analyze (optional)")]
        public string? Project { get; set; }

        [CommandOption("--output <FILE>")]
        [Description("Output file path (default: stdout)")]
        public string? Output { get; set; }
    }
}
