using Skua.Core.Models.Auras;
using Skua.Core.Flash;
using Skua.Core.Interfaces.Auras;
using Skua.Core.Interfaces;
using Newtonsoft.Json;

namespace Skua.Core.Scripts.Auras;
public partial class ScriptSelfAuras : IScriptSelfAuras
{
    private readonly Lazy<IFlashUtil> _lazyFlash;
    private IFlashUtil Flash => _lazyFlash.Value;

    public ScriptSelfAuras(Lazy<IFlashUtil> lazyFlash)
    {
        _lazyFlash = lazyFlash;
    }

    public List<Aura>? Auras
    {
        get
        {
            return JsonConvert.DeserializeObject<List<Aura>>(Flash.Call("getSubjectAuras", SubjectType.Self.ToString())) ?? new();
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
}
