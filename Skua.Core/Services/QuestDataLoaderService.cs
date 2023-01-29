using System.Dynamic;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Messaging;
using Skua.Core.Models.Quests;
using static System.Collections.Generic.Dictionary<int, Skua.Core.Models.Quests.Quest>;

namespace Skua.Core.Services;
public class QuestDataLoaderService : IQuestDataLoaderService
{
    public QuestDataLoaderService(IScriptQuest quests, IScriptPlayer player, IFlashUtil flash)
    {
        _quests = quests;
        _flash = flash;
        _player = player;
    }

    private readonly IScriptQuest _quests;
    private readonly IFlashUtil _flash;
    private readonly IScriptPlayer _player;
    private readonly Dictionary<string, List<QuestData>?> _cachedQuests = new();

    public async Task<List<QuestData>> GetFromFileAsync(string fileName)
    {
        fileName = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName));
        if (!File.Exists(fileName))
            return new();
        
        if (_cachedQuests.TryGetValue($"CachedQuests_{fileName}", out List<QuestData>? quests))
            return quests ?? new();
        
        string text = await File.ReadAllTextAsync(fileName);
        quests = JsonConvert.DeserializeObject<List<QuestData>>(text);
        _cachedQuests.Add($"CachedQuests_{fileName}", quests);
        return quests ?? new();
    }

    public async Task<List<QuestData>> UpdateAsync(string fileName, bool all, IProgress<string>? progress, CancellationToken token)
    {
        return await Task.Run(async () =>
        {
            if (!_player.LoggedIn)
                return _quests.Cached = await GetFromFileAsync(fileName);
            
            _quests.Cached = await GetFromFileAsync(fileName);
            AutoResetEvent wait = new(false);
            
            int start = 1;
            if (!all && (_quests.Cached.Count > 0))
                 start = _quests.Cached.Last().ID + 1;
            
            List<QuestData> quests = new();
            for (int i = start; i < 13000; i += 29)
            {
                if (token.IsCancellationRequested)
                    break;
                
                _flash.SetGameObject("world.questTree", new ExpandoObject());
                progress?.Report($"Loading Quests {i}-{i + 29}...");
                
                List<Quest> currQuests = new();
                StrongReferenceMessenger.Default.Register<QuestDataLoaderService, ExtensionPacketMessage, int>(this, (int)MessageChannels.GameEvents, packetListener);
                
                _quests.Load(Enumerable.Range(i, 29).ToArray());
                
                wait.WaitOne(10000);
                StrongReferenceMessenger.Default.Unregister<ExtensionPacketMessage, int>(this, (int)MessageChannels.GameEvents);
                if (currQuests.Count == 0)
                {
                    progress?.Report("No more quests found.");
                    break;
                }
                
                quests.AddRange(currQuests.Select(q => ConvertToQuestData(q)));
                if (!token.IsCancellationRequested)
                    await Task.Delay(1500);

                void packetListener(QuestDataLoaderService recipient, ExtensionPacketMessage message)
                {
                    if ((message.Packet["params"].type == "json") && (message.Packet["params"].dataObj.cmd == "getQuests"))
                    {
                        ValueCollection col = JsonConvert.DeserializeObject<Dictionary<int, Quest>>(JsonConvert.SerializeObject(message.Packet["params"].dataObj.quests)).Values;
                        currQuests = col.ToList();
                        wait.Set();
                    }
                }
            }
            
            quests.AddRange(_quests.Cached);
            await File.WriteAllTextAsync(fileName, JsonConvert.SerializeObject(quests.Distinct().OrderBy(q => q.ID), Formatting.Indented));
            progress?.Report($"Getting quests from file {fileName}");
            
            return _quests.Cached = await GetFromFileAsync(fileName);
        });
    }

    private QuestData ConvertToQuestData(Quest q)
    {
        return new()
        {
            ID = q.ID,
            Name = q.Name,
            AcceptRequirements = q.AcceptRequirements,
            Field = q.Field,
            Gold = q.Gold,
            Index = q.Index,
            Level = q.Level,
            Once = q.Once,
            RequiredClassID = q.RequiredClassID,
            RequiredClassPoints = q.RequiredClassPoints,
            RequiredFactionId = q.RequiredFactionId,
            RequiredFactionRep = q.RequiredFactionRep,
            Requirements = q.Requirements,
            Rewards = q.Rewards,
            SimpleRewards = q.SimpleRewards,
            Slot = q.Slot,
            Upgrade = q.Upgrade,
            Value = q.Value,
            XP = q.XP
        };
    }
}
