
using BuriedEchoes.Resources;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField, Min(0.1f)] private float pickupRange = 3f;

    private readonly ResourceInventory inventory = new();

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

        CollectibleItem collectible =
            hit.collider.GetComponentInParent<CollectibleItem>();

        if (collectible == null)
            return;

        if (collectible.TryCollect(inventory))
        {
            Debug.Log(
                $"Total Gold: {inventory.GetAmount(ResourceType.Gold)}"
            );
        }
    }
}
