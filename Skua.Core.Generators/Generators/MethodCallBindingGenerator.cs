using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Skua.Core.Generators.Extensions;
using Skua.Core.Generators.Models;
using System.Collections.Immutable;
using System.Linq;
using static Skua.Core.Generators.Attributes;

namespace Skua.Core.Generators;

[Generator]
public partial class MethodCallBindingGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Register the attribute source
        context.RegisterPostInitializationOutput(i => i.AddSource($"{MethodCallBindingName}Attribute.g.cs", MethodCallBindingSource));

        // Get all method declarations with at least one attribute
        IncrementalValuesProvider<(IMethodSymbol Symbol, MethodDeclarationSyntax Declaration)> methodSymbols =
            context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is MethodDeclarationSyntax method && method.AttributeLists.Count > 0,
                static (context, _) => ((IMethodSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node)!, (MethodDeclarationSyntax)context.Node)!);

        // Filter the methods using [MethodCallBinding]
        IncrementalValuesProvider<(IMethodSymbol Symbol, MethodDeclarationSyntax Declaration)> methodSymbolsWithAttribute =
            methodSymbols
            .Where(static item => item.Symbol.HasAttributeWithFullyQualifiedName(MethodCallBindingFullName));

        // Gather info for all annotated methods
        IncrementalValuesProvider<(ClassInfo ClassInfo, Result<MethodCallBindingPropertyInfo?> Info)> methodInfoWithErrors =
            methodSymbolsWithAttribute
            .Select(static (item, _) =>
            {
                ClassInfo classInfo = item.Symbol.GetClassInfo();
                MethodCallBindingPropertyInfo? methodInfo = Execute.TryGetInfo(item.Symbol, item.Declaration, out ImmutableArray<Diagnostic> diagnostics);

                return (classInfo, new Result<MethodCallBindingPropertyInfo?>(methodInfo, diagnostics));
            });

        // Output the diagnostics
        context.ReportDiagnostics(methodInfoWithErrors.Select(static (item, _) => item.Info.Errors));

        // Get the filtered sequence to enable caching
        IncrementalValuesProvider<(ClassInfo ClassInfo, MethodCallBindingPropertyInfo Info)> methodInfo =
            methodInfoWithErrors
            .Where(static item => item.Info.Value is not null)
            .Select(static (item, _) => (item.ClassInfo, item.Info.Value!))
            .WithComparers(ClassInfo.Comparer.Default, MethodCallBindingPropertyInfo.Comparer.Default);

        // Split and group by containing type
        IncrementalValuesProvider<(ClassInfo ClassInfo, ImmutableArray<MethodCallBindingPropertyInfo> Methods)> groupedMethodInfo =
            methodInfo
            .GroupBy(ClassInfo.Comparer.Default)
            .WithComparers(ClassInfo.Comparer.Default, MethodCallBindingPropertyInfo.Comparer.Default.ForImmutableArray());

        // Generate the methods
        context.RegisterSourceOutput(groupedMethodInfo, static (context, item) =>
        {
            Execute.GenerateClass(item.ClassInfo, item.Methods, MethodCallBindingName, context);
        });
    }
}