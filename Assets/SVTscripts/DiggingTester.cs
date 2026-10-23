
using UnityEngine;

public class DiggingTester : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float digRange = 100f;
    [SerializeField] private float digRadius = 2f;
    [SerializeField] private float digStrength = 256f;
    [SerializeField] private float digCooldown = 0.25f;

    private float nextDigTime;

    private void Update()
    {
        // Dig continuously while left mouse is held.
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

        // Shoot a ray from the camera through the mouse cursor.
        Ray ray = targetCamera.ScreenPointToRay(
            Input.mousePosition
        );

        if (Physics.Raycast(ray, out RaycastHit hit, digRange))
        {
            ChunkSVT chunk =
                hit.collider.GetComponentInParent<ChunkSVT>();

            if (chunk != null)
            {
                // Move the brush slightly into the surface.
                Vector3 digPosition =
                    hit.point - hit.normal * 0.5f;

                chunk.DigSphere(
                    digPosition,
                    digRadius,
                    digStrength
                );

                Debug.Log("Digging at " + digPosition);
            }
        }
    }
}

