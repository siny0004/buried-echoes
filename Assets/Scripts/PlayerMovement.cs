using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Controller reference
    public CharacterController controller;

    // Physics parameters
    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    private Vector3 velocity;
    public bool ControlsLocked { get; set; }

    // Rope ascender
    [Header("Rope Ascender")]
    public KeyCode ropeKey = KeyCode.T;

    [SerializeField] private int ropeLevel = 1;

    private readonly float[] ropeSpeeds =
    {
        3f, 5f, 7f, 9f, 12f
    };

    private readonly int[] ropeUpgradeCosts =
    {
        50, 100, 250, 500
    };

    public int RopeLevel => ropeLevel;

    public int MaxRopeLevel => ropeSpeeds.Length;

    public float RopeSpeed => ropeSpeeds[
        Mathf.Clamp(ropeLevel - 1, 0, ropeSpeeds.Length - 1)
    ];

    [Header("Rope Audio")]
    [SerializeField] private AudioSource ropeAudioSource;

    // Ground checking (for jumping)
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private bool isGrounded;

    public bool CanUpgradeRope()
    {
        return ropeLevel < MaxRopeLevel;
    }

    public int GetRopeUpgradeCost()
    {
        if (!CanUpgradeRope())
            return 0;

        return ropeUpgradeCosts[ropeLevel - 1];
    }

    public bool UpgradeRope()
    {
        if (!CanUpgradeRope())
            return false;

        ropeLevel++;
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = isGrounded ? Color.green : Color.red;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundDistance
        );
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        if (ControlsLocked)
        {
            if (ropeAudioSource != null && ropeAudioSource.isPlaying)
            {
                ropeAudioSource.Stop();
            }

            return;
        }

        // CharacterController reports whether the last
        // movement touched walkable ground.
        isGrounded = controller.isGrounded;

        // Prevent downward velocity from accumulating
        // while standing on the ground.
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        // Horizontal movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move =
            transform.right * x +
            transform.forward * z;

        // Jump only if grounded.
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = Mathf.Sqrt(
                jumpHeight * -2f * gravity
            );
        }

        // Rope ascender
        if (Input.GetKey(ropeKey))
        {
            velocity.y = RopeSpeed;

            // Start the rope sound when ascending.
            if (ropeAudioSource != null && !ropeAudioSource.isPlaying)
            {
                ropeAudioSource.Play();
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;

            // Stop the rope sound when T is released.
            if (ropeAudioSource != null && ropeAudioSource.isPlaying)
            {
                ropeAudioSource.Stop();
            }
        }

        // Combine horizontal and vertical movement
        // into one CharacterController.Move call.
        Vector3 movement =
            move * speed + Vector3.up * velocity.y;

        controller.Move(movement * Time.deltaTime);

        // Temporary fall protection.
        // TODO: Replace with a proper world-boundary respawn.
        if (transform.position.y < -65f)
        {
            transform.position = new Vector3(
                transform.position.x,
                10f,
                transform.position.z
            );

            velocity = Vector3.zero;
        }
    }
}
