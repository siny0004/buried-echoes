
using System.Collections.Generic;
using UnityEngine;

public class DiggingTester : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;

    [Header("Digging Settings")]
    [SerializeField] private float digRange = 5f;
    [SerializeField] private float digRadius = 1f;
    [SerializeField] private float digStrength = 160f;
    [SerializeField] private float digCooldown = 0.25f;

    private float nextDigTime;

    private readonly HashSet<ChunkSVT> changedChunks = new();
    private readonly HashSet<ChunkSVT> rebuildChunks = new();

    private void Update()
    {
        if (Input.GetMouseButton(0) &&
            Time.time >= nextDigTime)
        {
            TryDig();
            nextDigTime = Time.time + digCooldown;
        }
    }

    private void TryDig()
    {
        if (targetCamera == null)
            return;

        Ray ray = targetCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        if (!Physics.Raycast(ray, out RaycastHit hit, digRange))
            return;

        ChunkSVT hitChunk =
            hit.collider.GetComponentInParent<ChunkSVT>();

        if (hitChunk == null)
            return;

        Vector3 digPosition =
            hit.point - hit.normal * 0.5f;

        DigAcrossChunks(hitChunk, digPosition);
    }

    private void DigAcrossChunks(
        ChunkSVT hitChunk,
        Vector3 digPosition)
    {
        changedChunks.Clear();
        rebuildChunks.Clear();

        // The hit chunk and its eight horizontal neighbors
        // are enough for our current 1-metre brush.
        TryApplyDig(hitChunk, digPosition);

        if (hitChunk.neighborhood != null)
        {
            foreach (ChunkSVT neighbor in hitChunk.neighborhood)
            {
                TryApplyDig(neighbor, digPosition);
            }
        }

        // Collect every chunk whose mesh depends on
        // voxels belonging to a changed chunk.
        foreach (ChunkSVT chunk in changedChunks)
        {
            rebuildChunks.Add(chunk);

            if (chunk.neighborhood == null)
                continue;

            // These neighbors read border voxel values
            // from this chunk in ChunkSVT's grid conversion.
            AddRebuildChunk(chunk.neighborhood[0]);
            AddRebuildChunk(chunk.neighborhood[1]);
            AddRebuildChunk(chunk.neighborhood[3]);
        }

        // Rebuild only after all voxel modifications.
        // Update grid cells near the brush instead of the entire grid.
        foreach (ChunkSVT chunk in rebuildChunks)
        {
            chunk.RebuildAfterDig(digPosition, digRadius);
        }
    }

    private void TryApplyDig(
        ChunkSVT chunk,
        Vector3 digPosition)
    {
        if (chunk == null)
            return;

        if (!BrushMayTouchChunk(chunk, digPosition))
            return;

        if (chunk.ApplyDigSphere(
            digPosition,
            digRadius,
            digStrength))
        {
            changedChunks.Add(chunk);
        }
    }

    private void AddRebuildChunk(ChunkSVT chunk)
    {
        if (chunk != null)
            rebuildChunks.Add(chunk);
    }

    private bool BrushMayTouchChunk(
        ChunkSVT chunk,
        Vector3 point)
    {
        Vector3 local = chunk.transform.InverseTransformPoint(point);

        return local.x + digRadius >= 0f &&
               local.x - digRadius <= ChunkSVT.chunkWidth &&
               local.y + digRadius >= 0f &&
               local.y - digRadius <= ChunkSVT.chunkHeight &&
               local.z + digRadius >= 0f &&
               local.z - digRadius <= ChunkSVT.chunkWidth;
    }
}