using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SVTterrain is terrain made of SVT-chunks. Class contains 
/// </summary>
public class SVTterrain : MonoBehaviour
{
    public Transform characterRef;                                  // reference to character
    public GameObject chunkPrefab;                                  // reference to prefab with ChunkSVT script
    
    [Header("World Boundaries")]
    public int minChunkX = -4;
    public int maxChunkX = 3;
    public int minChunkZ = -4;
    public int maxChunkZ = 3;

    public List<GameObject> terrainChunks = new List<GameObject>(); // list of all created chunks
    private Vector2Int lastChunkPlayerLocation;                     // last player location described in chunks
    private bool lastChunkPlayerLocationNotInitialized = true;      // is "player location described in chunk" not initialized

    /// <summary>
    /// Always executes when player changes chunk position
    /// </summary>
    public void CheckAndCreateChunks(Vector2Int playerOnChunkLocation)
    {
        // Make first chunks within draw distance
        int xIndex = playerOnChunkLocation.x;
        int yIndex = playerOnChunkLocation.y;

        for (int x = xIndex - MainScript.DrawDistance; x < xIndex + MainScript.DrawDistance + 1; x++)
        {
            for (int y = yIndex - MainScript.DrawDistance; y < yIndex + MainScript.DrawDistance + 1; y++)
            {
                // Skip chunks outside our fixed world boundaries
                if (x < minChunkX || x > maxChunkX ||
                    y < minChunkZ || y > maxChunkZ)
                {
                    continue;
                }

                // Check if chunk already exists
                bool chunkExist = false;

                for (int i = 0; i < terrainChunks.Count; i++)
                {
                    if (terrainChunks[i].GetComponent<ChunkSVT>().xIndex == x &&
                        terrainChunks[i].GetComponent<ChunkSVT>().yIndex == y)
                    {
                        chunkExist = true;
                        break;
                    }
                }

                // If not, create it
                if (chunkExist == false)
                {
                    terrainChunks.Add(Instantiate(chunkPrefab));

                    terrainChunks[terrainChunks.Count - 1]
                        .GetComponent<ChunkSVT>()
                        .ChunkInit(this, transform.position, x, y);
                }
            }
        }
    }

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>    
    void Start()
    {
        // Generate the entire fixed digging area at startup.
        for (int x = minChunkX; x <= maxChunkX; x++)
        {
            for (int z = minChunkZ; z <= maxChunkZ; z++)
            {
                GameObject newChunk = Instantiate(chunkPrefab);

                terrainChunks.Add(newChunk);

                newChunk.GetComponent<ChunkSVT>().ChunkInit(
                    this,
                    transform.position,
                    x,
                    z
                );
            }
        }

        Debug.Log($"Generated {terrainChunks.Count} terrain chunks.");
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // Preserve the existing chunk update behavior.
        for (int i = 0; i < terrainChunks.Count; i++)
        {
            terrainChunks[i].GetComponent<ChunkSVT>().Update();
        }
    }
}
