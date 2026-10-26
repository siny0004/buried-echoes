
using BuriedEchoes.Resources;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField, Min(0.1f)] private float pickupRange = 3f;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private DiggingTester diggingTester;
    [SerializeField] private InteractionUI interactionUI;
    private readonly ResourceSeller resourceSeller = new();

    private readonly ResourceInventory inventory = new();
    private readonly PlayerWallet wallet = new();
    private void Start()
    {
        inventoryUI?.Refresh(inventory, wallet);
    }

    private void Update()
    {
        UpdateInteractionPrompt();

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }

        // Temporary developer money cheat.
        if (Input.GetKeyDown(KeyCode.M))
        {
            wallet.AddMoney(10000);
            inventoryUI?.Refresh(inventory, wallet);
        }
    }

    private void TryInteract()
    {
        if (playerCamera == null)
            return;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (!Physics.Raycast(ray, out RaycastHit hit, pickupRange))
            return;

        // Check whether the player is looking at a collectible.
        CollectibleItem collectible =
            hit.collider.GetComponentInParent<CollectibleItem>();

        if (collectible != null)
        {
            if (collectible.TryCollect(inventory))
            {
                inventoryUI?.Refresh(inventory, wallet);
            }

            return;
        }

        // Check whether the player is looking at a selling station.
        SellingStation station =
            hit.collider.GetComponentInParent<SellingStation>();

        if (station != null)
        {
            station.Sell(inventory, wallet);

            inventoryUI?.Refresh(inventory, wallet);
        }
        // Check whether the player is looking at an upgrade station.
        UpgradeStation upgradeStation =
            hit.collider.GetComponentInParent<UpgradeStation>();

        if (upgradeStation != null && diggingTester != null)
        {
            upgradeStation.PurchaseShovelUpgrade(
                wallet,
                diggingTester.ToolStats
            );

            inventoryUI?.Refresh(inventory, wallet);
        }
    }

    private void UpdateInteractionPrompt()
    {
        if (interactionUI == null)
            return;

        if (playerCamera == null)
        {
            interactionUI.Hide();
            return;
        }

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (!Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            interactionUI.Hide();
            return;
        }

        // Collectible resources
        CollectibleItem collectible =
            hit.collider.GetComponentInParent<CollectibleItem>();

        if (collectible != null)
        {
            interactionUI.Show(
                $"[E] Collect {collectible.ResourceType}"
            );
            return;
        }

        // Selling station
        SellingStation sellingStation =
            hit.collider.GetComponentInParent<SellingStation>();

        if (sellingStation != null)
        {
            int value = resourceSeller.GetTotalValue(inventory);

            interactionUI.Show(
                $"[E] Sell resources (${value})"
            );
            return;
        }

        // Upgrade station
        UpgradeStation upgradeStation =
            hit.collider.GetComponentInParent<UpgradeStation>();

        if (upgradeStation != null && diggingTester != null)
        {
            DiggingToolStats stats = diggingTester.ToolStats;

            if (!stats.CanUpgradeShovel)
            {
                interactionUI.Show("Shovel MAX LEVEL");
                return;
            }

            interactionUI.Show(
                $"[E] Upgrade Shovel Lv. {stats.ShovelLevel} " +
                $"→ {stats.ShovelLevel + 1} " +
                $"(${stats.NextShovelUpgradeCost})"
            );
            return;
        }

        interactionUI.Hide();
    }
}
