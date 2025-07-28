using Spectre.Console.Cli;
using Olve.Tools.ProjectDependencyGraph.Models;
using Olve.Tools.ProjectDependencyGraph.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Olve.Tools.ProjectDependencyGraph.Commands
{
    public sealed class AnalyzeCommand : Command<Settings>
    {
        public override int Execute(CommandContext context, Settings settings)
        {
            // 1. Locate solution
            var solution = GraphBuilder.FindSolutionFile(settings.RepoRoot);

            // 2. Build MSBuild graph
            var graph = GraphBuilder.BuildGraph(solution);

            // 3. Build reverse dependency map
            var reverseMap = GraphBuilder.BuildReverseDependencyMap(graph);

            // 4. Determine which project nodes to process
            var roots = GraphBuilder.SelectRootProjects(graph, settings.Project);

            // 5. Load common paths
            var common = OutputWriter.LoadCommonPaths(settings.CommonPathsFile);

            // 6. Resolve affected projects
            var affected = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var root in roots)
            {
                var set = ReverseDependencyResolver.CollectAffectedProjects(root, reverseMap);
                foreach (var p in set)
                    affected.Add(p);
            }

            // 7. Write output
            OutputWriter.Write(common, affected, settings);

            return 0;
        }
    }
}
