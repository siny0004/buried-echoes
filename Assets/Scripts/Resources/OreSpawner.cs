
using System;
using System.Collections;
using UnityEngine;

namespace BuriedEchoes.Resources
{
    public class OreSpawner : MonoBehaviour
    {

        [Serializable]
        public class OreSpawnEntry
        {
            public ResourceType resourceType;

            [Header("Prefabs")]
            public GameObject prefab;
            public GameObject largePrefab;

            [Header("Spawn Settings")]
            [Min(1)]
            public int count = 10;

            public Vector2 depthRange = new(2f, 8f);

            [Range(0f, 100f)]
            public float largeChunkChance = 15f;
        }


        [Header("Resources")]
        [SerializeField] private OreSpawnEntry[] resources;

        [Header("Spawn Settings")]
        [SerializeField] private Vector2 areaSize = new(12f, 12f);
        [SerializeField] private float rayStartHeight = 50f;
        [SerializeField, Min(1)] private int maxAttemptsPerNugget = 20;

        [Header("Random Generation")]
        [SerializeField] private int seed = 12345;

        private System.Random random;

        private IEnumerator Start()
        {
            random = new System.Random(seed);

            // Allow terrain generation to finish.
            yield return new WaitForEndOfFrame();
            yield return null;

            Physics.SyncTransforms();

            SpawnResources();
        }

        private void SpawnResources()
        {
            if (resources == null || resources.Length == 0)
            {
                Debug.LogWarning("OreSpawner: No resources configured!");
                return;
            }

            foreach (OreSpawnEntry resource in resources)
            {
                if (resource.prefab == null)
                {
                    Debug.LogWarning(
                        $"OreSpawner: Missing prefab for {resource.resourceType}"
                    );
                    continue;
                }

                int spawned = 0;

                for (int i = 0; i < resource.count; i++)
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
                            // Ignore player and other objects.
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
                            resource.depthRange.x,
                            resource.depthRange.y
                        );

                        Vector3 position = surfacePoint +
                            Vector3.down * depth;

                        // Determine whether this should be a large chunk.
                        bool spawnLarge =
                            resource.largePrefab != null &&
                            RandomBetween(0f, 100f) < resource.largeChunkChance;

                        // Choose the appropriate prefab.
                        GameObject prefabToSpawn = spawnLarge
                            ? resource.largePrefab
                            : resource.prefab;

                        // Spawn the mineral.
                        GameObject spawnedOre = Instantiate(
                            prefabToSpawn,
                            position,
                            Quaternion.identity,
                            transform
                        );

                        // Randomize the visible orientation.
                        spawnedOre.transform.rotation = Quaternion.Euler(
                            RandomBetween(0f, 360f),
                            RandomBetween(0f, 360f),
                            RandomBetween(0f, 360f)
                        );

                        // Give each procedural rock a unique shape.
                        ProceduralOreRock[] rocks =
                            spawnedOre.GetComponentsInChildren<ProceduralOreRock>();

                        foreach (ProceduralOreRock rock in rocks)
                        {
                            int rockSeed = Mathf.FloorToInt(
                                RandomBetween(1f, 1000000f)
                            );

                            rock.RandomizeShape(rockSeed, 0.15f);
                        }

                        spawned++;
                        break;
                    }
                }

                Debug.Log(
                    $"OreSpawner: Spawned {spawned} {resource.resourceType} nuggets."
                );
            }
        }

        private float RandomBetween(float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }
    }
}
