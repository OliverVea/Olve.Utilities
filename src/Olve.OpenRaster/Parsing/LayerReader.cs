using System.Xml.Linq;
using Olve.Results;

namespace Olve.OpenRaster.Parsing;

internal static class LayerReader
{
    public static Result ReadLayerElement(XElement element, List<Layer> layers, List<Group> groups)
    {
        if (element.Elements().Any())
        {
            return new ResultProblem("layer element is not expected to have children");
        }

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
            return problems.Prepend(new ResultProblem("could not get attribute 'y' from element '{0}'", element.Name));
        }

        if (element.GetAttribute("src").TryPickProblems(out problems, out var source))
        {
            return problems.Prepend(new ResultProblem("could not get attribute 'src' from element '{0}'", element.Name));
        }

        Layer layer = new()
        {
            Name = name,
            Source = source,
            CompositeOperation = compositeOperation,
            Opacity = opacity,
            Visible = visibility == Visibility.Visible,
            X = x,
            Y = y,
        };

        layers.Add(layer);
        return Result.Success();
    }
}