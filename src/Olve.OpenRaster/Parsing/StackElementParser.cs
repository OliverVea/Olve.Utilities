using System.Xml.Linq;
using Olve.Results;

namespace Olve.OpenRaster.Parsing;

internal static class StackElementParser
{
    public static Result ParseStackElements(IEnumerable<XElement> elements, List<Layer> layers, List<Group> groups)
    {
        var results = elements.Select(element => ParseStackElement(element, layers, groups));

        if (results.TryPickProblems(out var problems))
        {
            problems.Prepend(new ResultProblem("could not parse stack elements"));
            return problems;
        }

        return Result.Success();
    }

    public static Result ParseStackElement(XElement element, List<Layer> layers, List<Group> groups)
    {
        return element.Name.LocalName switch
        {
            "stack" => GroupReader.ReadGroupElement(element, layers, groups),
            "layer" => LayerReader.ReadLayerElement(element, layers, groups),
            _ => new ResultProblem("could not parse element with tag '{0}'", element.Value)
        };
    }
}