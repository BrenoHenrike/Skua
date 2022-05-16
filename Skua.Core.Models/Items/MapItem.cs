namespace Skua.Core.Models.Items;

public record struct MapItem(int MapItemID, int QuestID, string MapFilePath, string MapName)
{
    public override string ToString()
    {
        return $"Item ID [{MapItemID}], Quest [{QuestID}]";
    }
}
