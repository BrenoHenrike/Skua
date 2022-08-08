using CommunityToolkit.Mvvm.Messaging.Messages;
using Skua.Core.Models.Items;
using Skua.Core.Models.Shops;

namespace Skua.Core.Messaging;
public record LogoutMessage();
public record LoginMessage(string Username);
public record PlayerDeathMessage();
public record MonsterKilledMessage(int MapID);
public record QuestAcceptedMessage(int QuestID);
public record QuestTurninMessage(int QuestID);
public record MapChangedMessage(string Map);
public record CellChangedMessage(string Map, string Cell, string Pad);
public record ReloginTriggeredMessage(bool WasKicked);
public record ExtensionPacketMessage(dynamic Packet);
public record PlayerAFKMessage();
public record TryBuyItemMessage(int ShopID, int ItemID, int ShopItemID);
public record CounterAttackMessage(bool Faded);
public record ItemAddedToBankMessage(ItemBase Item, int QuantityNow);
public record RunToAreaMessage(string Zone);
public record ItemDroppedMessage(ItemBase Item, bool AddedToInv = false, int QuantityNow = 0);
public record ShopLoadedMessage(ShopInfo Info);
public record BankLoadedMessage();
public record ItemBoughtMessage(int CharItemID);
public record ItemSoldMessage(int CharItemID, int QuantitySold, int CurrentQuantity, int Cost, bool IsAC);
public record ScriptStartedMessage();
public record ScriptErrorMessage(Exception Exception);
public record ScriptStoppingMessage();
public record ScriptStoppedMessage();
public class ScriptStoppingRequestMessage : AsyncRequestMessage<bool?>
{
    public Exception? Exception { get; }

    public ScriptStoppingRequestMessage(Exception? exception)
    {
        Exception = exception;
    }
}
