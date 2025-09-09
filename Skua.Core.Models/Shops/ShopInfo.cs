namespace Skua.Core.Models.Shops;

public class ShopInfo
{
    public int ID { get; set; }
    public string Name { get; set; }
    public List<ShopItem> Items { get; set; }

    public ShopInfo(int id, string name, List<ShopItem> items)
    {
        ID = id;
        Name = name;
        Items = items;
    }

    public override string ToString()
    {
        return $"{Name} [{ID}]";
    }

    public override bool Equals(object? obj)
    {
        return obj is ShopInfo info && info.ID == ID;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ID, Name);
    }
}