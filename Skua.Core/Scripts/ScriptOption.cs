using Skua.Core.PostSharp;
using Skua.Core.Interfaces;
using Skua.Core.Models.Servers;

namespace Skua.Core.Scripts;
public class ScriptOption : ScriptableObject, IScriptOption
{
    public bool AttackWithoutTarget { get; set; }
    public bool AcceptACDrops { get; set; }
    public bool RestPackets { get; set; }
    public bool SafeTimings { get; set; } = true;
    public bool ExitCombatBeforeQuest { get; set; }
    [CallBinding("skipCutscenes", UseValue = false, Get = false)]
    public bool SkipCutscenes { get; set; }
    public bool PrivateRooms { get; set; }
    [CallBinding("magnetize", UseValue = false, Get = false)]
    public bool Magnetise { get; set; }
    [CallBinding("killLag", Get = false)]
    public bool LagKiller { get; set; }
    [ObjectBinding("stage.frameRate", Get = false)]
    public int SetFPS { get; set; } = 30;
    public bool AggroMonsters { get; set; }
    public bool AggroAllMonsters { get; set; }
    [CallBinding("infiniteRange", UseValue = false, Get = false)]
    public bool InfiniteRange { get; set; }
    [ModuleBinding("DisableFX")]
    public bool DisableFX { get; set; }
    public bool AutoRelogin { get; set; }
    public bool AutoReloginAny { get; set; }
    public bool RetryRelogin { get; set; } = true;
    public bool SafeRelogin { get; set; }
    [ModuleBinding("DisableCollisions")]
    public bool DisableCollisions { get; set; }
    [CallBinding("disableDeathAd", Get = false)]
    public bool DisableDeathAds { get; set; }
    [ModuleBinding("HidePlayers")]
    public bool HidePlayers { get; set; }
    public Server LoginServer { get; set; } = null!;
    [ObjectBinding("world.myAvatar.objData.strUsername", "world.rootClass.ui.mcPortrait.strName.text", "world.myAvatar.pMC.pname.ti.text", Get = false)]
    public string CustomName { get; set; } = string.Empty;
    [ObjectBinding("world.myAvatar.pMC.pname.ti.textColor", Get = false)]
    public int NameColor { get; set; }
    [ObjectBinding("world.myAvatar.pMC.pname.tg.text", Get = false)]
    public string CustomGuild { get; set; } = string.Empty;
    [ObjectBinding("world.myAvatar.pMC.pname.tg.textColor", Get = false)]
    public int GuildColor { get; set; }
    [ObjectBinding("world.WALKSPEED", Get = false)]
    public int WalkSpeed { get; set; } = 8;
    public int LoadTimeout { get; set; } = 16000;
    public int HuntDelay { get; set; } = 1000;
    public int HuntBuffer { get; set; } = 1;
    public int MaximumTries { get; set; } = 10;
    public int ActionDelay { get; set; } = 700;
    public int PrivateNumber { get; set; } = 0;
    public int JoinMapTries { get; set; } = 3;
    public int QuestAcceptAndCompleteTries { get; set; } = 30;
    public int ReloginTries { get; set; } = 3;
    public int ReloginTryDelay { get; set; } = 3000;
}
