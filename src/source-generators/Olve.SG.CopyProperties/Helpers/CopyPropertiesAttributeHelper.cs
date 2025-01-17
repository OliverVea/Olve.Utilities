namespace Olve.SG.CopyProperties.Helpers;

public static class CopyPropertiesAttributeHelper
{
    public const string Namespace = "Olve.SG.CopyProperties";
    public const string AttributeName = "CopyProperties";
    public const string ClassName = AttributeName + "Attribute";
    public const string GeneratedFileName = ClassName + ".g.cs";
    public const string FullyQualifiedName = Namespace + "." + ClassName;

    public const string SourceCode =
        $$"""
          namespace {{Namespace}};

          [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false, AllowMultiple = true)]
          public class {{ClassName}} : Attribute
          {
             public {{ClassName}}(Type source)
             {
                 
             }
          }
          """;
}