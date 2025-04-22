using Microsoft.CodeAnalysis;
using Olve.SG.CopyProperties.Models;

namespace Olve.SG.CopyProperties.Helpers;

public static class PropertyAccessModifiersExtractionHelper
{
    public static PropertyAccessModifiersModel GetAccessModifiers(IPropertySymbol propertySymbol)
    {
        var get = propertySymbol.GetMethod is { } getMethod ? GetAccessModifier(getMethod) : null;
        var set = propertySymbol.SetMethod is { } setMethod ? GetAccessModifier(setMethod) : null;

        return new PropertyAccessModifiersModel(get, set);
    }

    private static PropertyAccessModifierModel GetAccessModifier(IMethodSymbol methodSymbol)
    {
        var isSet = methodSymbol.MethodKind == MethodKind.PropertySet;
        var isInit = isSet && methodSymbol.IsInitOnly;

        var verb =
            isInit ? "init"
            : isSet ? "set"
            : "get";
        var accessibility = methodSymbol.DeclaredAccessibility;

        return new PropertyAccessModifierModel(accessibility, verb);
    }
}
