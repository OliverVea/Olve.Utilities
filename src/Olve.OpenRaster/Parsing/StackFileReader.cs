using System.IO.Compression;
using System.Xml.Linq;
using Olve.Results;

namespace Olve.OpenRaster.Parsing;

internal static class StackFileReader
{
    public static Result<OpenRasterFile> ReadStackXml(ZipArchive zipArchive)
    {
        if (GetStackXmlRoot(zipArchive).TryPickProblems(out var problems, out var root))
        {
            return problems.Prepend(new ResultProblem("failed reading stack.xml from .ora file"));
        }

        if (root.GetAttribute("version", "0.0.0").TryPickProblems(out problems, out var version))
        {
            return problems.Prepend(new ResultProblem("failed reading version from stack.xml"));
        }

        if (root.GetIntAttribute("w").TryPickProblems(out problems, out var width))
        {
            return problems.Prepend(new ResultProblem("failed reading width from stack.xml"));
        }

        if (root.GetIntAttribute("h").TryPickProblems(out problems, out var height))
        {
            return problems.Prepend(new ResultProblem("failed reading height from stack.xml"));
        }

        if (root.GetIntAttribute("xres", 72).TryPickProblems(out problems, out var xRes))
        {
            return problems.Prepend(new ResultProblem("failed reading xres from stack.xml"));
        }

        if (root.GetIntAttribute("yres", 72).TryPickProblems(out problems, out var yRes))
        {
            return problems.Prepend(new ResultProblem("failed reading yres from stack.xml"));
        }

        var rootStack = root.Element("stack");
        if (rootStack == null)
        {
            return new ResultProblem("root stack element is missing in stack.xml");
        }

        if (ParseChildren(rootStack.Elements()).TryPickProblems(out problems, out var children))
        {
            return problems;
        }

        var stack = new OpenRasterFile
        {
            Version = version,
            Width = width,
            Height = height,
            XResolution = xRes,
            YResolution = yRes,
            Groups = children.Groups,
            Layers = children.Layers,
        };

        return stack;
    }

    private static Result<XElement> GetStackXmlRoot(ZipArchive zipArchive)
    {
        var stackFile = zipArchive.GetEntry("stack.xml");
        if (stackFile == null)
        {
            return new ResultProblem("stack.xml was not found in the .ora file");
        }

        using var stream = stackFile.Open();
        using var reader = new StreamReader(stream);
        var xml = reader.ReadToEnd();

        var document = XDocument.Parse(xml);
        var root = document.Root;
        if (root == null)
        {
            return new ResultProblem("stack.xml is empty");
        }

        return root;
    }

    private static Result<(List<Group> Groups, List<Layer> Layers)> ParseChildren(IEnumerable<XElement> children)
    {
        List<Group> groups = [];
        List<Layer> layers = [];

        var parseChildrenResult = StackElementParser.ParseStackElements(children, layers, groups);
        if (parseChildrenResult.TryPickProblems(out var problems))
        {
            return problems.Prepend(new ResultProblem("failed parsing stack in stack.xml"));
        }

        return (groups, layers);
    }
}