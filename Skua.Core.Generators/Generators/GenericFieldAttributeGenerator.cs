using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Skua.Core.Generators.Extensions;
using Skua.Core.Generators.Models;

namespace Skua.Core.Generators;
public abstract partial class GenericFieldAttributeGenerator<TInfo> : IIncrementalGenerator
{
    /// <summary>
    /// The fully qualified name of the attribute type to look for.
    /// </summary>
    protected readonly string attributeFullName;

    /// <summary>
    /// The name of the attribute without namespace and "Attribute" used for file naming.
    /// </summary>
    protected readonly string attributeName;

    /// <summary>
    /// The attribute code to add in the generation.
    /// </summary>
    protected readonly string attributeSource;

    /// <summary>
    /// An <see cref="IEqualityComparer{T}"/> instance to compare intermediate models.
    /// </summary>
    /// <remarks>
    /// This is needed to cache extracted info on attributes used to annotate target types.
    /// </remarks>
    private readonly IEqualityComparer<TInfo> comparer;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericFieldAttributeGenerator{TInfo}"/>
    /// </summary>
    /// <param name="attributeFullName">The fully qualified name of the attribute type to look for.</param>
    /// <param name="attributeName">The name of the attribute without namespace and "Attribute" used for file naming.</param>
    /// <param name="attributeSource">The attribute code to add in the generation.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> instance to compare intermediate models.</param>
    private protected GenericFieldAttributeGenerator(string attributeFullName, string attributeName, string attributeSource, IEqualityComparer<TInfo> comparer)
    {
        this.attributeFullName = attributeFullName;
        this.attributeName = attributeName;
        this.attributeSource = attributeSource;
        this.comparer = comparer;
    }

    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Register the attribute source
        context.RegisterPostInitializationOutput(i => i.AddSource($"{attributeName}Attribute.g.cs", attributeSource));

        // Get all field declarations with at least one attribute
        IncrementalValuesProvider<IFieldSymbol> fieldSymbols =
            context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is FieldDeclarationSyntax field && field.AttributeLists.Count > 0,
                static (context, _) => ((FieldDeclarationSyntax)context.Node).Declaration.Variables.Select(v => (IFieldSymbol)context.SemanticModel.GetDeclaredSymbol(v)!))
            .SelectMany(static (item, _) => item);

        // Filter the fields using the attribute type
        IncrementalValuesProvider<IFieldSymbol> fieldSymbolsWithAttribute =
            fieldSymbols
            .Where(item => item.HasAttributeWithFullyQualifiedName(attributeFullName));

        // Gather info for all annotated fields
        IncrementalValuesProvider<(ClassInfo ClassInfo, Result<TInfo?> Info)> propertyInfoWithErrors =
            fieldSymbolsWithAttribute
            .Select((item, _) =>
            {
                ClassInfo classInfo = item.GetClassInfo();
                TInfo? propertyInfo = TryGetInfo(item, out ImmutableArray<Diagnostic> diagnostics);

                return (classInfo, new Result<TInfo?>(propertyInfo, diagnostics));
            });

        // Output the diagnostics
        context.ReportDiagnostics(propertyInfoWithErrors.Select(static (item, _) => item.Info.Errors));

        // Get the filtered sequence to enable caching
        IncrementalValuesProvider<(ClassInfo ClassInfo, TInfo Info)> propertyInfo =
            propertyInfoWithErrors
            .Select(static (item, _) => (item.ClassInfo, Info: item.Info.Value))
            .Where(static item => item.Info is not null)!
            .WithComparers(ClassInfo.Comparer.Default, comparer);

        // Split and group by containing type
        IncrementalValuesProvider<(ClassInfo ClassInfo, ImmutableArray<TInfo> Properties)> groupedPropertyInfo =
            propertyInfo
            .GroupBy(ClassInfo.Comparer.Default)
            .WithComparers(ClassInfo.Comparer.Default, comparer.ForImmutableArray());

        // Generate the requested properties and methods
        context.RegisterSourceOutput(groupedPropertyInfo, (context, item) =>
        {
            GenerateClass(item.ClassInfo, item.Properties, attributeName, context);
        });
    }

    /// <summary>
    /// Processes the info to generate the final class.
    /// </summary>
    /// <param name="classInfo">The information about the class to generate.</param>
    /// <param name="fields">The fields to be added to the class.</param>
    /// <param name="suffix">A suffix for the name of the file.</param>
    /// <param name="context">The current context to generate the code.</param>
    internal virtual void GenerateClass(ClassInfo classInfo, ImmutableArray<TInfo> fields, string suffix, SourceProductionContext context)
    {
        StringBuilder source = new($"// <auto-generated>\r\nnamespace {classInfo.Namespace};public partial class {classInfo.Name}{(string.IsNullOrWhiteSpace(classInfo.InheritanceFormattedNames) ? "" : $" : {classInfo.InheritanceFormattedNames}")} {{");
        foreach (TInfo info in fields)
            GenerateProperties(source, info);
        source.Append("}");

        SyntaxTree tree = CSharpSyntaxTree.ParseText(source.ToString(), encoding: Encoding.UTF8);
        context.AddSource($"{classInfo.Name}_{suffix}.g.cs", tree.GetRoot().NormalizeWhitespace().ToFullString());
    }

    /// <summary>
    /// Processes a given field.
    /// </summary>
    /// <param name="fieldSymbol">The input <see cref="IFieldSymbol"/> instance to process.</param>
    /// <param name="diagnostics">The resulting diagnostics from the processing operation.</param>
    /// <returns>The resulting <see cref="TInfo"/> instance for <paramref name="fieldSymbol"/>, if successful.</returns>
    protected abstract TInfo? TryGetInfo(IFieldSymbol fieldSymbol, out ImmutableArray<Diagnostic> diagnostics);

    /// <summary>
    /// Processes the given info to generate source code.
    /// </summary>
    /// <param name="source"><see cref="StringBuilder"/> to add source code to.</param>
    /// <param name="info">The information to be processed.</param>
    protected abstract void GenerateProperties(StringBuilder source, TInfo info);

}
