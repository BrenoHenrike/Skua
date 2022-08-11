using System.Diagnostics;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Models;

namespace Skua.Core.Scripts;
public partial class ScriptManager : ObservableObject, IScriptManager
{
    public ScriptManager(
        ILogService logger,
        Lazy<IScriptInterface> scriptInterface,
        Lazy<IScriptHandlers> handlers,
        Lazy<IScriptSkill> skills,
        Lazy<IScriptDrop> drops,
        Lazy<IScriptWait> wait)
    {
        _lazyBot = scriptInterface;
        _lazyHandlers = handlers;
        _lazySkills = skills;
        _lazyDrops = drops;
        _lazyWait = wait;
        _logger = logger;
    }

    private readonly Lazy<IScriptInterface> _lazyBot;
    private readonly Lazy<IScriptHandlers> _lazyHandlers;
    private readonly Lazy<IScriptSkill> _lazySkills;
    private readonly Lazy<IScriptDrop> _lazyDrops;
    private readonly Lazy<IScriptWait> _lazyWait;
    private readonly ILogService _logger;
    private readonly Dictionary<string, bool> _configured = new();
    private IScriptHandlers Handlers => _lazyHandlers.Value;
    private IScriptSkill Skills => _lazySkills.Value;
    private IScriptDrop Drops => _lazyDrops.Value;
    private IScriptWait Wait => _lazyWait.Value;
    private Thread? _currentScriptThread;
    public CancellationTokenSource? ScriptCTS { get; private set; }
    
    [ObservableProperty]
    private bool _scriptRunning = false;
    [ObservableProperty]
    private string _loadedScript = string.Empty;
    [ObservableProperty]
    private string _compiledScript = string.Empty;
    public IScriptOptionContainer? Config { get; set; }

    public bool ShouldExit => ScriptCTS?.IsCancellationRequested ?? false;

    private bool _stoppedByScript;
    private bool _runScriptStoppingBool;
    private readonly List<string> _refCache = new();
    public async Task<Exception?> StartScriptAsync()
    {
        if (ScriptRunning)
        {
            _logger.ScriptLog("Script already running.");
            return new Exception("Script already running.");
        }

        try
        {
            await _lazyBot.Value.Auto.StopAsync();

            object? script = Compile(File.ReadAllText(LoadedScript));
            LoadScriptConfig(script);
            if (_configured.TryGetValue(Config!.Storage, out bool b) && !b)
                Config.Configure();
            Handlers.Clear();
            _currentScriptThread = new(async () =>
            {
                Exception? exception = null;
                ScriptCTS = new();
                try
                {
                    script?.GetType().GetMethod("ScriptMain")?.Invoke(script, new object[] { _lazyBot.Value });
                }
                catch (Exception e)
                {
                    if (e is not TargetInvocationException || !_stoppedByScript)
                    {
                        exception = e;
                        Trace.WriteLine($"Error while running script:\r\nMessage: {e.Message}\r\n{(e.InnerException is not null ? $"Inner Exception Message: {e.InnerException.Message}" : string.Empty)}StackTrace: {e.StackTrace}");
                        StrongReferenceMessenger.Default.Send<ScriptErrorMessage, int>(new(e), (int)MessageChannels.ScriptStatus);
                    }
                }
                finally
                {
                    _stoppedByScript = false;
                    if (_runScriptStoppingBool)
                    {
                        StrongReferenceMessenger.Default.Send<ScriptStoppingMessage, int>((int)MessageChannels.ScriptStatus);
                        try
                        {
                            switch (await Task.Run(async () => await StrongReferenceMessenger.Default.Send<ScriptStoppingRequestMessage, int>(new(exception), (int)MessageChannels.ScriptStatus)))
                            {
                                case true:
                                    Trace.WriteLine("Script finished succesfully.");
                                    break;
                                case false:
                                    Trace.WriteLine("Script finished early or with errors.");
                                    break;
                                default:
                                    break;
                            }
                        }
                        catch { }
                    }
                    Skills.Stop();
                    Drops.Stop();
                    ScriptCTS.Dispose();
                    ScriptCTS = null;
                    StrongReferenceMessenger.Default.Send<ScriptStoppedMessage, int>((int)MessageChannels.ScriptStatus);
                    ScriptRunning = false;
                }
            });
            _currentScriptThread.Name = "Script Thread";
            _currentScriptThread.Start();
            ScriptRunning = true;
            StrongReferenceMessenger.Default.Send<ScriptStartedMessage, int>((int)MessageChannels.ScriptStatus);
            return null;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task RestartScriptAsync()
    {
        Trace.WriteLine("Restarting script");
        await StopScriptAsync(false);
        await Task.Run(async () =>
        {
            await Task.Delay(5000);
            await StartScriptAsync();
        });
    }

    public void RestartScript()
    {
        Trace.WriteLine("Restarting script");
        StopScript(false);
        Task.Run(async () =>
        {
            await Task.Delay(5000);
            await StartScriptAsync();
        });
    }

    public void StopScript(bool runScriptStoppingEvent = true)
    {
        _runScriptStoppingBool = runScriptStoppingEvent;
        _stoppedByScript = true;
        ScriptCTS?.Cancel();
        if(Thread.CurrentThread.Name == "Script Thread")
        {
            ScriptCTS?.Token.ThrowIfCancellationRequested();
            return;
        }
        Wait.ForTrue(() => ScriptCTS == null, 20);
        OnPropertyChanged(nameof(ScriptRunning));
    }

    public async ValueTask StopScriptAsync(bool runScriptStoppingEvent = true)
    {
        _runScriptStoppingBool = runScriptStoppingEvent;
        _stoppedByScript = true;
        ScriptCTS?.Cancel();
        if (Thread.CurrentThread.Name == "Script Thread")
        {
            ScriptCTS?.Token.ThrowIfCancellationRequested();
            return;
        }
        await Wait.ForTrueAsync(() => ScriptCTS == null, 30);
        OnPropertyChanged(nameof(ScriptRunning));
    }

    public object? Compile(string source)
    {
        Stopwatch sw = new();
        sw.Start();

        List<string> references = new();
        if (_refCache.Count == 0 && Directory.Exists("plugins"))
            _refCache.AddRange(Directory.GetFiles("plugins", "*.dll").Select(x => Path.Combine(AppContext.BaseDirectory, x)).Where(CanLoadAssembly));
        _refCache.ForEach(x => references.Add(x));
        string toRemove = "";
        List<string> sources = new() { source };
        foreach (string line in source.Split('\n').Select(l => l.Trim()))
        {
            if (line.StartsWith("using"))
                break;
            if (line.StartsWith("//cs_"))
            {
                string[] parts = line.Split((char[])null!, 2, StringSplitOptions.RemoveEmptyEntries);
                string cmd = parts[0].Substring(5);
                switch (cmd)
                {
                    case "ref":
                        string local = Path.Combine(AppContext.BaseDirectory, parts[1]);
                        if (File.Exists(local))
                            references.Add(local);
                        else if (File.Exists(parts[1]))
                            references.Add(parts[1]);
                        break;
                    case "include":
                        string localSource = Path.Combine(AppContext.BaseDirectory, parts[1]);
                        if (File.Exists(localSource))
                            sources.Add($"// Added from {localSource}\n{File.ReadAllText(localSource)}");
                        else if (File.Exists(parts[1]))
                            sources.Add($"// Added from {parts[1]}\n{File.ReadAllText(parts[1])}");
                        break;
                }
                toRemove = $"{toRemove}{line}{Environment.NewLine}";
            }
        }

        if (sources.Count > 1)
        {
            sources[0] = sources[0].Replace(toRemove, "");
            string joinedSource = string.Join(Environment.NewLine, sources);
            List<string> lines = joinedSource.Split('\n').Select(l => l.Trim()).ToList();
            List<string> usings = new();
            for (int i = lines.Count - 1; i >= 0; i--)
            {
                if (lines[i].StartsWith("using") && lines[i].Split(' ').Length == 2)
                {
                    usings.Add(lines[i]);
                    lines.RemoveAt(i);
                }
            }
            lines.Insert(0, $"{string.Join(Environment.NewLine, usings.Distinct())}{Environment.NewLine}#nullable enable{Environment.NewLine}");
            sources = lines;
        }

        string final = string.Join('\n', sources);
        SyntaxTree tree = CSharpSyntaxTree.ParseText(final, encoding: Encoding.UTF8);
        final = tree.GetRoot().NormalizeWhitespace().ToFullString();
        Compiler compiler = Ioc.Default.GetRequiredService<Compiler>();
        if (references.Count > 0)
            compiler.AddAssemblies(references.ToArray());

        dynamic? assembly = compiler.CompileClass(final);
        sw.Stop();
        Trace.WriteLine($"Script compilation took {sw.ElapsedMilliseconds}ms.");
        File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "Scripts", "z_CompiledScript.cs"), final);
        if (compiler.Error)
            throw new ScriptCompileException(compiler.ErrorMessage, compiler.GeneratedClassCodeWithLineNumbers);

        return assembly;
    }
    public void LoadScriptConfig(object? script)
    {
        if (script is null)
            return;

        IScriptOptionContainer opts = Config = Ioc.Default.GetRequiredService<IScriptOptionContainer>();
        Type t = script.GetType();
        FieldInfo? storageField = t.GetField("OptionsStorage");
        FieldInfo? optsField = t.GetField("Options");
        FieldInfo? multiOptsField = t.GetField("MultiOptions");
        FieldInfo? dontPreconfField = t.GetField("DontPreconfigure");
        if (multiOptsField is not null)
        {
            List<FieldInfo> multiOpts = new();
            foreach (string optField in (string[])multiOptsField.GetValue(script))
            {
                FieldInfo? fi = t.GetField(optField);
                if (fi is not null)
                    multiOpts.Add(fi);
            }
            foreach (FieldInfo opt in multiOpts)
            {
                List<IOption> parsedOpt = (List<IOption>)opt.GetValue(script)!;
                parsedOpt.ForEach(o => o.Category = opt.Name.Replace('_', ' '));
                opts.MultipleOptions.Add(opt.Name, parsedOpt);
            }
        }
        if (optsField is not null)
            opts.Options.AddRange((List<IOption>)optsField.GetValue(script)!);
        if (storageField is not null)
            opts.Storage = (string)storageField.GetValue(script)!;
        if (dontPreconfField is not null)
            _configured[opts.Storage] = (bool)dontPreconfField.GetValue(script)!;
        else if (optsField is not null)
            _configured[opts.Storage] = false;
        opts.SetDefaults();
        opts.Load();
    }

    private static bool CanLoadAssembly(string path)
    {
        try
        {
            AssemblyName.GetAssemblyName(path);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void SetLoadedScript(string path)
    {
        LoadedScript = path;
    }
}