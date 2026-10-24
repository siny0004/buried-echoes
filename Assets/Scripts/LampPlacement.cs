using UnityEngine;

public class LampPlacement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject lampPrefab;

    [Header("Placement Settings")]
    [SerializeField] private float placementRange = 5f;
    [SerializeField] private KeyCode placementKey = KeyCode.F;

    private void Update()
    {
        if (Input.GetKeyDown(placementKey))
        {
            TryPlaceLamp();
        }
    }

    private void TryPlaceLamp()
    {
        if (playerCamera == null || lampPrefab == null)
            return;

        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        if (!Physics.Raycast(ray, out RaycastHit hit, placementRange))
            return;

        // Lamps can only be placed on voxel terrain.
        ChunkSVT terrainChunk =
            hit.collider.GetComponentInParent<ChunkSVT>();

        if (terrainChunk == null)
            return;

        // Only allow placement on reasonably flat ground.
        if (Vector3.Dot(hit.normal, Vector3.up) < 0.7f)
            return;

        Vector3 placementPosition = hit.point;

        Instantiate(
            lampPrefab,
            placementPosition,
            Quaternion.identity
        );
    }
}