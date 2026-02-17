using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml.Linq;
using Olve.Results;

namespace Olve.OpenRaster.Parsing;

internal static class XmlElementParser
{
    public static bool TryGetAttribute(this XElement element, string attributeName, [NotNullWhen(true)] out string? attributeValue)
    {
        var attribute = element.Attribute(attributeName);
        if (attribute is null)
        {
            attributeValue = null;
            return false;
        }

        attributeValue = attribute.Value;
        return true;
    }

    public static Result<string> GetAttribute(this XElement element, string attributeName)
    {
        var attribute = element.Attribute(attributeName);
        if (attribute is null)
        {
            return new ResultProblem("element '{0}' does not have expected attribute '{1}'", element.Name,
                attributeName);
        }

        return attribute.Value;
    }

    public static Result<string> GetAttribute(this XElement element, string attributeName, string defaultValue)
    {
        var attribute = element.Attribute(attributeName);
        if (attribute is null)
        {
            return defaultValue;
        }

        return attribute.Value;
    }

    public static Result<int> GetIntAttribute(this XElement element, string attributeName)
    {
        if (GetAttribute(element, attributeName).TryPickProblems(out var problems, out var attributeString))
        {
            return problems;
        }

        if (!int.TryParse(attributeString, CultureInfo.InvariantCulture, out var attributeValue))
        {
            return new ResultProblem("attribute '{0}' in element '{1}' with value '{2}' is not a valid integer", attributeName,  element.Name, attributeString);
        }

        return attributeValue;
    }

    public static Result<int> GetIntAttribute(this XElement element, string attributeName, int defaultValue)
    {
        if (!element.TryGetAttribute(attributeName, out var attributeString))
        {
            return defaultValue;
        }

        if (!int.TryParse(attributeString, CultureInfo.InvariantCulture, out var attributeValue))
        {
            return new ResultProblem("attribute '{0}' in element '{1}' with value '{2}' is not a valid integer", attributeName,  element.Name, attributeString);
        }

        return attributeValue;
    }

    public static Result<float> GetFloatAttribute(this XElement element, string attributeName)
    {
        if (GetAttribute(element, attributeName).TryPickProblems(out var problems, out var attributeString))
        {
            return problems;
        }

        if (!float.TryParse(attributeString, CultureInfo.InvariantCulture, out var attributeValue))
        {
            return new ResultProblem("attribute '{0}' in element '{1}' with value '{2}' is not a valid decimal number", attributeName,  element.Name, attributeString);
        }

        return attributeValue;
    }

    public static Result<float> GetFloatAttribute(this XElement element, string attributeName, float defaultValue)
    {
        if (!element.TryGetAttribute(attributeName, out var attributeString))
        {
            return defaultValue;
        }

        if (!float.TryParse(attributeString, CultureInfo.InvariantCulture, out var attributeValue))
        {
            return new ResultProblem("attribute '{0}' in element '{1}' with value '{2}' is not a valid decimal number", attributeName,  element.Name, attributeString);
        }

        return attributeValue;
    }

    public static Result<Visibility> GetVisibilityAttribute(this XElement element, string attributeName = "visibility")
    {
        if (GetAttribute(element, attributeName).TryPickProblems(out var problems, out var attributeString))
        {
            return problems;
        }

        return attributeString switch
        {
            "visible" => Visibility.Visible,
            "hidden" => Visibility.Hidden,
            _ => new ResultProblem("element '{0}' has invalid visibility value '{1}'", element.Name, attributeString)
        };
    }

    public static Result<Visibility> GetVisibilityAttribute(this XElement element, Visibility defaultValue, string attributeName = "visibility")
    {
        if (!element.TryGetAttribute(attributeName, out var attributeString))
        {
            return defaultValue;
        }

        return attributeString switch
        {
            "visible" => Visibility.Visible,
            "hidden" => Visibility.Hidden,
            _ => new ResultProblem("element '{0}' has invalid visibility value '{1}'", element.Name, attributeString)
        };
    }
}