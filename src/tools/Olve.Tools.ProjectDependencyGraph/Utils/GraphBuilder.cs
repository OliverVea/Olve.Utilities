using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Graph;

namespace Olve.Tools.ProjectDependencyGraph.Utils
{
    public static class GraphBuilder
    {
        public static string FindSolutionFile(string repoRoot)
        {
            var files = Directory.GetFiles(repoRoot, "*.sln", SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
                throw new InvalidOperationException("No .sln file found in repo root");
            return files[0];
        }

        public static ProjectGraph BuildGraph(string solutionPath)
            => new ProjectGraph(solutionPath);

        public static Dictionary<string, List<string>> BuildReverseDependencyMap(ProjectGraph graph)
        {
            var map = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            foreach (var node in graph.ProjectNodes)
            {
                var src = node.ProjectInstance.FullPath;
                foreach (var dep in node.ProjectReferences)
                {
                    var key = dep.ProjectInstance.FullPath;
                    if (!map.TryGetValue(key, out var list))
                        map[key] = list = new List<string>();
                    list.Add(src);
                }
            }
            return map;
        }

        public static IEnumerable<string> SelectRootProjects(ProjectGraph graph, string? projectName)
        {
            if (string.IsNullOrWhiteSpace(projectName))
            {
                return graph.ProjectNodes
                    .Select(n => n.ProjectInstance.FullPath);
            }
            var match = graph.ProjectNodes
                .FirstOrDefault(n => Path.GetFileNameWithoutExtension(n.ProjectInstance.FullPath)
                    .Equals(projectName, StringComparison.OrdinalIgnoreCase));
            if (match == null)
                throw new InvalidOperationException($"Project '{projectName}' not found");
            return new[] { match.ProjectInstance.FullPath };
        }
    }
}
