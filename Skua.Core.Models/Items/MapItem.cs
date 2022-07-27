namespace Skua.Core.Models.Items;

public record MapItem(int ID, int QuestID, string MapFilePath, string MapName)
{
    public override string ToString()
    {
        return $"ID [{ID}], Quest [{QuestID}]";
    }
}
