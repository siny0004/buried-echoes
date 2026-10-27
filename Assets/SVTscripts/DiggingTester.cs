
using System.Collections.Generic;
using UnityEngine;

public class DiggingTester : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [Header("Digging Effects")]
    [SerializeField] private ParticleSystem dirtParticlesPrefab;
    private ParticleSystem dirtParticlesInstance;

    [Header("Digging Audio")]
    [SerializeField] private AudioSource shovelAudioSource;
    [SerializeField] private AudioSource drillAudioSource;

    [SerializeField] private AudioClip[] shovelDigSounds;
    [SerializeField] private AudioClip drillLoopSound;

    [Header("Tool UI")]
    [SerializeField] private ToolSelectionUI toolSelectionUI;

    [Header("Tool Animation")]
    [SerializeField] private ToolAnimation shovelAnimation;
    [SerializeField] private DrillAnimation drillAnimation;

    [Header("Tool Models")]
    [SerializeField] private GameObject shovelModel;
    [SerializeField] private GameObject drillModel;

    [Header("Digging Settings")]
    [SerializeField] private float digRange = 5f;
    private DiggingToolStats toolStats = new();
    private IDiggingStrategy currentStrategy;
    private ShovelStrategy shovelStrategy;
    private DrillStrategy drillStrategy;

    private float nextDigTime;

    private readonly HashSet<ChunkSVT> changedChunks = new();
    private readonly HashSet<ChunkSVT> rebuildChunks = new();

    private void Update()
    {
        // Switch to Shovel
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentStrategy = shovelStrategy;
            RefreshToolModels();
            RefreshToolUI();
            Debug.Log("Equipped Shovel");
        }

        // Switch to Drill
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentStrategy = drillStrategy;
            RefreshToolModels();
            RefreshToolUI();
            Debug.Log("Equipped Drill");
        }

        // Dig using the currently equipped tool.
        if (Input.GetMouseButton(0) &&
            Time.time >= nextDigTime)
        {
            TryDig();
            nextDigTime = Time.time + currentStrategy.DigCooldown;
        }

        UpdateDrillAudio();

        bool isDrillingNow =
            currentStrategy == drillStrategy &&
            Input.GetMouseButton(0);

        drillAnimation?.SetDrilling(isDrillingNow);
    }

    private void Awake()
    {
        shovelStrategy = new ShovelStrategy(toolStats);
        drillStrategy = new DrillStrategy();

        // Start with the shovel equipped.
        currentStrategy = shovelStrategy;

        RefreshToolModels();
        RefreshToolUI();

        if (dirtParticlesPrefab != null)
        {
            dirtParticlesInstance = Instantiate(dirtParticlesPrefab);
            dirtParticlesInstance.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }
        if (drillAudioSource != null)
        {
            drillAudioSource.clip = drillLoopSound;
            drillAudioSource.loop = true;
            drillAudioSource.playOnAwake = false;
            drillAudioSource.Stop();
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
            chunk.RebuildAfterDig(digPosition, currentStrategy.DigRadius);
        }
        // Play the digging effect only when terrain changes.
        if (changedChunks.Count > 0)
        {
            PlayDigParticles(digPosition);
            PlayDigSound();

            if (currentStrategy == shovelStrategy)
            {
                shovelAnimation?.PlayDigAnimation();
            }
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
            currentStrategy.DigRadius,
            currentStrategy.DigStrength))
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
        float radius = currentStrategy.DigRadius;

        return local.x + radius >= 0f &&
               local.x - radius <= ChunkSVT.chunkWidth &&
               local.y + radius >= 0f &&
               local.y - radius <= ChunkSVT.chunkHeight &&
               local.z + radius >= 0f &&
               local.z - radius <= ChunkSVT.chunkWidth;
    }
    private void PlayDigParticles(Vector3 position)
    {
        if (dirtParticlesInstance == null)
            return;

        dirtParticlesInstance.transform.position = position;

        dirtParticlesInstance.Stop(
            true,
            ParticleSystemStopBehavior.StopEmittingAndClear
        );

        dirtParticlesInstance.Play();
    }
    private void PlayDigSound()
    {
        // Drill audio is handled continuously in Update().
        if (currentStrategy == drillStrategy)
            return;

        if (shovelAudioSource == null ||
            shovelDigSounds == null ||
            shovelDigSounds.Length == 0)
            return;

        AudioClip clip = shovelDigSounds[
            Random.Range(0, shovelDigSounds.Length)
        ];

        if (clip == null)
            return;

        shovelAudioSource.pitch = Random.Range(0.95f, 1.05f);
        shovelAudioSource.PlayOneShot(clip, 0.65f);
    }
    private void UpdateDrillAudio()
    {
        if (drillAudioSource == null || drillLoopSound == null)
            return;

        bool shouldPlay =
            currentStrategy == drillStrategy &&
            Input.GetMouseButton(0);

        if (shouldPlay)
        {
            if (!drillAudioSource.isPlaying)
                drillAudioSource.Play();
        }
        else if (drillAudioSource.isPlaying)
        {
            drillAudioSource.Stop();
        }
    }
    public void RefreshToolUI()
    {
        if (toolSelectionUI == null)
        {
            Debug.LogWarning("ToolSelectionUI is NOT assigned!");
            return;
        }

        Debug.Log("Refreshing Tool UI");

        toolSelectionUI.Refresh(
            currentStrategy == shovelStrategy,
            toolStats.ShovelLevel
        );
    }
    private void RefreshToolModels()
    {
        bool shovelEquipped = currentStrategy == shovelStrategy;

        if (shovelModel != null)
            shovelModel.SetActive(shovelEquipped);

        if (drillModel != null)
            drillModel.SetActive(!shovelEquipped);
    }
    public DiggingToolStats ToolStats => toolStats;
}