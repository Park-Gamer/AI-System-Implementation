using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;  // Movement speed of the player
    public float rotationSpeed = 200f;
    private Rigidbody rb;  // Rigidbody of the player

    void Start()
    {
        rb = GetComponent<Rigidbody>();  // Get the Rigidbody component attached to the player
    }

    private void Update()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        Vector2 move = new Vector2 (horizontal, vertical);

        // Only rotate if there is some direction
        if (move.magnitude > 0)
        {
            // Calculate the angle in radians from the Vector2 (the direction vector)
            float angle = Mathf.Atan2(move.x, move.y) * Mathf.Rad2Deg;

            // Smoothly rotate the player to the desired angle
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        // Calculate the movement direction
        Vector3 moveDirection = new Vector3(move.x, 0f, move.y).normalized;
        // Move the player
        MovePlayer(moveDirection);
    }

    void MovePlayer(Vector3 direction)
    {
        // Apply the movement to the Rigidbody
        rb.velocity = direction * moveSpeed;  // Move the player based on the input direction and speed
    }
}
