using Microsoft.CodeAnalysis;

namespace Olve.SG.CopyProperties.Models;

public class PropertyAccessModifierModel(Accessibility accessibility, string verb)
{
    public Accessibility Accessibility { get; } = accessibility;
    public string Verb { get; } = verb;
}