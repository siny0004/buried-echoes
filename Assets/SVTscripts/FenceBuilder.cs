
using UnityEngine;

public class FenceBuilder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SVTterrain terrain;
    [SerializeField] private GameObject fenceSectionPrefab;

    [Header("Fence Settings")]
    [SerializeField] private float sectionLength = 4f;
    [SerializeField] private float groundHeight = 5f;
    [SerializeField] private float boundaryInset = 0.5f;

    [Header("Shed Entrance")]
    [SerializeField] private bool createShedEntrance = true;
    [SerializeField, Min(0f)] private float entranceWidth = 8f;
    [SerializeField] private float entranceCenterX = 0f;

    [ContextMenu("Generate Fence")]
    public void GenerateFence()
    {
        if (terrain == null || fenceSectionPrefab == null)
        {
            Debug.LogWarning("Assign Terrain and Fence Section Prefab.");
            return;
        }

        if (sectionLength <= 0f)
        {
            Debug.LogWarning("Section Length must be greater than zero.");
            return;
        }

        // Remove previously generated sections.
        Transform oldFence = transform.Find("GeneratedFence");

        if (oldFence != null)
        {
            DestroyImmediate(oldFence.gameObject);
        }

        GameObject fenceParent = new GameObject("GeneratedFence");
        fenceParent.transform.SetParent(transform, false);

        float chunkSize = ChunkSVT.chunkWidth;

        Vector3 terrainOrigin = terrain.transform.position;

        float minX = terrainOrigin.x +
                     terrain.minChunkX * chunkSize + boundaryInset;

        float maxX = terrainOrigin.x +
                     (terrain.maxChunkX + 1) * chunkSize - boundaryInset;

        float minZ = terrainOrigin.z +
                     terrain.minChunkZ * chunkSize + boundaryInset;

        float maxZ = terrainOrigin.z +
                     (terrain.maxChunkZ + 1) * chunkSize - boundaryInset;

        float worldY = terrainOrigin.y + groundHeight;

        // South side - leave an opening for the mining shed.
        CreateSide(
            new Vector3(minX, worldY, minZ),
            new Vector3(maxX, worldY, minZ),
            fenceParent.transform,
            createShedEntrance
        );
        // North side.
        CreateSide(
            new Vector3(minX, worldY, maxZ),
            new Vector3(maxX, worldY, maxZ),
            fenceParent.transform
        );

        // East and West sides.
        CreateSide(
            new Vector3(minX, worldY, minZ),
            new Vector3(minX, worldY, maxZ),
            fenceParent.transform
        );

        CreateSide(
            new Vector3(maxX, worldY, minZ),
            new Vector3(maxX, worldY, maxZ),
            fenceParent.transform
        );

        Debug.Log("Fence generated successfully.");
    }

    private void CreateSide(
        Vector3 start,
        Vector3 end,
        Transform parent,
        bool hasEntrance = false)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;

        int sectionCount = Mathf.CeilToInt(distance / sectionLength);

        if (sectionCount == 0)
            return;

        Vector3 normalizedDirection = direction.normalized;

        // Distribute sections evenly so the sides meet.
        float actualLength = distance / sectionCount;

        Quaternion rotation = Quaternion.FromToRotation(
            Vector3.right,
            normalizedDirection
        );

        for (int i = 0; i < sectionCount; i++)
        {
            Vector3 position = start +
                normalizedDirection * ((i + 0.5f) * actualLength);

            // Skip fence sections overlapping the shed entrance.
            if (hasEntrance)
            {
                float sectionMinX = position.x - actualLength * 0.5f;
                float sectionMaxX = position.x + actualLength * 0.5f;

                float entranceMinX = entranceCenterX - entranceWidth * 0.5f;
                float entranceMaxX = entranceCenterX + entranceWidth * 0.5f;

                if (sectionMaxX > entranceMinX &&
                    sectionMinX < entranceMaxX)
                {
                    continue;
                }
            }

            GameObject section = Instantiate(
                fenceSectionPrefab,
                position,
                rotation,
                parent
            );

            // FenceSection is designed to be 4 metres wide
            // along its local X axis.
            Vector3 scale = section.transform.localScale;
            scale.x *= actualLength / sectionLength;
            section.transform.localScale = scale;
        }
    }
}
