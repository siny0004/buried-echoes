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

    // Ground checking (for jumping)
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private bool isGrounded;

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

        // Gravity
        velocity.y += gravity * Time.deltaTime;

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
