using System.Xml.Linq;
using Olve.Results;
#pragma warning disable CS0618 // Type or member is obsolete

namespace Olve.OpenRaster.Parsing;

internal static class GroupReader
{
    public static Result ReadGroupElement(XElement element, List<Layer> layers, List<Group> groups)
    {
        if (element.GetAttribute("name").TryPickProblems(out var problems, out var name))
        {
            return problems.Prepend(new ResultProblem("could not get attribute 'name' from element '{0}'", element.Name));
        }

        if (element.GetAttribute("composite-op", "svg:src-over").TryPickProblems(out problems, out var compositeString)
            || CompositeOperation.FromKey(compositeString).TryPickProblems(out problems, out var compositeOperation))
        {
            return problems.Prepend(new ResultProblem("could not get composite operation from element '{0}'", element.Name));
        }

        if (element.GetFloatAttribute("opacity", 1f).TryPickProblems(out problems, out var opacity))
        {
            return problems.Prepend(new ResultProblem("could not get attribute 'opacity' from element '{0}'", element.Name));
        }

        if (element.GetVisibilityAttribute(Visibility.Visible).TryPickProblems(out problems, out var visibility))
        {
            return problems.Prepend(new ResultProblem("could not get attribute 'visibility' from element '{0}'", element.Name));
        }

        if (element.GetIntAttribute("x", 0).TryPickProblems(out problems, out var x))
        {
            return problems.Prepend(new ResultProblem("could not get attribute 'x' from element '{0}'", element.Name));
        }

        if (element.GetIntAttribute("y", 0).TryPickProblems(out problems, out var y))
        {
            return problems.Prepend(new ResultProblem("could not get attribute 'x' from element '{0}'", element.Name));
        }

        var layerIndex = layers.Count;

        var childElementResults = StackElementParser.ParseStackElements(element.Elements(), layers, groups);
        if (childElementResults.TryPickProblems(out problems))
        {
            return problems.Prepend(new ResultProblem("could not parse child element(s) of '{0}'", element.Name));
        }

        Group group = new()
        {
            Name = name,
            CompositeOperation = compositeOperation,
            Opacity = opacity,
            Visibility = visibility,
            X = x,
            Y = y,
            Layers = layers[layerIndex..],
        };

        foreach (var layer in group.Layers)
        {
            layer.Groups.Add(group);
        }

        groups.Add(group);

        return Result.Success();
    }
}