using Skua.Core.Interfaces;
using Skua.Core.Models.Servers;
using Skua.Core.Flash;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Immutable;
using Skua.Core.Models;

namespace Skua.Core.Scripts;
public partial class ScriptOption : ObservableObject, IScriptOption
{
    public ScriptOption(Lazy<IFlashUtil> flash)
    {
        _lazyFlash = flash;
        OptionsDictionary = GenerateDictionary().ToImmutableDictionary();
    }
    private readonly Lazy<IFlashUtil> _lazyFlash;
    private IFlashUtil Flash => _lazyFlash.Value;

    public void ResetOptions()
    {
        SafeTimings = true;
    }

    public ImmutableDictionary<string, Func<object>> OptionsDictionary { get; }
    [ObservableProperty]
    private bool _AttackWithoutTarget;
    [ObservableProperty]
    private bool _AcceptACDrops;
    [ObservableProperty]
    private bool _RestPackets;
    [ObservableProperty]
    private bool _SafeTimings = true;
    [ObservableProperty]
    private bool _ExitCombatBeforeQuest;
    [CallBinding("skipCutscenes", UseValue = false, Get = false, HasSetter = true)]
    private bool _skipCutscenes;
    public bool PrivateRooms { get; set; }
    [CallBinding("magnetize", UseValue = false, Get = false, HasSetter = true)]
    private bool _Magnetise;
    [CallBinding("killLag", Get = false, HasSetter = true)]
    private bool _LagKiller;
    [ObjectBinding("stage.frameRate", Get = false, HasSetter = true)]
    private int _SetFPS = 30;
    [ObservableProperty]
    private bool _AggroMonsters;
    [ObservableProperty]
    private bool _AggroAllMonsters;
    [CallBinding("infiniteRange", UseValue = false, Get = false, HasSetter = true)]
    private bool _InfiniteRange;
    [ModuleBinding("DisableFX")]
    private bool _DisableFX;
    [ObservableProperty]
    private bool _AutoRelogin;
    [ObservableProperty]
    private bool _AutoReloginAny;
    [ObservableProperty]
    private bool _RetryRelogin = true;
    [ObservableProperty]
    private bool _SafeRelogin;
    [ModuleBinding("DisableCollisions")]
    private bool _DisableCollisions;
    [CallBinding("disableDeathAd", Get = false, HasSetter = true)]
    private bool _DisableDeathAds;
    [ModuleBinding("HidePlayers")]
    private bool _HidePlayers;
    [ObservableProperty]
    private Server? _LoginServer = null;
    [ObjectBinding("world.myAvatar.objData.strUsername", "world.rootClass.ui.mcPortrait.strName.text", "world.myAvatar.pMC.pname.ti.text", Get = false, HasSetter = true, Default = "string.Empty")]
    private string _CustomName = string.Empty;
    [ObjectBinding("world.myAvatar.pMC.pname.ti.textColor", Get = false, HasSetter = true)]
    private int _NameColor;
    [ObjectBinding("world.myAvatar.pMC.pname.tg.text", Get = false, HasSetter = true, Default = "string.Empty")]
    private string _CustomGuild = string.Empty;
    [ObjectBinding("world.myAvatar.pMC.pname.tg.textColor", Get = false, HasSetter = true)]
    public int _GuildColor;
    [ObjectBinding("world.WALKSPEED", Get = false, HasSetter = true, Default = "8")]
    private int _WalkSpeed = 8;
    [ObservableProperty]
    private int _LoadTimeout = 16000;
    [ObservableProperty]
    private int _HuntDelay = 1000;
    [ObservableProperty]
    private int _HuntBuffer = 1;
    [ObservableProperty]
    private int _MaximumTries = 10;
    [ObservableProperty]
    private int _ActionDelay = 700;
    [ObservableProperty]
    private int _PrivateNumber = 0;
    [ObservableProperty]
    private int _JoinMapTries = 3;
    [ObservableProperty]
    private int _QuestAcceptAndCompleteTries = 30;
    [ObservableProperty]
    private int _ReloginTries = 3;
    [ObservableProperty]
    private int _ReloginTryDelay = 3000;
    [ObservableProperty]
    private HuntPriorities _HuntPriority = HuntPriorities.None;

    private Dictionary<string, Func<object>> GenerateDictionary()
    {
        Dictionary<string, Func<object>> dict = new Dictionary<string, Func<object>>();
        foreach (PropertyInfo pi in GetType().GetProperties())
        {
            if (pi.Name == nameof(OptionsDictionary))
                continue;
            var methodCall = Expression.Call(Expression.Constant(this), pi.GetGetMethod()!, null);
            var convertedExpression = Expression.Convert(methodCall, typeof(object));
            Func<object> function = Expression.Lambda<Func<object>>(convertedExpression).Compile();
            dict.Add(pi.Name, function);
        }
        return dict;
    }
}
