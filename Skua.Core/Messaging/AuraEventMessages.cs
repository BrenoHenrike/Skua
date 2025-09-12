using Skua.Core.Models.Auras;

namespace Skua.Core.Messaging;

/// <summary>
/// Message sent when an aura is activated on a subject.
/// </summary>
public sealed record AuraActivatedMessage(
    string AuraName,
    DateTime TimeStarted,
    int Duration,
    int StackValue,
    SubjectType Subject,
    AuraEventData FullData
);

/// <summary>
/// Message sent when an aura is deactivated/fades from a subject.
/// </summary>
public sealed record AuraDeactivatedMessage(
    string AuraName,
    SubjectType Subject,
    AuraEventData FullData
);

/// <summary>
/// Message sent when an aura's stack value changes.
/// </summary>
public sealed record AuraStackChangedMessage(
    string AuraName,
    int OldValue,
    int NewValue,
    SubjectType Subject,
    AuraEventData FullData
);

/// <summary>
/// Complete aura event data.
/// </summary>
public class AuraEventData
{
    public string Name { get; set; } = string.Empty;
    public int StackValue { get; set; }
    public DateTime TimeStarted { get; set; }
    public int DurationSeconds { get; set; }
    public DateTime ExpiresAt => TimeStarted.AddSeconds(DurationSeconds);
    public int SecondsRemaining => Math.Max(0, (int)(ExpiresAt - DateTime.Now).TotalSeconds);
    public bool IsPassive { get; set; }
    public string? Category { get; set; }
    public string? PotionType { get; set; }
    public SubjectType Subject { get; set; }

    public static AuraEventData FromAura(Aura aura, SubjectType subject)
    {
        return new AuraEventData
        {
            Name = aura.Name ?? string.Empty,
            StackValue = aura.Value,
            TimeStarted = aura.TimeStamp ?? DateTime.Now,
            DurationSeconds = aura.Duration,
            IsPassive = aura.Passive ?? false,
            Category = aura.Category,
            PotionType = aura.PotionType,
            Subject = subject
        };
    }
}