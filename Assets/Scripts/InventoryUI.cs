
using BuriedEchoes.Resources;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private TMP_Text inventoryText;

    public void Refresh(ResourceInventory inventory, PlayerWallet wallet)
    {
        if (inventoryText == null || inventory == null || wallet == null)
            return;

        int totalItems = inventory.TotalItems;

        string capacityColor = inventory.IsFull ? "#FF6666" : "#A0A5AA";

        int iron = inventory.GetAmount(ResourceType.Iron);
        int largeIron = inventory.GetAmount(ResourceType.LargeIron);

        int copper = inventory.GetAmount(ResourceType.Copper);
        int largeCopper = inventory.GetAmount(ResourceType.LargeCopper);

        int silver = inventory.GetAmount(ResourceType.Silver);
        int largeSilver = inventory.GetAmount(ResourceType.LargeSilver);

        int gold = inventory.GetAmount(ResourceType.Gold);
        int largeGold = inventory.GetAmount(ResourceType.LargeGold);

        // Always visible.
        string text =
            "<color=#E6C38D><b>INVENTORY</b></color>\n" +
            $"<color={capacityColor}>Capacity: {totalItems} / {inventory.Capacity}</color>\n" +
            $"<color=#7EDC87><b>Money: ${wallet.Balance}</b></color>";

        // Add the separator only when carrying minerals.
        if (totalItems > 0)
        {
            text += "\n<color=#666666>────────────</color>";
        }

        // Iron
        if (iron > 0)
            text += $"\n<color=#A0A5AA>Iron: {iron}</color>";

        if (largeIron > 0)
            text += $"\n<color=#A0A5AA>Large Iron: {largeIron}</color>";

        // Copper
        if (copper > 0)
            text += $"\n<color=#B87333>Copper: {copper}</color>";

        if (largeCopper > 0)
            text += $"\n<color=#B87333>Large Copper: {largeCopper}</color>";

        // Silver
        if (silver > 0)
            text += $"\n<color=#C0C0C0>Silver: {silver}</color>";

        if (largeSilver > 0)
            text += $"\n<color=#C0C0C0>Large Silver: {largeSilver}</color>";

        // Gold
        if (gold > 0)
            text += $"\n<color=#FFD700>Gold: {gold}</color>";

        if (largeGold > 0)
            text += $"\n<color=#FFD700>Large Gold: {largeGold}</color>";

        inventoryText.text = text;
    }
}
