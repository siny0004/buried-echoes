
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
        {
            Debug.Log("Digging strength is already at maximum level!");
            return;
        }

        int cost = toolStats.NextShovelUpgradeCost;

        if (wallet.Balance < cost)
        {
            Debug.Log(
                $"Not enough money! Upgrade costs ${cost}. " +
                $"Current balance: ${wallet.Balance}"
            );
            return;
        }

        if (!wallet.TrySpend(cost))
            return;

        if (toolStats.TryUpgradeShovel())
        {
            Debug.Log(
                $"Shovel upgraded to level {toolStats.ShovelLevel}! " +
                $"Radius: {toolStats.DigRadius}, " +
                $"Strength: {toolStats.DigStrength}, " +
                $"Cooldown: {toolStats.DigCooldown}s. " +
                $"Remaining money: ${wallet.Balance}"
            );
        }
        else
        {
            // Safety fallback: refund if the upgrade fails.
            wallet.AddMoney(cost);
        }
    }
}
