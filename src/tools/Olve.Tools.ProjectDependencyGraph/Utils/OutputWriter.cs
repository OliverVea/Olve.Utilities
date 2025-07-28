using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Olve.Tools.ProjectDependencyGraph.Models;

namespace Olve.Tools.ProjectDependencyGraph.Utils
{
    public static class OutputWriter
    {
        public static List<string> LoadCommonPaths(string? file)
        {
            if (string.IsNullOrWhiteSpace(file) || !File.Exists(file))
                return new List<string>();
            return File.ReadAllLines(file)
                       .Where(l => !string.IsNullOrWhiteSpace(l))
                       .ToList();
        }

        public static void Write(
            IEnumerable<string> common,
            IEnumerable<string> affectedProjects,
            Settings settings)
        {
            var lines = new List<string>();

            // 1. Static common
            lines.AddRange(common);

            // 2. Project globs
            foreach (var proj in affectedProjects
                         .Select(p => Path.GetFileNameWithoutExtension(p))
                         .Distinct()
                         .OrderBy(n => n))
            {
                lines.Add($"src/{proj}/**");
            }

            // 3. Emit
            if (!string.IsNullOrWhiteSpace(settings.Output))
            {
                File.WriteAllLines(settings.Output, lines);
            }
            else
            {
                foreach (var l in lines)
                    Console.WriteLine(l);
            }
        }
    }
}
