
using BuriedEchoes.Resources;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField, Min(0.1f)] private float pickupRange = 3f;
    [SerializeField] private InventoryUI inventoryUI;

    private readonly ResourceInventory inventory = new();
    private readonly PlayerWallet wallet = new();
    private void Start()
    {
        inventoryUI?.Refresh(inventory, wallet);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
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
    }
}
