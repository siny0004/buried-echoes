
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

        inventoryText.text =
            "<color=#E6C38D><b>INVENTORY</b></color>\n\n" +
            $"<color=#B87333>Copper</color>: {inventory.GetAmount(ResourceType.Copper)}\n" +
            $"<color=#A0A5AA>Iron</color>: {inventory.GetAmount(ResourceType.Iron)}\n" +
            $"<color=#FFD700>Gold</color>: {inventory.GetAmount(ResourceType.Gold)}\n\n" +
            $"<color=#7EDC87><b>Money: ${wallet.Balance}</b></color>";
    }
}
