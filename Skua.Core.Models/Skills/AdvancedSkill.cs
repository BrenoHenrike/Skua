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
        SkillUseMode = (SkillUseMode)Enum.Parse(typeof(SkillUseMode), skillMode);
    }
    public AdvancedSkill(string className, string skills, int skillTimeout = -1, int classUseMode = 0, SkillUseMode skillUseMode = SkillUseMode.UseIfAvailable)
    {
        ClassName = className;
        Skills = skills;
        SkillTimeout = skillTimeout;
        ClassUseMode = (ClassUseMode)classUseMode;
        SkillUseMode = skillUseMode;
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
    public SkillUseMode SkillUseMode { get; set; } = SkillUseMode.UseIfAvailable;
    public override string ToString()
    {
        return $"{ClassUseMode} : {ClassName} => {Skills}";
    }
}
