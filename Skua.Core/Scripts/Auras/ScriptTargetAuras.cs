using Skua.Core.Models.Auras;
using Skua.Core.Flash;
using Skua.Core.Interfaces.Auras;
using Skua.Core.Interfaces;
using Newtonsoft.Json;

namespace Skua.Core.Scripts.Auras;
public partial class ScriptTargetAuras : IScriptTargetAuras
{
    private readonly Lazy<IFlashUtil> _lazyFlash;
    private IFlashUtil Flash => _lazyFlash.Value;

    public ScriptTargetAuras(Lazy<IFlashUtil> lazyFlash)
    {
        _lazyFlash = lazyFlash;
    }

    public List<Aura>? Auras
    {
        get
        {
            return JsonConvert.DeserializeObject<List<Aura>>(Flash.Call("getSubjectAuras", SubjectType.Target.ToString())) ?? new();
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
}
