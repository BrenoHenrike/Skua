using System.ComponentModel;
using Skua.Core.Models.Quests;

namespace Skua.Core.Interfaces;

public interface IScriptQuest : INotifyPropertyChanged
{
    /// <summary>
    /// The interval, in milliseconds, at which to complete the <see cref="Registered"/> quests.
    /// </summary>
    int RegisterCompleteInterval { get; set; }
    /// <summary>
    /// A list of the player's currently active quests.
    /// </summary>
    List<Quest> Active { get; }
    /// <summary>
    /// A list of the player's currently active quests which are ready to turn in.
    /// </summary>
    List<Quest> Completed { get; }
    /// <summary>
    /// A list of the quests loaded in the current session.
    /// </summary>
    List<Quest> Tree { get; }
    /// <summary>
    /// Dictionary with <see cref="QuestData"/> objects of all the game quests.
    /// </summary>
    Dictionary<int, QuestData> CachedDictionary { get; set; }
    /// <summary>
    /// List with <see cref="QuestData"/> objects of all the game quests.
    /// </summary>
    List<QuestData> Cached { get; set; }

    /// <summary>
    /// List of IDs of the current registered quests to complete automatically.
    /// </summary>
    IEnumerable<int> Registered { get; }

    /// <summary>
    /// Register quests to be completed while doing another actions, this enables the possibility to complete quests while in combat.
    /// </summary>
    /// <param name="ids">Quests to be completed.</param>
    void RegisterQuests(params int[] ids);
    /// <summary>
    /// Unegister quests so they are not automatically completed.
    /// </summary>
    /// <param name="ids">Quests to be removed.</param>
    void UnregisterQuests(params int[] ids);
    /// <summary>
    /// Removes all registered quests from the <see cref="Registered">list</see>
    /// </summary>
    void UnregisterAllQuests();

    /// <summary>
    /// Accepts the quest with specified <paramref name="id"/>.
    /// </summary>
    /// <param name="id">ID of the quest.</param>
    bool Accept(int id);
    /// <summary>
    /// Accepts the quests with specified <paramref name="ids"/>.
    /// </summary>
    /// <param name="ids">IDs of the quests.</param>
    void Accept(params int[] ids);
    /// <summary>
    /// Tries to accept a quest until it is successfully accepted.
    /// </summary>
    /// <param name="id">ID of the quest.</param>
    /// <remarks>It will try <see cref="IScriptOption.MaximumTries"/> then move on even if the quests were not accepted.</remarks>
    bool EnsureAccept(int id);
    /// <summary>
    /// Tries to accept multiple quests until they are successfully accepted.
    /// </summary>
    /// <param name="ids">IDs of the quests.</param>
    /// <remarks>It will try <see cref="IScriptOption.MaximumTries"/> then move on even if the quests were not accepted.</remarks>void EnsureAccept(params int[] ids);
    void EnsureAccept(params int[] ids);
    /// <summary>
    /// Checks if the quest with specified <paramref name="id"/> can be turned in.
    /// </summary>
    /// <param name="id">ID of the quest.</param>
    /// <returns><see langword="true"/> if the specified quest is ready to turn in or not.</returns>
    /// <remarks>This will only look if the status of the quest is complete, which can not always be right.</remarks>
    bool CanComplete(int id);
    /// <summary>
    /// Checks if the player has all the required items to complete the quest;
    /// </summary>
    /// <param name="id">ID of the quest.</param>
    /// <returns><see langword="true"/> if the specified quest is ready to turn in or not.</returns>
    bool CanCompleteFullCheck(int id);
    /// <summary>
    /// Attempts to turn in the specified quest.
    /// </summary>
    /// <param name="id">ID of the quest.</param>
    /// <param name="itemId">ID of the item chosen when the quest is turned in.</param>
    /// <param name="special">Whether the quest is marked 'special' or not.</param>
    /// <remarks>The <paramref name="itemId"/> can be used to acquire a particular item when there is a choice of rewards from the quest. For example, in the Voucher Item: Totem of Nulgath quest, you are given the choice of getting a Totem of Nulgath or 10 Gems of Nulgath.</remarks>
    bool Complete(int id, int itemId = -1, bool special = false);
    /// <summary>
    /// Attempts to turn in multiple quests.
    /// </summary>
    /// <param name="ids">IDs of the quests.</param>
    /// <remarks>Use for 3 or more quests so it doesn't use the wrong overload.</remarks>
    void Complete(params int[] ids);
    /// <summary>
    /// Tries to turn in the specified quest until it is successfully turned in.
    /// </summary>
    /// <param name="id">ID of the quest.</param>
    /// <param name="itemId">ID of the item chosen when the quest is turned in.</param>
    /// <param name="special">Whether the quest is marked 'special' or not.</param>
    bool EnsureComplete(int id, int itemId = -1, bool special = false);
    /// <summary>
    /// Attempts to turn in multiple quests until they are successfully accepted.
    /// </summary>
    /// <param name="ids">IDs of the quests.</param>
    /// <remarks>Use for 3 or more quests so it doesn't use the wrong overload.</remarks>void EnsureComplete(params int[] ids);
    void EnsureComplete(params int[] ids);
    /// <summary>
    /// Performs all checks to see if a quest can be accepted/turned in.
    /// </summary>
    /// <param name="id">The id of the quest.</param>
    /// <returns><see langword="true0"/> if the quest can be accepted.</returns>
    /// <remarks>Most quests can be accepted even if they are marked red.</remarks>
    bool IsAvailable(int id);
    /// <summary>
    /// Checks if the specified quest is a completed daily quest.
    /// </summary>
    /// <param name="id">ID of the quest.</param>
    /// <returns><see langword="true"/> if the specified quest is a daily quest that the player has already completed.</returns>
    bool IsDailyComplete(int id);
    /// <summary>
    /// Checks if the specified quest is currently in progress.
    /// </summary>
    /// <param name="id">ID of the quest.</param>
    /// <returns><see langword="true"/> if the specified quest is in progress.</returns>
    bool IsInProgress(int id);
    /// <summary>
    /// Checks if a storyline quest is unlocked.
    /// </summary>
    /// <param name="id">ID of the quest.</param>
    /// <returns><see langword="true"/> if the quest is unlocked.</returns>
    bool IsUnlocked(int id);
    /// <summary>
    /// Checks if a quest has been completed before in a questline.
    /// </summary>
    /// <param name="id">ID of the quest.</param>
    /// <returns><see langword="true"/> if the quest has been completed before.</returns>
    bool HasBeenCompleted(int id);
    /// <summary>
    /// Loads the quests with specified <paramref name="ids"/>.
    /// </summary>
    /// <param name="ids">IDs of the quests to load.</param>
    void Load(params int[] ids);
    /// <summary>
    /// Loads the quest with the specified <paramref name="id"/> and waits until it's in the quest tree.
    /// </summary>
    /// <param name="id">ID of the quest to load.</param>
    /// <returns>The <see cref="Quest"/> with the given ID.</returns>
    Quest EnsureLoad(int id);
    /// <summary>
    /// Tries to get the quest with the given ID if it is loaded.
    /// </summary>
    /// <param name="id">The ID of the quest to get.</param>
    /// <param name="quest">The quest object to set as the result.</param>
    /// <returns>True if the quest is loaded and quest was set succesfully.</returns>
    bool TryGetQuest(int id, out Quest? quest);
    /// <summary>
    /// Send a client-side packet that makes the game think you have completed a questline up to a certain point
    /// </summary>
    /// <param name="id">ID of the quest you want the game to think you have completed</param>
    bool UpdateQuest(int id);
    /// <summary>
    /// Send a client-side packet that makes the game think you have completed a questline up to a certain point
    /// </summary>
    /// <param name="value">Value property of the quest you want it to think you have completed</param>
    /// <param name="slot">Slot property of the questline you want it to think you have progressed</param>
    void UpdateQuest(int value, int slot);
    /// <summary>
    /// Load the quests into the <see cref="Cached"/> list from the Quests.txt.
    /// </summary>
    /// <returns></returns>
    void LoadCachedQuests();
    /// <summary>
    /// Quests a list of <see cref="QuestData"/> from the Quests.txt file.
    /// </summary>
    /// <param name="start">The starting ID.</param>
    /// <param name="count">How many will be taken.</param>
    /// <returns></returns>
    List<QuestData> GetCachedQuests(int start, int count);
    /// <summary>
    /// Quests a list of <see cref="QuestData"/> from the Quests.txt file.
    /// </summary>
    /// <param name="ids">IDs to get.</param>
    /// <returns></returns>
    List<QuestData> GetCachedQuests(params int[] ids);
}
