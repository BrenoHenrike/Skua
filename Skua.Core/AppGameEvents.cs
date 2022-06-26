using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

namespace Skua.Core;
internal static class AppGameEvents
{
    internal static event ItemDroppedHandler? ItemDropped;
    internal static event LogoutEventHandler? Logout;

    internal static void OnItemDropped(ItemBase item, bool addedToInv = false, int quantityNow = 0)
    {
        ItemDropped?.Invoke(item, addedToInv, quantityNow);
    }
    internal static void OnLogout()
    {
        Logout?.Invoke();
    }
}
