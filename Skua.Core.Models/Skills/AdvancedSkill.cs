using Newtonsoft.Json;

namespace Skua.Core.Models.Skills;

public class AdvancedSkill
{
    public AdvancedSkill() { }
    public AdvancedSkill(string className, string skills, int skillTimeout = -1, string useMode = "Base", string skillMode = "UseIfAvailable")
    {
        ClassName = className;
        Skills = skills;
        SkillTimeout = skillTimeout;
        ClassUseMode = (ClassUseMode)Enum.Parse(typeof(ClassUseMode), useMode);
        SkillUseMode = (SkillMode)Enum.Parse(typeof(SkillMode), skillMode);
    }
    [JsonProperty("ClassName")]
    public string ClassName { get; set; } = "Generic";
    [JsonProperty("Skills")]
    public string Skills { get; set; } = "1 | 2 | 3 | 4";
    [JsonProperty("Timeout")]
    public int SkillTimeout { get; set; } = -1;
    [JsonProperty("ClassUseMode")]
    public ClassUseMode ClassUseMode { get; set; } = ClassUseMode.Base;
    [JsonProperty("SkillUseMode")]
    public SkillMode SkillUseMode { get; set; } = SkillMode.UseIfAvailable;
    public override string ToString()
    {
        return $"{ClassUseMode} : {ClassName} => {Skills}";
    }
}
