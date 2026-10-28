
using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    [Header("Flashlight")]
    [SerializeField] private Light flashlight;

    [SerializeField] private KeyCode toggleKey = KeyCode.R;

    [SerializeField] private bool startEnabled = true;

    private bool isEnabled;

    private void Start()
    {
        if (flashlight == null)
        {
            Debug.LogWarning("Flashlight reference is missing!");
            enabled = false;
            return;
        }

        isEnabled = startEnabled;
        flashlight.enabled = isEnabled;
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isEnabled = !isEnabled;
            flashlight.enabled = isEnabled;
        }
    }
}
