using Skua.Core.Models.Quests;

namespace Skua.Core.Interfaces;

public interface IQuestDataLoaderService
{
    Task<List<QuestData>> GetFromFileAsync(string fileName);

    Task<List<QuestData>> UpdateAsync(string fileName, bool all, IProgress<string>? progress, CancellationToken token);
}