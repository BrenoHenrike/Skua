using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Interfaces.Auras;
using Skua.Core.Models.Auras;

namespace Skua.Core.Scripts.Auras;

public partial class ScriptTargetAuras : IScriptTargetAuras
{
    private readonly Lazy<IFlashUtil> _lazyFlash;
    private IFlashUtil Flash => _lazyFlash.Value;

    public ScriptTargetAuras(Lazy<IFlashUtil> lazyFlash)
    {
        _lazyFlash = lazyFlash;
    }

    public List<Aura> Auras
    {
        get
        {
            var auraData = Flash.Call("getSubjectAuras", SubjectType.Target.ToString());
            return JsonConvert.DeserializeObject<List<Aura>>(auraData) ?? new List<Aura>();
        }
    }

    public Aura? GetAura(string auraName)
    {
        return Auras.FirstOrDefault(a => a.Name.Equals(auraName, StringComparison.OrdinalIgnoreCase));
    }

    public bool HasActiveAura(string auraName)
    {
        return GetAura(auraName) != null;
    }

    public bool TryGetAura(string auraName, out Aura? aura)
    {
        if (HasActiveAura(auraName))
        {
            aura = GetAura(auraName);
            return true;
        }
        aura = null;
        return false;
    }

    public int GetAuraValue(string auraName)
    {
        return Flash.Call<int>("GetAurasValue", SubjectType.Target.ToString(), auraName);
    }

    public bool HasAuraWithMinStacks(string auraName, int minStacks)
    {
        return GetAuraValue(auraName) >= minStacks;
    }

    public int GetAuraSecondsRemaining(string auraName)
    {
        return Flash.Call<int>("GetAuraSecondsRemaining", SubjectType.Target.ToString(), auraName);
    }

    public bool HasAnyActiveAura(params string[] auraNames)
    {
        return Flash.Call<bool>("HasAnyActiveAura", SubjectType.Target.ToString(), string.Join(",", auraNames));
    }

    public bool HasAllActiveAuras(params string[] auraNames)
    {
        return Flash.Call<bool>("HasAllActiveAuras", SubjectType.Target.ToString(), string.Join(",", auraNames));
    }

    public int GetTotalAuraStacks(string auraNamePattern)
    {
        return Flash.Call<int>("GetTotalAuraStacks", SubjectType.Target.ToString(), auraNamePattern);
    }
}