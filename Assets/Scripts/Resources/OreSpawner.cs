using System;
using UnityEngine;

namespace BuriedEchoes.Resources
{
    public class OreSpawner : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private GameObject goldNuggetPrefab;

        [Header("Spawn Settings")]
        [SerializeField, Min(1)] private int nuggetCount = 10;
        [SerializeField] private Vector2 areaSize = new(12f, 12f);
        [SerializeField] private Vector2 depthRange = new(0.75f, 2.5f);
        [SerializeField] private float rayStartHeight = 50f;
        [SerializeField, Min(1)] private int maxAttemptsPerNugget = 20;

        [Header("Random Generation")]
        [SerializeField] private int seed = 12345;

        private System.Random random;

        private System.Collections.IEnumerator Start()
        {
            random = new System.Random(seed);

            // Temporary initialization safeguard.
            // Later we will use a terrain-generated event.
            yield return new WaitForEndOfFrame();
            yield return null;

            Physics.SyncTransforms();

            SpawnNuggets();
        }

        private void SpawnNuggets()
        {
            if (goldNuggetPrefab == null)
            {
                Debug.LogError("OreSpawner: Assign a gold nugget prefab!");
                return;
            }

            int spawned = 0;

            for (int i = 0; i < nuggetCount; i++)
            {
                for (int attempt = 0;
                     attempt < maxAttemptsPerNugget;
                     attempt++)
                {
                    float x = transform.position.x +
                        RandomBetween(-areaSize.x / 2f, areaSize.x / 2f);

                    float z = transform.position.z +
                        RandomBetween(-areaSize.y / 2f, areaSize.y / 2f);

                    Vector3 rayOrigin = new(
                        x,
                        transform.position.y + rayStartHeight,
                        z
                    );

                    RaycastHit[] hits = Physics.RaycastAll(
                        rayOrigin,
                        Vector3.down,
                        rayStartHeight * 2f,
                        ~0,
                        QueryTriggerInteraction.Ignore
                    );

                    float nearestDistance = float.PositiveInfinity;
                    bool foundTerrain = false;
                    Vector3 surfacePoint = default;

                    foreach (RaycastHit hit in hits)
                    {
                        // Ignore the player and other objects.
                        if (hit.collider.GetComponentInParent<ChunkSVT>()
                            == null)
                            continue;

                        if (hit.distance < nearestDistance)
                        {
                            nearestDistance = hit.distance;
                            surfacePoint = hit.point;
                            foundTerrain = true;
                        }
                    }

                    if (!foundTerrain)
                        continue;

                    float depth = RandomBetween(
                        depthRange.x,
                        depthRange.y
                    );

                    Vector3 position = surfacePoint +
                        Vector3.down * depth;

                    Instantiate(
                        goldNuggetPrefab,
                        position,
                        Quaternion.identity,
                        transform
                    );

                    spawned++;
                    break;
                }
            }

            Debug.Log($"OreSpawner: Spawned {spawned} gold nuggets.");
        }

        private float RandomBetween(float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }
    }
}
