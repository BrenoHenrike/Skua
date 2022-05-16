using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
