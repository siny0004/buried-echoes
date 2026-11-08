using BuriedEchoes.Resources;
using UnityEngine;

public class UpgradeStation : MonoBehaviour
{
    public void PurchaseShovelUpgrade(
        PlayerWallet wallet,
        DiggingToolStats toolStats)
    {
        if (wallet == null || toolStats == null)
            return;

        if (!toolStats.CanUpgradeShovel)
            return;

        int cost = toolStats.NextShovelUpgradeCost;

        if (wallet.Balance < cost)
            return;

        if (!wallet.TrySpend(cost))
            return;

        if (!toolStats.TryUpgradeShovel())
        {
            // Refund the player if the upgrade unexpectedly fails.
            wallet.AddMoney(cost);
        }
    }

    public void PurchaseInventoryUpgrade(
    PlayerWallet wallet,
    ResourceInventory inventory)
    {
        if (wallet == null || inventory == null)
            return;

        if (!inventory.CanUpgradeInventory)
            return;

        int cost = inventory.NextInventoryUpgradeCost;

        if (wallet.Balance < cost)
            return;

        if (!wallet.TrySpend(cost))
            return;

        if (!inventory.TryUpgradeInventory())
        {
            // Refund the player if the upgrade unexpectedly fails.
            wallet.AddMoney(cost);
        }
    }

    public void PurchaseRopeUpgrade(
    PlayerWallet wallet,
    PlayerMovement playerMovement)
    {
        if (wallet == null || playerMovement == null)
            return;

        if (!playerMovement.CanUpgradeRope())
            return;

        int cost = playerMovement.GetRopeUpgradeCost();

        if (wallet.Balance < cost)
            return;

        if (!wallet.TrySpend(cost))
            return;

        if (!playerMovement.UpgradeRope())
        {
            // Refund the player if the upgrade unexpectedly fails.
            wallet.AddMoney(cost);
        }
    }
}