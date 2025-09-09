using CommunityToolkit.Mvvm.Messaging.Messages;
using Skua.Core.Models.Items;
using Skua.Core.Models.Shops;

namespace Skua.Core.Messaging;
public sealed record LogoutMessage();
public sealed record LoginMessage(string Username);
public sealed record PlayerDeathMessage();
public sealed record MonsterKilledMessage(int MapID);
public sealed record QuestAcceptedMessage(int QuestID);
public sealed record QuestTurninMessage(int QuestID);
public sealed record MapChangedMessage(string Map);
public sealed record CellChangedMessage(string Map, string Cell, string Pad);
public sealed record ReloginTriggeredMessage(bool WasKicked);
public sealed record ExtensionPacketMessage(dynamic Packet);
public sealed record PacketMessage(string Packet);
public sealed record PlayerAFKMessage();
public sealed record TryBuyItemMessage(int ShopID, int ItemID, int ShopItemID);
public sealed record CounterAttackMessage(bool Faded);
public sealed record ItemAddedToBankMessage(ItemBase Item, int QuantityNow);
public sealed record RunToAreaMessage(string Zone);
public sealed record ItemDroppedMessage(ItemBase Item, bool AddedToInv = false, int QuantityNow = 0);
public sealed record ShopLoadedMessage(ShopInfo Info);
public sealed record BankLoadedMessage();
public sealed record ItemBoughtMessage(int CharItemID);
public sealed record ItemSoldMessage(int CharItemID, int QuantitySold, int CurrentQuantity, int Cost, bool IsAC);
public sealed record ScriptStartedMessage();
public sealed record ScriptErrorMessage(Exception Exception);
public sealed record ScriptStoppingMessage();
public sealed record ScriptStoppedMessage();

public sealed class ScriptStoppingRequestMessage : AsyncRequestMessage<bool?>
{
    public Exception? Exception { get; }

    public ScriptStoppingRequestMessage(Exception? exception)
    {
        Exception = exception;
    }
}