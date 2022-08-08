using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection;
using System.Text;
using Westwind.Scripting;

namespace Skua.Core;
/// <summary>
/// Slightly modified compiler based on Westwind.Scripting (https://github.com/RickStrahl/Westwind.Scripting)
/// </summary>
public class Compiler : CSharpScriptExecution
{
    /// <summary>
    /// This method compiles a class and hands back a
    /// dynamic reference to that class that you can
    /// call members on.
    ///
    /// Must have include parameterless ctor()
    /// </summary>
    /// <param name="code">Fully self-contained C# class</param>
    /// <returns>Instance of that class or null</returns>
    public new dynamic? CompileClass(string code)
    {
        var type = CompileClassToType(code);
        if (type == null)
            return null;

        // Figure out the class name
        GeneratedClassName = type.Name;
        GeneratedNamespace = type.Namespace;

        return CreateInstance();
    }

    /// <summary>
    /// This method compiles a class and hands back a
    /// dynamic reference to that class that you can
    /// call members on.
    /// </summary>
    /// <param name="code">Fully self-contained C# class</param>
    /// <returns>Instance of that class or null</returns>
    public new Type? CompileClassToType(string code)
    {
        int hash = code.GetHashCode();

        GeneratedClassCode = code;

        if (!CachedAssemblies.ContainsKey(hash))
        {
            if (!CompileAssembly(code))
                return null;

            CachedAssemblies[hash] = Assembly;
        }
        else
        {
            Assembly = CachedAssemblies[hash];
        }

        // Figure out the class name
        return Assembly.ExportedTypes.First();
    }

    /// <summary>
    /// Compiles a class and creates an assembly from the compiled class.
    ///
    /// Assembly is stored on the `.Assembly` property. Use `noLoad()`
    /// to bypass loading of the assembly
    ///
    /// Must include parameterless ctor()
    /// </summary>
    /// <param name="source">Source code</param>
    /// <param name="noLoad">if set doesn't load the assembly (useful only when OutputAssembly is set)</param>
    /// <returns></returns>
    public new bool CompileAssembly(string source, bool noLoad = false)
    {
        ClearErrors();

        var tree = SyntaxFactory.ParseSyntaxTree(source.Trim());

        var compilation = CSharpCompilation.Create(GeneratedClassName + ".cs")
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                        optimizationLevel: OptimizationLevel.Release))
            .WithReferences(References)
            .AddSyntaxTrees(tree);

        if (SaveGeneratedCode)
            GeneratedClassCode = tree.ToString();

        bool isFileAssembly = false;
        Stream? codeStream = null;
        if (string.IsNullOrEmpty(OutputAssembly))
        {
            codeStream = new MemoryStream(); // in-memory assembly
        }
        else
        {
            codeStream = new FileStream(OutputAssembly, FileMode.Create, FileAccess.Write);
            isFileAssembly = true;
        }

        using (codeStream)
        {
            EmitResult? compilationResult = null;
            if (CompileWithDebug)
            {
                var debugOptions = CompileWithDebug ? DebugInformationFormat.Embedded : DebugInformationFormat.Pdb;
                compilationResult = compilation.Emit(codeStream, options: new EmitOptions(debugInformationFormat: debugOptions));
            }
            else
                compilationResult = compilation.Emit(codeStream);

            // Compilation Error handling
            if (!compilationResult.Success)
            {
                var sb = new StringBuilder();
                foreach (var diag in 
                    compilationResult.Diagnostics
                        .Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error))
                {
                    sb.AppendLine(diag.ToString());
                }

                ErrorType = ExecutionErrorTypes.Compilation;
                ErrorMessage = sb.ToString();
                SetErrors(new ApplicationException(ErrorMessage));
                return false;
            }

            if (!noLoad)
            {
                if (!isFileAssembly)
                    Assembly = Assembly.Load(((MemoryStream)codeStream).ToArray());
                else
                    Assembly = Assembly.LoadFrom(OutputAssembly);
            }
        }

        return true;
    }

    private void ClearErrors()
    {
        LastException = null;
        Error = false;
        ErrorMessage = null;
        ErrorType = ExecutionErrorTypes.None;
    }

    private void SetErrors(Exception ex)
    {
        Error = true;
        LastException = ex.GetBaseException();
        ErrorMessage = LastException.Message;

        if (ThrowExceptions)
            throw LastException;
    }
}
