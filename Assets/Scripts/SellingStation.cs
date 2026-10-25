
using BuriedEchoes.Resources;
using UnityEngine;

public class SellingStation : MonoBehaviour
{
    public void Sell(
        ResourceInventory inventory,
        PlayerWallet wallet)
    {
        if (inventory == null || wallet == null)
            return;

        ResourceSeller seller = new ResourceSeller();

        int earned = seller.SellAll(inventory);

        wallet.AddMoney(earned);

        Debug.Log(
            $"Selling Station: Sold resources for ${earned}. " +
            $"Current balance: ${wallet.Balance}"
        );
    }
}
