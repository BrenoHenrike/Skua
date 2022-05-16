using System.Drawing;
using Skua.Core.PostSharp;
using Skua.Core.Interfaces;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Skills;
using Skua.Core.Models.Items;
using Skua.Core.Models.Players;

namespace Skua.Core.Scripts;
public class ScriptPlayer : ScriptableObject, IScriptPlayer
{
    [ObjectBinding("world.myAvatar.uid")]
    public int ID { get; }
    [ObjectBinding("world.myAvatar.objData.intExp")]
    public int XP { get; }
    [ObjectBinding("world.myAvatar.objData.intExpToLevel")]
    public int RequiredXP { get; }
    [ObjectBinding("world.strFrame")]
    public string Cell { get; } = string.Empty;
    [ObjectBinding("world.strPad")]
    public string Pad { get; } = string.Empty;
    [ObjectBinding("serverIP", Static = true)]
    public string ServerIP { get; } = string.Empty;
    public bool Playing => LoggedIn && Alive;
    [CallBinding("isLoggedIn")]
    public bool LoggedIn { get; }
    [ObjectBinding("loginInfo.strUsername", Static = true)]
    public string Username { get; } = string.Empty;
    [ObjectBinding("loginInfo.strPassword", Static = true)]
    public string Password { get; } = string.Empty;
    [CallBinding("isKicked")]
    public bool Kicked { get; }
    [ObjectBinding("world.myAvatar.dataLeaf.intState", RequireNotNull = "world.myAvatar", DefaultValue = 0)]
    public int State { get; }
    public bool InCombat => State == 2;
    public bool IsMember => Bot.Flash.GetGameObject<int>("world.myAvatar.objData.iUpgDays") >= 0;
    public bool Alive => State > 0;
    [ObjectBinding("world.myAvatar.dataLeaf.intHP")]
    public int Health { get; }
    [ObjectBinding("world.myAvatar.dataLeaf.intHPMax")]
    public int MaxHealth { get; }
    [ObjectBinding("world.myAvatar.objData.intMP", "world.myAvatar.dataLeaf.intMP")]
    public int Mana { get; set; }
    [ObjectBinding("world.myAvatar.dataLeaf.intMPMax")]
    public int MaxMana { get; }
    [ObjectBinding("world.myAvatar.dataLeaf.intLevel")]
    public int Level { get; set; }
    [ObjectBinding("world.myAvatar.objData.intGold")]
    public int Gold { get; }
    [ObjectBinding("world.myAvatar.objData.iRank")]
    public int CurrentClassRank { get; }
    public bool HasTarget
    {
        get
        {
            Monster m = Target!;
            return m?.Alive ?? false;
        }
    }
    public bool Loaded => Bot.Flash.GetGameObject<int>("world.myAvatar.items.length") > 0
                        && !Bot.Flash.GetGameObject<bool>("world.mapLoadInProgress")
                        && Bot.Flash.CallGameFunction<bool>("world.myAvatar.pMC.artLoaded");
    [ObjectBinding("world.myAvatar.objData.intAccessLevel")]
    public int AccessLevel { get; set; }
    public bool Upgrade
    {
        get
        {
            return Bot.Flash.GetGameObject<int>("world.myAvatar.objData.iUpgDays") > 0;
        }
        set
        {
            Bot.Flash.SetGameObject("world.myAvatar.objData.iUpg", value ? 1000 : 0);
            Bot.Flash.SetGameObject("world.myAvatar.objData.iUpgDays", value ? 1000 : 0);
        }
    }
    [ObjectBinding("world.actions.active")]
    public SkillInfo[] Skills { get; } = null!;
    [ObjectBinding("world.myAvatar.dataLeaf.afk")]
    public bool AFK { get; }
    [ObjectBinding("world.myAvatar.pMC.x")]
    public int X { get; }
    [ObjectBinding("world.myAvatar.pMC.y")]
    public int Y { get; }
    [ObjectBinding("world.WALKSPEED")]
    public int WalkSpeed { get; set; }
    [ObjectBinding("world.SCALE")]
    public int Scale { get; set; }
    [ObjectBinding("world.myAvatar.target.objData", RequireNotNull = "world.myAvatar.target")]
    public Monster? Target { get; }
    [ObjectBinding("world.myAvatar.dataLeaf.sta")]
    public PlayerStats Stats { get; }
    public InventoryItem? CurrentClass => Playing ? Bot.Inventory.Items.Find(i => i.Equipped && i.Category == ItemCategory.Class) : null;

    [MethodCallBinding("walkTo", RunMethodPost = true)]
    public void WalkTo(int x, int y, int speed = 8)
    {
        if (Bot.Options.SafeTimings)
            Bot.Wait.ForPlayerPosition(x, y);
    }

    [MethodCallBinding("world.setSpawnPoint", GameFunction = true)]
    public void SetSpawnPoint(string cell, string pad) { }

    [MethodCallBinding("world.goto", GameFunction = true)]
    public void Goto(string name) { }
}
