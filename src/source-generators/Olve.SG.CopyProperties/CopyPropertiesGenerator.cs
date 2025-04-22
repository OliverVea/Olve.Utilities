using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Olve.SG.CopyProperties.Helpers;
using Olve.SG.CopyProperties.Models;
using Olve.SG.CopyProperties.SourceComposition;

namespace Olve.SG.CopyProperties;

[Generator]
public class CopyPropertiesGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
            ctx.AddSource(
                CopyPropertiesAttributeHelper.GeneratedFileName,
                SourceText.From(CopyPropertiesAttributeHelper.SourceCode, Encoding.UTF8)
            )
        );

        var toGenerate = context.SyntaxProvider.ForAttributeWithMetadataName(
            CopyPropertiesAttributeHelper.FullyQualifiedName,
            static (_, _) => true,
            static (ctx, _) => GetClassOrStructToGenerate(ctx)
        );

        context.RegisterSourceOutput(
            toGenerate,
            static (spc, source) =>
            {
                var (model, message, success) = source;

                if (model is null || !success)
                {
                    spc.ReportDiagnostic(
                        Diagnostic.Create(
                            new DiagnosticDescriptor(
                                "CP002",
                                "CopyProperties",
                                message ?? "No model was generated.",
                                "CopyProperties",
                                DiagnosticSeverity.Error,
                                true
                            ),
                            Location.None
                        )
                    );
                    return;
                }

                Execute(model, spc);

                spc.ReportDiagnostic(
                    Diagnostic.Create(
                        new DiagnosticDescriptor(
                            "CP003",
                            "CopyProperties",
                            $"Generated {model.TypeType} {model.TypeName}.",
                            "CopyProperties",
                            DiagnosticSeverity.Info,
                            true
                        ),
                        Location.None
                    )
                );
            }
        );
    }

    private static (GeneratedTypeModel? Model, string Log, bool Success) GetClassOrStructToGenerate(
        GeneratorAttributeSyntaxContext context
    )
    {
        var sb = new StringBuilder();

        var (semanticModel, syntaxNode) = (context.SemanticModel, context.TargetNode);

        if (syntaxNode is not TypeDeclarationSyntax destinationDeclaration)
        {
            return (null, "The attribute must be applied to a class or struct.", false);
        }

        var destinationSymbol = semanticModel.GetDeclaredSymbol(destinationDeclaration);
        if (destinationSymbol is null)
        {
            return (null, "Could not find the symbol for the destination type.", false);
        }

        var allAttributes = destinationDeclaration
            .AttributeLists.SelectMany(al => al.Attributes)
            .ToArray();

        sb.Append(
            $"Found {allAttributes.Length} attributes with names: {string.Join(", ", allAttributes.Select(a => a.Name.ToString()))}\t"
        );

        var copyPropertyAttributes = allAttributes
            .Where(a => a.Name.ToString() == CopyPropertiesAttributeHelper.AttributeName)
            .ToArray();

        sb.Append($"Found {copyPropertyAttributes.Length} CopyProperties attributes.\t");

        var typeOfExpressions = copyPropertyAttributes
            .Select(a => a.ArgumentList?.Arguments.FirstOrDefault()?.Expression)
            .OfType<TypeOfExpressionSyntax>()
            .ToArray();

        sb.Append($"Found {typeOfExpressions.Length} type of expressions.\t");

        var sourceSymbols = typeOfExpressions
            .Select(x => ModelExtensions.GetTypeInfo(semanticModel, x.Type))
            .Where(x => x.Type is INamedTypeSymbol)
            .Select(x => x.Type as INamedTypeSymbol)
            .OfType<INamedTypeSymbol>()
            .ToArray();

        sb.Append($"Found {sourceSymbols.Length} source symbols.\t");

        var model = GeneratedTypeExtractionHelper.ExtractGeneratedType(
            sourceSymbols,
            destinationSymbol,
            destinationDeclaration
        );

        return (model, sb.ToString(), sourceSymbols.Length > 0);
    }

    private static void Execute(GeneratedTypeModel source, SourceProductionContext spc)
    {
        var sourceCode = DestinationTypeSourceComposer.GenerateSource(source);

        spc.AddSource($"{source.TypeName}.g.cs", SourceText.From(sourceCode, Encoding.UTF8));

        spc.ReportDiagnostic(
            Diagnostic.Create(
                new DiagnosticDescriptor(
                    "CP001",
                    "CopyProperties",
                    $"Generated {source.TypeType} {source.TypeName}.",
                    "CopyProperties",
                    DiagnosticSeverity.Info,
                    true
                ),
                Location.None
            )
        );
    }
}
