using UnityEngine;

public class AmbientAudioController : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource surfaceAudio;
    [SerializeField] private AudioSource undergroundAudio;

    [Header("Depth Settings")]
    [SerializeField] private float surfaceY = 5f;
    [SerializeField] private float fullUndergroundDepth = 25f;

    [Header("Maximum Volumes")]
    [Range(0f, 1f)]
    [SerializeField] private float surfaceVolume = 0.6f;

    [Range(0f, 1f)]
    [SerializeField] private float undergroundVolume = 0.25f;

    private void Start()
    {
        if (surfaceAudio != null)
        {
            surfaceAudio.loop = true;
            surfaceAudio.volume = 0f;
            surfaceAudio.Play();
        }

        if (undergroundAudio != null)
        {
            undergroundAudio.loop = true;
            undergroundAudio.volume = 0f;
            undergroundAudio.Play();
        }
    }

    private void Update()
    {
        // Calculate how deep the player is below the surface.
        float depth = Mathf.Max(0f, surfaceY - transform.position.y);

        // 0 = surface, 1 = fully underground.
        float t = Mathf.Clamp01(
            depth / Mathf.Max(0.01f, fullUndergroundDepth)
        );

        // Smoothly blend between the two environments.
        t = Mathf.SmoothStep(0f, 1f, t);

        if (surfaceAudio != null)
        {
            surfaceAudio.volume = (1f - t) * surfaceVolume;
        }

        if (undergroundAudio != null)
        {
            undergroundAudio.volume = t * undergroundVolume;
        }
    }
}