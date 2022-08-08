using Skua.Core.Models.Skills;

namespace Skua.Core.Messaging;
public sealed record SaveAdvancedSkillMessage(AdvancedSkill AdvSkill);
public sealed record EditAdvancedSkillMessage(AdvancedSkill AdvSkill);
