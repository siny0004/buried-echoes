using BuriedEchoes.Resources;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField, Min(0.1f)] private float pickupRange = 3f;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private DiggingTester diggingTester;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private MouseLook mouseLook;
    [SerializeField] private InteractionUI interactionUI;

    [Header("Ore Pickup Audio")]
    [SerializeField] private AudioSource orePickupAudioSource;

    [Header("Selling Audio")]
    [SerializeField] private AudioSource sellingAudioSource;

    [Header("Upgrade Menu")]
    [SerializeField] private GameObject upgradePanel;

    [Header("Upgrade UI")]
    [SerializeField] private Button shovelUpgradeButton;
    [SerializeField] private Button inventoryUpgradeButton;
    [SerializeField] private Button ropeUpgradeButton;
    [SerializeField] private TMP_Text upgradeBalanceText;

    [SerializeField] private TMP_Text shovelNameText;
    [SerializeField] private TMP_Text shovelLevelText;
    [SerializeField] private TMP_Text shovelCostText;

    [SerializeField] private TMP_Text inventoryNameText;
    [SerializeField] private TMP_Text inventoryLevelText;
    [SerializeField] private TMP_Text inventoryCostText;

    [SerializeField] private TMP_Text ropeNameText;
    [SerializeField] private TMP_Text ropeLevelText;
    [SerializeField] private TMP_Text ropeCostText;

    private bool upgradeMenuOpen = false;
    private readonly ResourceSeller resourceSeller = new();
    private readonly ResourceInventory inventory = new();
    private readonly PlayerWallet wallet = new();
    private void Start()
    {
        inventoryUI?.Refresh(inventory, wallet);
    }

    private void Update()
    {
        // Handle the upgrade menu while it is open.
        if (upgradeMenuOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseUpgradeMenu();
            }

            return;
        }

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
                Debug.Log($"Collected resource type: {collectible.ResourceType}");

                // Play pickup sound only when collection succeeds.
                if (orePickupAudioSource != null)
                {
                    orePickupAudioSource.PlayOneShot(orePickupAudioSource.clip);
                }

                inventoryUI?.Refresh(inventory, wallet);
            }
            return;
        }

        // Check whether the player is looking at a selling station.
        SellingStation station =
            hit.collider.GetComponentInParent<SellingStation>();

        if (station != null)
        {
            // Check the value before selling empties the inventory.
            int totalValue = resourceSeller.GetTotalValue(inventory);

            if (totalValue > 0)
            {
                station.Sell(inventory, wallet);

                if (sellingAudioSource != null &&
                    sellingAudioSource.clip != null)
                {
                    sellingAudioSource.PlayOneShot(
                        sellingAudioSource.clip
                    );
                }
            }

            inventoryUI?.Refresh(inventory, wallet);
            return;
        }

        // Check whether the player is looking at an upgrade station.
        UpgradeStation upgradeStation =
            hit.collider.GetComponentInParent<UpgradeStation>();

        if (upgradeStation != null)
        {
            OpenUpgradeMenu();
            return;
        }
    }

    private void OpenUpgradeMenu()
    {
        if (upgradePanel == null)
            return;

        upgradeMenuOpen = true;
        upgradePanel.SetActive(true);
        RefreshUpgradeUI();

        if (playerMovement != null)
            playerMovement.ControlsLocked = true;
        
        diggingTester?.SetControlsLocked(true);

        if (mouseLook != null)
            mouseLook.ControlsLocked = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        interactionUI?.Hide();
    }

    private void CloseUpgradeMenu()
    {
        upgradeMenuOpen = false;
        upgradePanel.SetActive(false);

        if (playerMovement != null)
            playerMovement.ControlsLocked = false;
        
        diggingTester?.SetControlsLocked(false);

        if (mouseLook != null)
            mouseLook.ControlsLocked = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void BuyShovelUpgrade()
    {
        if (!upgradeMenuOpen || diggingTester == null)
            return;

        UpgradeStation station = FindFirstObjectByType<UpgradeStation>();

        if (station == null)
            return;

        station.PurchaseShovelUpgrade(
            wallet,
            diggingTester.ToolStats
        );

        inventoryUI?.Refresh(inventory, wallet);
        diggingTester.RefreshToolUI();
        RefreshUpgradeUI();
    }

    public void BuyInventoryUpgrade()
    {
        if (!upgradeMenuOpen)
            return;

        UpgradeStation station = FindFirstObjectByType<UpgradeStation>();

        if (station == null)
            return;

        station.PurchaseInventoryUpgrade(wallet, inventory);

        inventoryUI?.Refresh(inventory, wallet);
        RefreshUpgradeUI();
    }

    public void BuyRopeUpgrade()
    {
        if (!upgradeMenuOpen || playerMovement == null)
            return;

        UpgradeStation station =
            FindFirstObjectByType<UpgradeStation>();

        if (station == null)
            return;

        station.PurchaseRopeUpgrade(wallet, playerMovement);

        inventoryUI?.Refresh(inventory, wallet);
        RefreshUpgradeUI();
    }

    private void RefreshUpgradeUI()
    {
        if (upgradeBalanceText != null)
            upgradeBalanceText.text = $"Balance: ${wallet.Balance}";

        Color affordableColor = new Color32(70, 120, 70, 255);     // green
        Color unaffordableColor = new Color32(130, 60, 60, 255);   // red
        Color maxedColor = new Color32(90, 90, 90, 255);           // gray

        // -----------------------------
        // Shovel upgrade
        // -----------------------------
        if (shovelUpgradeButton != null && diggingTester != null)
        {
            DiggingToolStats stats = diggingTester.ToolStats;
            Image shovelImage = shovelUpgradeButton.GetComponent<Image>();

            if (shovelNameText != null)
                shovelNameText.text = "UPGRADE SHOVEL";

            if (stats.CanUpgradeShovel)
            {
                if (shovelLevelText != null)
                    shovelLevelText.text =
                        $"Level {stats.ShovelLevel} → {stats.ShovelLevel + 1}";

                if (shovelCostText != null)
                    shovelCostText.text = $"Cost: ${stats.NextShovelUpgradeCost}";

                bool canAfford = wallet.Balance >= stats.NextShovelUpgradeCost;
                shovelUpgradeButton.interactable = canAfford;

                if (shovelImage != null)
                    shovelImage.color = canAfford ? affordableColor : unaffordableColor;
            }
            else
            {
                if (shovelLevelText != null)
                    shovelLevelText.text = "MAX LEVEL";

                if (shovelCostText != null)
                    shovelCostText.text = "Purchased";

                shovelUpgradeButton.interactable = false;

                if (shovelImage != null)
                    shovelImage.color = maxedColor;
            }
        }

        // -----------------------------
        // Inventory upgrade
        // -----------------------------
        if (inventoryUpgradeButton != null)
        {
            Image inventoryImage = inventoryUpgradeButton.GetComponent<Image>();

            if (inventoryNameText != null)
                inventoryNameText.text = "UPGRADE INVENTORY";

            if (inventory.CanUpgradeInventory)
            {
                int nextCapacity = inventory.InventoryLevel switch
                {
                    1 => 8,
                    2 => 12,
                    3 => 18,
                    4 => 25,
                    _ => inventory.Capacity
                };

                if (inventoryLevelText != null)
                    inventoryLevelText.text =
                        $"{inventory.Capacity} → {nextCapacity} slots";

                if (inventoryCostText != null)
                    inventoryCostText.text =
                        $"Cost: ${inventory.NextInventoryUpgradeCost}";

                bool canAfford = wallet.Balance >= inventory.NextInventoryUpgradeCost;
                inventoryUpgradeButton.interactable = canAfford;

                if (inventoryImage != null)
                    inventoryImage.color = canAfford ? affordableColor : unaffordableColor;
            }
            else
            {
                if (inventoryLevelText != null)
                    inventoryLevelText.text = "MAX LEVEL";

                if (inventoryCostText != null)
                    inventoryCostText.text = "25 slots";

                inventoryUpgradeButton.interactable = false;

                if (inventoryImage != null)
                    inventoryImage.color = maxedColor;
            }
        }

        // -----------------------------
        // Rope upgrade
        // -----------------------------
        if (ropeUpgradeButton != null && playerMovement != null)
        {
            Image ropeImage = ropeUpgradeButton.GetComponent<Image>();

            if (ropeNameText != null)
                ropeNameText.text = "UPGRADE ROPE";

            if (playerMovement.CanUpgradeRope())
            {
                if (ropeLevelText != null)
                    ropeLevelText.text =
                        $"Level {playerMovement.RopeLevel} → {playerMovement.RopeLevel + 1}";

                int cost = playerMovement.GetRopeUpgradeCost();

                if (ropeCostText != null)
                    ropeCostText.text = $"Cost: ${cost}";

                bool canAfford = wallet.Balance >= cost;
                ropeUpgradeButton.interactable = canAfford;

                if (ropeImage != null)
                    ropeImage.color =
                        canAfford ? affordableColor : unaffordableColor;
            }
            else
            {
                if (ropeLevelText != null)
                    ropeLevelText.text = "MAX LEVEL";

                if (ropeCostText != null)
                    ropeCostText.text = "12 m/s";

                ropeUpgradeButton.interactable = false;

                if (ropeImage != null)
                    ropeImage.color = maxedColor;
            }
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
            if (inventory.IsFull)
            {
                interactionUI.Show("INVENTORY FULL", Color.red);
            }
            else
            {
                interactionUI.Show(
                    $"[E] Collect {collectible.ResourceType}"
                );
            }

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

        if (upgradeStation != null)
        {
            interactionUI.Show("[E] Open Upgrade Station");
            return;
        }

        interactionUI.Hide();
    }
}
