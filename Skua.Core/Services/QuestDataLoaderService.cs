using System.Dynamic;
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Models.Quests;
using static System.Collections.Generic.Dictionary<int, Skua.Core.Models.Quests.Quest>;

namespace Skua.Core.Services;
public class QuestDataLoaderService : IQuestDataLoaderService
{
    // TODO Cache
    private readonly IScriptQuest Quests;
    private readonly IFlashUtil Flash;
    private readonly IScriptPlayer Player;
    private readonly IScriptEvent Events;

    public QuestDataLoaderService(IScriptQuest quests, IScriptPlayer player, IFlashUtil flash, IScriptEvent events)
    {
        Quests = quests;
        Flash = flash;
        Player = player;
        Events = events;
    }

    public async Task<List<QuestData>> GetFromFileAsync(string fileName)
    {
        if (!File.Exists(fileName))
            return new();
        string text = await File.ReadAllTextAsync(fileName);
        List<QuestData>? quests = JsonConvert.DeserializeObject<List<QuestData>>(text);

        return quests is null || quests.Count == 0 ? new() : quests;
    }

    public async Task<List<QuestData>> UpdateAsync(string fileName, bool all, IProgress<string>? progress, CancellationToken token)
    {
        return await Task.Run(async () =>
        {
            if (!Player.LoggedIn)
                return Quests.Cached = await GetFromFileAsync(fileName);
            Quests.Cached = await GetFromFileAsync(fileName);
            AutoResetEvent wait = new(false);
            int start = all ? 1 : Quests.Cached.Count > 0 ? Quests.Cached.Last().ID + 1 : 1;
            List<QuestData> quests = new();
            for (int i = start; i < 13000; i += 29)
            {
                if (token.IsCancellationRequested)
                    break;
                Flash.SetGameObject("world.questTree", new ExpandoObject());
                progress?.Report($"Loading Quests {i}-{i + 29}...");
                Quests.Load(Enumerable.Range(i, 29).ToArray());
                List<Quest> currQuests = new();
                void packetListener(dynamic packet)
                {
                    if (packet["params"].type == "json" && packet["params"].dataObj.cmd == "getQuests")
                    {
                        ValueCollection col = JsonConvert.DeserializeObject<Dictionary<int, Quest>>(JsonConvert.SerializeObject(packet["params"].dataObj.quests)).Values;
                        currQuests = col.ToList();
                        wait.Set();
                    }
                }
                Events.ExtensionPacketReceived += packetListener;
                wait.WaitOne(5000);
                Events.ExtensionPacketReceived -= packetListener;
                if (currQuests.Count == 0)
                {
                    progress?.Report("No more quests found.");
                    break;
                }
                quests.AddRange(currQuests.Select(q => ConvertToQuestData(q)));
                if (!token.IsCancellationRequested)
                    await Task.Delay(1500);
            }
            quests.AddRange(Quests.Cached);
            await File.WriteAllTextAsync(fileName, JsonConvert.SerializeObject(quests.Distinct().OrderBy(q => q.ID), Formatting.Indented));
            progress?.Report($"Getting quests from file {fileName}");
            return Quests.Cached = await GetFromFileAsync(fileName);
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
