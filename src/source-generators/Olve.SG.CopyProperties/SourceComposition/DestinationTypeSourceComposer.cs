using System.Text;
using Microsoft.CodeAnalysis;
using Olve.SG.CopyProperties.Models;

namespace Olve.SG.CopyProperties.SourceComposition;

public static class DestinationTypeSourceComposer
{
    public static string GenerateSource(GeneratedTypeModel model)
    {
        var sb = new StringBuilder();

        // Collect all unique namespaces
        var namespaces = model
            .Properties
            .SelectMany(p => p.Namespaces)
            .Distinct()
            .OrderBy(ns => ns);

        // Add using statements
        foreach (var ns in namespaces)
        {
            sb.AppendLine($"using {ns};");
        }

        sb.AppendLine();
        sb.AppendLine($"namespace {model.Namespace};");
        sb.AppendLine();
        sb.AppendLine($"partial {model.TypeType} {model.TypeName}");
        sb.AppendLine("{");

        foreach (var property in model.Properties)
        {
            // Format XML comments into triple-slash style
            if (!string.IsNullOrWhiteSpace(property.XmlComment))
            {
                var formattedComment = FormatXmlComment(property.XmlComment!);
                sb.Append(formattedComment);
            }

            // Add attributes
            foreach (var attribute in property.Attributes)
            {
                sb.AppendLine($"    {attribute}");
            }

            // Generate property
            sb.Append($"    public {property.Type} {property.Name} {{");

            if (property.AccessModifiers.Get is { } get)
            {
                if (get.Accessibility != property.PropertyAccessibility)
                {
                    sb.Append($"{AccessibilityToString(get.Accessibility)} ");
                }

                sb.Append($"{get.Verb};");
            }

            if (property.AccessModifiers.Set is { } set)
            {
                if (set.Accessibility != property.PropertyAccessibility)
                {
                    sb.Append($"{AccessibilityToString(set.Accessibility)} ");
                }

                sb.Append($"{set.Verb};");
            }

            sb.AppendLine("}");
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    private static string AccessibilityToString(Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Protected => "protected",
            Accessibility.Private => "private",
            Accessibility.Internal => "internal",
            Accessibility.ProtectedOrInternal => "protected internal",
            Accessibility.ProtectedAndInternal => "private protected",
            _ => "internal"
        };
    }


    private static string FormatXmlComment(string rawXml)
    {
        var sb = new StringBuilder();

        // Split the raw XML comment into lines and trim each line
        var lines = rawXml.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            // Exclude the outer <member> tags
            if (line.Contains("<member") || line.Contains("</member>"))
            {
                continue;
            }

            // Add the triple-slash and indent correctly
            sb.AppendLine($"    /// {line.Trim()}");
        }

        return sb.ToString();
    }
}