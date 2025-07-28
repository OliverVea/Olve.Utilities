using System;
using System.Collections.Generic;

namespace Olve.Tools.ProjectDependencyGraph.Utils
{
    public static class ReverseDependencyResolver
    {
        public static HashSet<string> CollectAffectedProjects(
            string rootProjectPath,
            Dictionary<string, List<string>> reverseMap)
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var queue = new Queue<string>();
            queue.Enqueue(rootProjectPath);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (result.Add(current) && reverseMap.TryGetValue(current, out var parents))
                {
                    foreach (var p in parents)
                        queue.Enqueue(p);
                }
            }

            return result;
        }
    }
}
