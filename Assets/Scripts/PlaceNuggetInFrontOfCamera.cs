using UnityEngine;

public class PlaceNuggetInFrontOfCamera : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;

    private void Start()
    {
        if (playerCamera == null)
        {
            Debug.LogError("Player Camera is not assigned!");
            return;
        }

        transform.position =
            playerCamera.transform.position +
            playerCamera.transform.forward * 2f;

        Debug.Log($"Nugget positioned at {transform.position}");
    }
}