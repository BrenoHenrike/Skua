using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Linq;

namespace Skua.Core.Generators.Extensions;

internal static class IncrementalGeneratorInitializationContextExtensions
{
    /// <summary>
    /// Registers an output node into an <see cref="IncrementalGeneratorInitializationContext"/> to output diagnostics.
    /// </summary>
    /// <param name="context">The input <see cref="IncrementalGeneratorInitializationContext"/> instance.</param>
    /// <param name="diagnostics">The input <see cref="IncrementalValuesProvider{TValues}"/> sequence of diagnostics.</param>
    public static void ReportDiagnostics(this IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<ImmutableArray<Diagnostic>> diagnostics)
    {
        context.RegisterSourceOutput(diagnostics, static (context, diagnostics) =>
        {
            foreach (Diagnostic diagnostic in diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        });
    }

    /// <summary>
    /// Get all fields with atleast 1 attribute.
    /// </summary>
    /// <param name="context">The input <see cref="IncrementalGeneratorInitializationContext"/> instance.</param>
    /// <returns>An <see cref="IncrementalValuesProvider{IFieldSymbol}"/> of all found fields with atleast 1 attribute.</returns>
    public static IncrementalValuesProvider<IFieldSymbol> GetFieldsWithAttributes(this IncrementalGeneratorInitializationContext context)
    {
        return context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is FieldDeclarationSyntax field && field.AttributeLists.Count > 0,
                static (context, _) => ((FieldDeclarationSyntax)context.Node).Declaration.Variables.Select(v => (IFieldSymbol)context.SemanticModel.GetDeclaredSymbol(v)!))
            .SelectMany(static (item, _) => item);
    }
}