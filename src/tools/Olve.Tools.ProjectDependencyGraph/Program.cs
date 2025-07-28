using Spectre.Console.Cli;
using Commands;

namespace Olve.Tools.ProjectDependencyGraph
{
    class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandApp<AnalyzeCommand>();
            return app.Run(args);
        }
    }
}
