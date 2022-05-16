using System.Diagnostics;
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Items;
using Skua.Core.Models.Players;
using Skua.Core.PostSharp;
using Skua.Core.Utils;

namespace Skua.Core.Scripts;
public class ScriptMap : ScriptableObject, IScriptMap
{
    Dictionary<string, List<MapItem>> SavedMapItems = new();

    public ScriptMap()
    {
        LoadSavedMapItems();
    }
    public string LastMap { get; set; } = default!;
    public string FilePath { get; set; } = default!;
    public string FileName => string.IsNullOrEmpty(FilePath) ? "" : FilePath.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries).Last();
    public string Name => _name.ToLower();
    public string FullName => Loaded ? Bot.Flash.GetGameObject("ui.mcInterface.areaList.title.t1.text")?.Split(' ').Last() ?? "" : "";

    [ObjectBinding("world.strMapName", RequireNotNull = "world")]
    private string _name { get; } = string.Empty;
    [ObjectBinding("world.curRoom")]
    public int RoomID { get; }
    [ObjectBinding("world.areaUsers.length")]
    public int PlayerCount { get; }
    [ObjectBinding("world.areaUsers")]
    public List<string> PlayerNames { get; } = new();
    [ObjectBinding("world.uoTree")]
    private readonly Dictionary<string, PlayerInfo> _players = new();
    public List<PlayerInfo> Players => _players.Values.ToList();
    public List<PlayerInfo> CellPlayers => Players.FindAll(p => p.Cell == Bot.Player.Cell);
    public bool Loaded => !Bot.Flash.GetGameObject<bool>("world.mapLoadInProgress")
                        && Bot.Flash.IsNull("mcConnDetail.stage");
    [ObjectBinding("world.map.currentScene.labels", Select = "name")]
    public List<string> Cells { get; } = new();

    [MethodCallBinding("world.moveToCell", RunMethodPost = true, GameFunction = true)]
    public void Jump(string cell, string pad, bool clientOnly = false)
    {
        if (Bot.Options.SafeTimings)
            Bot.Wait.ForCellChange(cell);
    }

    public void Join(string map, string cell = "Enter", string pad = "Spawn", bool ignoreCheck = false)
    {
        _Join(map, cell, pad, ignoreCheck);
    }

    private void _Join(string map, string cell = "Enter", string pad = "Spawn", bool ignoreCheck = false)
    {
        LastMap = map;
        if (!Bot.Player.Playing || !Bot.Player.Loaded || (!ignoreCheck && Name == map))
            return;
        int i = 0;
        while (Name != map && !Bot.ShouldExit && ++i < Bot.Options.JoinMapTries)
        {
            if (Bot.Options.PrivateRooms || map.Contains("-1e9"))
                map = $"{map.Split('-')[0]}{(Bot.Options.PrivateNumber != -1 ? Bot.Options.PrivateNumber : "-100000")}";
            if (Bot.Options.SafeTimings)
                Bot.Wait.ForActionCooldown(GameActions.Transfer);
            JoinPacket(map, cell, pad);
            if (Bot.Options.SafeTimings)
            {
                if (!Bot.Wait.ForMapLoad(map, 20) && !Bot.ShouldExit)
                    Jump(Bot.Player.Cell, Bot.Player.Pad);
                else
                    Jump(cell, pad);
                Bot.Sleep(Bot.Options.ActionDelay);
            }
        }
    }

    public void JoinPacket(string map, string cell = "Enter", string pad = "Spawn")
    {
        Bot.Send.Packet($"%xt%zm%cmd%{RoomID}%tfer%{Bot.Player.Username}%{map}%{cell}%{pad}%");
    }

    public PlayerInfo? GetPlayer(string username)
    {
        return Bot.Flash.GetGameObject<PlayerInfo>($"world.uoTree[\"{username.ToLower()}\"]");
    }

    [MethodCallBinding("world.reloadCurrentMap", GameFunction = true)]
    public void Reload() { }

    [MethodCallBinding("world.getMapItem", RunMethodPre = true, GameFunction = true)]
    public void GetMapItem(int id)
    {
        if (Bot.Options.SafeTimings)
            Bot.Wait.ForActionCooldown(GameActions.GetMapItem);
    }

    private Dictionary<string, List<MapItem>>? LoadSavedMapItems()
    {
        if (!File.Exists(savedCacheFilePath))
            return null;

        return SavedMapItems = JsonConvert.DeserializeObject<Dictionary<string, List<MapItem>>>(File.ReadAllText(savedCacheFilePath))!;
    }

    private readonly string cachePath = Path.Combine(AppContext.BaseDirectory, "tools\\cache");
    private readonly string savedCacheFilePath = Path.Combine(AppContext.BaseDirectory, "tools\\cache", "0SavedMaps.json");
    public List<MapItem>? FindMapItems()
    {
        if (string.IsNullOrEmpty(FilePath))
            return null;

        if (!Directory.Exists(cachePath))
            Directory.CreateDirectory(cachePath);

        if (SavedMapItems.ContainsKey(FileName))
            return SavedMapItems[FileName];
        List<string> files = new();
        files = Directory.GetFiles(cachePath).ToList();
        var sw = Stopwatch.StartNew();
        if (files.Count > 0 && files.Contains(Path.Combine(cachePath, FileName)))
            return !DecompileSWF(FileName) ? null : ParseMapSWFData();

        return !DownloadMapSWF(FileName) ? null : !DecompileSWF(FileName) ? null : ParseMapSWFData();

        void SaveMapItemInfo(List<MapItem> info)
        {
            SavedMapItems.Add(FileName, info);
            File.WriteAllText(savedCacheFilePath, JsonConvert.SerializeObject(SavedMapItems, Formatting.Indented));
        }

        List<MapItem>? ParseMapSWFData()
        {
            if (!Directory.Exists($"{cachePath}\\tmp\\scripts\\town_fla"))
                return null;
            sw.Restart();
            List<MapItem> items = new();
            List<string> MainTimelineText = new();
            string[] files = Array.Empty<string>();
            try
            {
                MainTimelineText = File.ReadAllLines($"{cachePath}\\tmp\\scripts\\town_fla\\MainTimeline.as").ToList();
                files = Directory.GetFiles($"{cachePath}\\tmp\\scripts", "*APOP*", SearchOption.TopDirectoryOnly) ?? Array.Empty<string>();

                var mapItemLines = MainTimelineText.Select((l, i) => new Tuple<string, int>(l, i)).Where(l => l.Item1.Contains("mapitem", StringComparison.OrdinalIgnoreCase) || l.Item1.Contains("itemdrop", StringComparison.OrdinalIgnoreCase));
                foreach ((string mapItemLine, int index) in mapItemLines)
                {
                    string questID, mapItem;
                    switch (mapItemLine.Contains("getmapitem"))
                    {
                        case true:
                            questID = MainTimelineText.Skip(index - 5 < 0 ? 0 : index - 5).Take(10).Where(l => l.Contains("isquestinprogress")).First().ToLower().Split("isquestinprogress")[1].Split(')')[0].RemoveLetters() ?? "";
                            mapItem = mapItemLine.RemoveLetters();
                            break;
                        case false:
                            questID = MainTimelineText.Skip(index - 5 < 0 ? 0 : index - 5).Take(10).Where(l => l.Contains("questnum") || (l.Contains("intquest") && !l.Contains("intquestval"))).First().Split('=')[1].RemoveLetters() ?? "";
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
                Directory.Delete($"{cachePath}\\tmp\\", true);

                void AddMapItem(int mapitem, int questid, string mapfilepath, string mapname)
                {
                    if (!items.Contains(i => i.MapItemID == mapitem))
                        items.Add(new MapItem(mapitem, questid, mapfilepath, mapname));
                }
            }
            catch (Exception ex)
            {
                //if (ex is FileNotFoundException || ex is DirectoryNotFoundException)
                //    ControlUtils.ShowErrorMessage("Could not find one or more files to read.", "Get Map Item");
                //else if (ex is PathTooLongException)
                //    ControlUtils.ShowErrorMessage($"The path for the file is too long.\r\n{cachePath}\\tmp\\scripts\\town_fla\\MainTimeline.as", "Get Map Item");
                //else if (ex is UnauthorizedAccessException)
                //    ControlUtils.ShowErrorMessage("The program don't have permission to access the file", "Get Map Item");
                //else
                //    ControlUtils.ShowErrorMessage($"An error ocurred.\r\nMessage: {ex.Message}\r\nStackTrace:{ex.StackTrace}", "Get Map Item");
            }
            if (items.Count > 0)
            {
                items = items.OrderBy(i => i.MapItemID).ToList();
                SaveMapItemInfo(items);
            }
            Debug.WriteLine($"Parsing took {sw.Elapsed:s\\.ff}s");
            return items;
        }

        bool DownloadMapSWF(string fileName)
        {
            sw.Restart();
            Task.Run(async () =>
            {
                byte[] fileBytes = await HttpClients.GitHubClient.GetByteArrayAsync($"https://game.aq.com/game/gamefiles/maps/{Bot.Map.FilePath}");
                await File.WriteAllBytesAsync(Path.Combine(cachePath, fileName), fileBytes);
            }).Wait();
            Debug.WriteLine($"Download of \"{fileName}\" took {sw.Elapsed:s\\.ff}s");
            return File.Exists($"{cachePath}\\{fileName}");
        }

        bool DecompileSWF(string fileName)
        {
            sw.Restart();
            var decompile = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    FileName = "powershell.exe",
                    WorkingDirectory = Path.Combine(AppContext.BaseDirectory, "tools\\ffdec"),
                    Arguments = $"/c ./ffdec.bat -export script \"{cachePath}\\tmp\" \"{cachePath}\\{fileName}\""
                }
            };
            decompile.Start();
            string error = decompile.StandardError.ReadToEnd();
            decompile.WaitForExit();
            if (!string.IsNullOrEmpty(error))
                Debug.WriteLine($"Error while decompiling the SWF: {error}");
            else
                Debug.WriteLine($"Decompilation of \"{fileName}\" took {sw.Elapsed:s\\.ff}s");
            return Directory.Exists($"{cachePath}\\tmp");
        }
    }
}
