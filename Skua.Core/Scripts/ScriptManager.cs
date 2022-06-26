using System.Diagnostics;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Westwind.Scripting;

namespace Skua.Core.Scripts;
public class ScriptManager : IScriptManager
{
    public ScriptManager(CSharpScriptExecution compiler, ILogService logger, Lazy<IScriptInterface> scriptInterface,
        Lazy<IScriptHandlers> handlers, Lazy<IScriptOption> options, Lazy<IScriptEvent> events, Lazy<IScriptSkill> skills, Lazy<IScriptDrop> drops)
    {
        _lazyBot = scriptInterface;
        _lazyHandlers = handlers;
        _lazyOptions = options;
        _lazyEvents = events;
        _lazySkills = skills;
        _lazyDrops = drops;
        _logger = logger;

        _compiler = compiler;
    }

    private readonly Lazy<IScriptInterface> _lazyBot;
    private readonly Lazy<IScriptHandlers> _lazyHandlers;
    private readonly Lazy<IScriptOption> _lazyOptions;
    private readonly Lazy<IScriptEvent> _lazyEvents;
    private readonly Lazy<IScriptSkill> _lazySkills;
    private readonly Lazy<IScriptDrop> _lazyDrops;
    private readonly ILogService _logger;
    private IScriptHandlers Handlers => _lazyHandlers.Value;
    private IScriptOption Options => _lazyOptions.Value;
    private IScriptEvent Events => _lazyEvents.Value;
    private IScriptSkill Skills => _lazySkills.Value;
    private IScriptDrop Drops => _lazyDrops.Value;
    private readonly CSharpScriptExecution _compiler;
    private Thread? CurrentScriptThread;
    private CancellationTokenSource? ScriptCTS;
    public bool ScriptRunning { get; set; }
    public string LoadedScript { get; set; } = string.Empty;
    public string CompiledScript { get; set; } = string.Empty;

    public event Action? ScriptStarted;
    public event Action<bool>? ScriptStopped;
    public event Action<Exception>? ScriptError;
    public event Action<bool>? ApplicationShutDown;

    public bool ShouldExit => ScriptCTS?.IsCancellationRequested ?? false;

    private bool stoppedByScript;
    private bool runScriptStoppingBool;
    private List<string> _refCache = new();
    public async Task<Exception?> StartScriptAsync()
    {
        if (ScriptRunning)
        {
            _logger.ScriptLog("Script already running.");
            return new Exception("Script already running.");
        }

        try
        {
            // TODO Auto
            //Forms.Main.StopAuto();

            object? script = await Task.Run(() => Compile(File.ReadAllText(LoadedScript)));
            //LoadScriptConfig(script);
            //if (_configured.TryGetValue(ScriptInterface.Instance.Config.Storage, out bool b) && !b)
            //    ScriptInterface.Instance.Config.Configure();
            Handlers.Clear();
            CurrentScriptThread = new(async () =>
            {
                ScriptCTS = new();
                ScriptStarted?.Invoke();
                try
                {
                    script?.GetType().GetMethod("ScriptMain")?.Invoke(script, new object[] { _lazyBot.Value });
                }
                catch (Exception e)
                {
                    if (e is not TargetInvocationException || !stoppedByScript)
                    {
                        Debug.WriteLine($"Error while running script:\r\nMessage: {e.InnerException?.Message ?? e.Message}\r\nStackTrace: {e.InnerException?.StackTrace ?? e.StackTrace}");
                        ScriptError?.Invoke(e);
                    }
                }
                finally
                {
                    stoppedByScript = false;
                    ScriptCTS.Dispose();
                    ScriptCTS = null;
                    if (runScriptStoppingBool)
                    {
                        try
                        {
                            switch (await Events.OnScriptStoppedAsync())
                            {
                                case true:
                                    Debug.WriteLine("Script finished succesfully.");
                                    break;
                                case false:
                                    Debug.WriteLine("Script finished early or with errors.");
                                    break;
                                default:
                                    break;
                            }
                        }
                        catch { }
                    }
                    Options.AutoRelogin = false;
                    Options.LagKiller = false;
                    Options.LagKiller = true;
                    Options.LagKiller = false;
                    Options.AggroAllMonsters = false;
                    Options.AggroMonsters = false;
                    Options.SkipCutscenes = false;
                    Skills.Stop();
                    Drops.Stop();
                    Events.ClearHandlers();
                    ScriptStopped?.Invoke(true);
                }
            });
            CurrentScriptThread.Name = "Script Thread";
            CurrentScriptThread.Start();

            return null;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public Task<Exception?> RestartScriptAsync()
    {
        throw new NotImplementedException();
    }

    public void StopScript(bool runScriptStoppingEvent = true)
    {
        runScriptStoppingBool = runScriptStoppingEvent;
        stoppedByScript = true;
        ScriptCTS?.Cancel();
        if(Thread.CurrentThread.Name == "Script Thread")
            ScriptCTS?.Token.ThrowIfCancellationRequested();
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
                string[] parts = line.Split((char[])null, 2, StringSplitOptions.RemoveEmptyEntries);
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
            string joinedSource = string.Join('\n', sources);
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
            lines.Insert(0, $"{string.Join('\n', usings.Distinct())}\n#nullable enable\n");
            sources = lines;
        }
        string final = string.Join('\n', sources);
        SyntaxTree tree = CSharpSyntaxTree.ParseText(final.ToString(), encoding: Encoding.UTF8);
        final = tree.GetRoot().NormalizeWhitespace().ToFullString();
        if (references.Count > 0)
            _compiler.AddAssemblies(references.ToArray());

        dynamic assembly = _compiler.CompileClass(final);
        sw.Stop();
        Debug.WriteLine($"Script compilation took {sw.ElapsedMilliseconds}ms.");

        if (_compiler.Error)
            throw new ScriptCompileException(_compiler.ErrorMessage, _compiler.GeneratedClassCodeWithLineNumbers);

        return assembly;
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
}