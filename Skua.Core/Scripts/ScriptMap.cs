using Newtonsoft.Json;
using Skua.Core.Flash;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Items;
using Skua.Core.Models.Players;
using Skua.Core.Utils;
using System.Diagnostics;

namespace Skua.Core.Scripts;

public partial class ScriptMap : IScriptMap
{
    public ScriptMap(
        Lazy<IFlashUtil> flash,
        Lazy<IScriptPlayer> player,
        Lazy<IScriptOption> options,
        Lazy<IScriptSend> send,
        Lazy<IScriptWait> wait,
        Lazy<IScriptManager> manager,
        IDialogService dialogService)
    {
        _lazyFlash = flash;
        _lazyPlayer = player;
        _lazyOptions = options;
        _lazySend = send;
        _lazyWait = wait;
        _lazyManager = manager;
        _dialogService = dialogService;
        LoadSavedMapItems();
    }

    private Dictionary<string, List<MapItem>> _savedMapItems = new();

    private readonly Lazy<IFlashUtil> _lazyFlash;
    private readonly Lazy<IScriptPlayer> _lazyPlayer;
    private readonly Lazy<IScriptOption> _lazyOptions;
    private readonly Lazy<IScriptWait> _lazyWait;
    private readonly Lazy<IScriptManager> _lazyManager;
    private readonly IDialogService _dialogService;
    private readonly Lazy<IScriptSend> _lazySend;

    private IFlashUtil Flash => _lazyFlash.Value;
    private IScriptPlayer Player => _lazyPlayer.Value;
    private IScriptOption Options => _lazyOptions.Value;
    private IScriptWait Wait => _lazyWait.Value;
    private IScriptManager Manager => _lazyManager.Value;
    private IScriptSend Send => _lazySend.Value;

    public string LastMap { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileName => string.IsNullOrEmpty(FilePath) ? string.Empty : FilePath.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries).Last();
    public string FlaName => string.IsNullOrEmpty(FileName) ? string.Empty : Path.GetFileNameWithoutExtension(FileName).Replace("-", "_") + "_fla";

    public string FullName => Loaded ? Flash.GetGameObject("ui.mcInterface.areaList.title.t1.text")?.Split(' ').Last().Replace("\"", string.Empty) ?? string.Empty : string.Empty;

    [ObjectBinding("world.strMapName", RequireNotNull = "world", Default = "string.Empty")]
    private string _name = string.Empty;

    [ObjectBinding("world.curRoom")]
    private int _roomID;

    [ObjectBinding("world.areaUsers.length")]
    private int _playerCount;

    [ObjectBinding("world.areaUsers", Default = "new()")]
    private List<string> _playerNames = new();

    [ObjectBinding("world.uoTree", Default = "new()")]
    private readonly Dictionary<string, PlayerInfo> _playersDictionary = new();

    public List<PlayerInfo> Players => _playersDictionary.Values.ToList();
    public List<PlayerInfo> CellPlayers => Players.FindAll(p => p.Cell == Player.Cell);

    public bool Loaded => !Flash.GetGameObject<bool>("world.mapLoadInProgress")
                          && Flash.IsNull("mcConnDetail.stage");

    [ObjectBinding("world.map.currentScene.labels", Select = "name", Default = "new()")]
    private List<string> _cells;

    [MethodCallBinding("jumpCorrectRoom", RunMethodPost = true)]
    private void _jump(string cell, string pad, bool autoCorrect = true, bool clientOnly = false)
    {
        Thread.Sleep(Options.ActionDelay);
        if (Options.SafeTimings)
            Wait.ForCellChange(cell);
    }

    public void Join(string map, string cell = "Enter", string pad = "Spawn", bool ignoreCheck = false, bool autoCorrect = true)
    {
        _Join(map, cell, pad, ignoreCheck, autoCorrect);
    }

    private void _Join(string map, string cell = "Enter", string pad = "Spawn", bool ignoreCheck = false, bool autoCorrect = true)
    {
        string mapName = map.Split('-')[0];
        LastMap = mapName;
        if (!Player.Playing || !Player.Loaded || (!ignoreCheck && Name == map))
            return;
        int i = 0;
        while (Name != mapName && !Manager.ShouldExit && ++i < Options.JoinMapTries)
        {
            if ((Options.PrivateRooms && !map.Contains('-')) || map.Contains("-1e9"))
                map = $"{mapName}{(Options.PrivateNumber != -1 ? Options.PrivateNumber : "-100000")}";
            if (Options.SafeTimings)
                Wait.ForActionCooldown(GameActions.Transfer);
            JoinPacket(map, cell, pad);
            if (Options.SafeTimings)
            {
                if (!Wait.ForMapLoad(map, 20) && !Manager.ShouldExit)
                    Jump(Player.Cell, Player.Pad, autoCorrect);
                else
                    Jump(cell, pad, autoCorrect);
                Thread.Sleep(Options.ActionDelay);
            }
        }
    }

    public void JoinPacket(string map, string cell = "Enter", string pad = "Spawn")
    {
        Send.Packet($"%xt%zm%cmd%{RoomID}%tfer%{Player.Username}%{map}%{cell}%{pad}%");
    }

    public PlayerInfo? GetPlayer(string username)
    {
        return Flash.GetGameObject<PlayerInfo>($"world.uoTree[\"{username.ToLower()}\"]");
    }

    [MethodCallBinding("world.reloadCurrentMap", GameFunction = true)]
    private void _reload()
    { }

    [MethodCallBinding("world.getMapItem", RunMethodPre = true, GameFunction = true)]
    private void _getMapItem(int id)
    {
        if (Options.SafeTimings)
        {
            Wait.ForActionCooldown(Skua.Core.Models.GameActions.GetMapItem);
            Thread.Sleep(Options.ActionDelay);
        }
    }

    private Dictionary<string, List<MapItem>>? LoadSavedMapItems()
    {
        if (!File.Exists(_savedCacheFilePath))
            return null;

        return _savedMapItems = JsonConvert.DeserializeObject<Dictionary<string, List<MapItem>>>(File.ReadAllText(_savedCacheFilePath))!;
    }

    private readonly string _cachePath = Path.Combine(ClientFileSources.SkuaDIR, "cache");
    private readonly string _savedCacheFilePath = Path.Combine(ClientFileSources.SkuaDIR, "cache", "0SavedMaps.json");

    public List<MapItem>? FindMapItems()
    {
        if (string.IsNullOrEmpty(FilePath))
            return null;

        if (!Directory.Exists(_cachePath))
            Directory.CreateDirectory(_cachePath);

        if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "FFDec")))
        {
            _dialogService.ShowMessageBox("FFDec folder not found.", "FFDec");
            return null;
        }

        if (_savedMapItems.ContainsKey(FileName))
            return _savedMapItems[FileName];

        List<string> files = new();
        files = Directory.GetFiles(_cachePath).ToList();
        var sw = Stopwatch.StartNew();
        if (files.Count > 0 && files.Contains(Path.Combine(_cachePath, FileName)))
            return !DecompileSWF(FileName) ? null : ParseMapSWFData();
        return !DownloadMapSWF(FileName) ? null : !DecompileSWF(FileName) ? null : ParseMapSWFData();

        void SaveMapItemInfo(List<MapItem> info)
        {
            _savedMapItems.Add(FileName, info);
            File.WriteAllText(_savedCacheFilePath, JsonConvert.SerializeObject(_savedMapItems, Formatting.Indented));
        }

        List<MapItem>? ParseMapSWFData()
        {
            sw.Restart();
            List<MapItem> items = new();
            List<string> MainTimelineText = new();
            string[] files = Array.Empty<string>();

            try
            {
                var scriptsPath = $"{_cachePath}\\tmp\\scripts";
                var flaDirectory = Directory.Exists($"{Path.Combine(scriptsPath, FlaName)}") ? Path.Combine(scriptsPath, FlaName) : Path.Combine(scriptsPath, "town_fla");

                if (!Directory.Exists(flaDirectory))
                    return null;
                var mainTimelinePath = Path.Combine(flaDirectory, "MainTimeline.as");

                if (!File.Exists(mainTimelinePath))
                    return null;

                MainTimelineText = File.ReadAllLines(mainTimelinePath).ToList();
                files = Directory.GetFiles($"{_cachePath}\\tmp\\scripts", "*APOP*", SearchOption.TopDirectoryOnly) ?? Array.Empty<string>();

                var mapItemLines = MainTimelineText.Select((l, i) => new Tuple<string, int>(l, i)).Where(l => l.Item1.Contains("mapitem", StringComparison.OrdinalIgnoreCase) || l.Item1.Contains("itemdrop", StringComparison.OrdinalIgnoreCase));
                foreach ((string mapItemLine, int index) in mapItemLines)
                {
                    var questID = string.Empty;
                    var mapItem = string.Empty;
                    switch (mapItemLine.Contains("getmapitem"))
                    {
                        case true:
                            questID = MainTimelineText.Skip(index - 5 < 0 ? 0 : index - 5).Take(10).Where(l => l.Contains("isquestinprogress")).FirstOrDefault()!;

                            if (questID == null)
                                questID = string.Empty;
                            else
                                questID = questID.ToLower().Split("isquestinprogress")[1].Split(')')[0].RemoveLetters() ?? "";

                            mapItem = mapItemLine.RemoveLetters();
                            break;

                        case false:
                            questID = MainTimelineText.Skip(index - 5 < 0 ? 0 : index - 5).Take(10).Where(l => l.Contains("questnum") || (l.Contains("intquest") && !l.Contains("intquestval"))).FirstOrDefault()!;

                            if (questID == null)
                                questID = string.Empty;
                            else
                                questID = questID.Split('=')[1].RemoveLetters() ?? "";

                            mapItem = mapItemLine.Split('=')[1].RemoveLetters();
                            break;
                    }
                    if (!string.IsNullOrEmpty(questID))
                        AddMapItem(int.Parse(mapItem), int.Parse(questID), FilePath, LastMap);
                }

                List<string> linesToParse = new();
                for (int i = 0; i < files.Length; i++)
                {
                    bool take = false;
                    foreach (string line in File.ReadLines(files[i]).Reverse())
                    {
                        if (!take && !line.Contains("getmapitem"))
                            continue;
                        if (take && line.Contains("isquestinprogress"))
                        {
                            linesToParse.Add(line.Trim().ToLower());
                            take = false;
                            continue;
                        }
                        if (line.Contains("getmapitem"))
                        {
                            linesToParse.Add(line.Trim().ToLower());
                            take = true;
                            continue;
                        }
                    }
                }

                if (linesToParse.Count > 0)
                {
                    foreach ((string mapItem, string questID) in linesToParse.PairUp())
                    {
                        if (string.IsNullOrEmpty(mapItem) || string.IsNullOrEmpty(questID))
                            continue;
                        AddMapItem(int.Parse(mapItem.RemoveLetters()), int.Parse(questID.Split("isquestinprogress")[1].Split(')')[0].RemoveLetters()), FilePath, LastMap);
                    }
                }
                Directory.Delete($"{_cachePath}\\tmp\\", true);

                void AddMapItem(int mapitem, int questid, string mapfilepath, string mapname)
                {
                    if (!items.Contains(i => i.ID == mapitem))
                        items.Add(new MapItem(mapitem, questid, mapfilepath, mapname));
                }
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException || ex is DirectoryNotFoundException)
                    _dialogService.ShowMessageBox("Could not find one or more files to read.", "Get Map Item");
                else if (ex is PathTooLongException)
                    _dialogService.ShowMessageBox($"The path for the file is too long.\r\n{_cachePath}\\tmp\\scripts\\*_fla\\MainTimeline.as", "Get Map Item");
                else if (ex is UnauthorizedAccessException)
                    _dialogService.ShowMessageBox("The program don't have permission to access the file", "Get Map Item");
                else
                    _dialogService.ShowMessageBox($"An error ocurred.\r\nMessage: {ex.Message}\r\nStackTrace:{ex.StackTrace}", "Get Map Item");
            }
            if (items.Count > 0)
            {
                items = items.OrderBy(i => i.ID).ToList();
                SaveMapItemInfo(items);
            }
            Trace.WriteLine($"Parsing took {sw.Elapsed:s\\.ff}s");
            return items;
        }

        bool DownloadMapSWF(string fileName)
        {
            sw.Restart();
            Task.Run(async () =>
            {
                byte[] fileBytes = await HttpClients.GetMapClient.GetByteArrayAsync($"https://game.aq.com/game/gamefiles/maps/{FilePath}");
                await File.WriteAllBytesAsync(Path.Combine(_cachePath, fileName), fileBytes);
            }).Wait();
            Trace.WriteLine($"Download of \"{fileName}\" took {sw.Elapsed:s\\.ff}s");
            return File.Exists($"{_cachePath}\\{fileName}");
        }

        bool DecompileSWF(string fileName)
        {
            sw.Restart();
            fileName = fileName.EndsWith(".swf") ? fileName : fileName + ".swf";
            var decompile = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    FileName = "powershell.exe",
                    WorkingDirectory = Path.Combine(AppContext.BaseDirectory, "FFDec"),
                    ArgumentList = {
                        "/c",
                        "./ffdec.bat",
                        "-export",
                        "script",
                        @$"""""""{_cachePath}\tmp""""""",
                        @$"""""""{_cachePath}\{fileName}"""""""
                    }
                }
            };
            decompile.Start();
            string error = decompile.StandardError.ReadToEnd();
            decompile.WaitForExit();
            if (!string.IsNullOrEmpty(error))
            {
                string errorMsg = $"Error while decompiling the SWF: {error}";
                Trace.WriteLine(errorMsg);
                _dialogService.ShowMessageBox(errorMsg, "SWF Decompile Error");
            }
            else
                Trace.WriteLine($"Decompilation of \"{fileName}\" took {sw.Elapsed:s\\.ff}s");
            return Directory.Exists($"{_cachePath}\\tmp");
        }
    }
}