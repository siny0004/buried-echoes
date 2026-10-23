using System.Collections;
using System.Collections.Generic;
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

    // Shooting
    public Camera fpsCam;
    public float range = 100000f;
    public int weaponIndex = 0;
    public readonly string[] weaponNames2 = { "One voxel -20", "WEAPON NOT CREATED", "WEAPON NOT CREATED", "WEAPON NOT CREATED", "WEAPON NOT CREATED" };

    /// <summary>
    /// Shooting
    /// </summary>
    public void Shoot()
    {
        if (weaponIndex == 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
            {
                ChunkSVT hitChunk = hit.transform.GetComponent<ChunkSVT>();
                if (hitChunk != null)
                {
                    // IMPROVE  - zawsze jest brany pierwszy chunk !!! ... no i na testy jeszcze dac wyswietlanie sąsiadów postrzelonego chunka
                    EditSVT.ChangeVoxelValue(-20, hitChunk, hit.point);
                    /*for(int i = 0; i < 8; i++)
                    {
                        if(hitChunk.neighborhood[i] == null)
                            Debug.Log("i[" + i + "] = NULL");
                        else
                            Debug.Log("i[" + i + "] != NULL");
                    }*/
                }
            }
        }
        else if (weaponIndex == 1)
        {
            RaycastHit hit;
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
            {
                ChunkSVT hitChunk = hit.transform.GetComponent<ChunkSVT>();
                if (hitChunk != null)
                {
                    EditSVT.ChangeVoxelValue(20, hitChunk, hit.point);
                }
            }
        }
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
        if (transform.position.y < -10f)
        {
            transform.position = new Vector3(
                transform.position.x,
                10f,
                transform.position.z
            );

            velocity = Vector3.zero;
        }

        // Original terrain editing remains disabled.
        // DiggingTester handles excavation instead.

        // Original weapon selection
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weaponIndex = 0;
            Debug.Log("Chosen weapon: " + weaponNames2[0]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            weaponIndex = 1;
            Debug.Log("Chosen weapon: " + weaponNames2[1]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            weaponIndex = 2;
            Debug.Log("Chosen weapon: " + weaponNames2[2]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            weaponIndex = 3;
            Debug.Log("Chosen weapon: " + weaponNames2[3]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            weaponIndex = 4;
            Debug.Log("Chosen weapon: " + weaponNames2[4]);
        }
    }
}
