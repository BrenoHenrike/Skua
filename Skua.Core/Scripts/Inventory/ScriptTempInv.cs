using Skua.Core.Models.Items;
using Skua.Core.Interfaces;
using Skua.Core.PostSharp;

namespace Skua.Core.Scripts;
public class ScriptTempInv : ScriptableObject, IScriptTempInv
{
    [ObjectBinding("world.myAvatar.tempitems")]
    public List<ItemBase> Items { get; } = new();
}
