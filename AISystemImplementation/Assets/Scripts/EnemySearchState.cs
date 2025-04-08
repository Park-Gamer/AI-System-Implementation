using TMPro;
using UnityEditor;
using UnityEngine;

public class EnemySearchState : EnemyBaseState
{
    private Transform player;   // Reference to the player’s transform
    private float searchSpeed = 3f; // Speed at which the enemy moves during the search
    private float rotationSpeed = 100f; // Speed of rotation when checking for the player
    private float raycastDistance = 10f; // Distance of the raycast check
    private LayerMask playerLayer; // The layer mask to detect the player
    private float searchTimeout = 10f; // Timeout before giving up the search
    private Vector3 lastKnownPosition; // The last known position of the player
    private float searchTimer = 0f; // Timer to check when to stop searching
    private bool hasFoundPlayer = false; // Flag to track if the player is detected

    private GameObject light;
    private Animator anim;
    private int layerIndex;
    private LayerMask layerMask;

    public override void EnterState(EnemyStateManager enemy)
    {
        light = GameObject.Find("EnemyLight");
        anim = light.GetComponent<Animator>();
        anim.SetBool("isSearch", true);

        GameObject playerLocation = GameObject.FindGameObjectWithTag("Player");
        player = playerLocation.transform;

        lastKnownPosition = player.position;
        searchTimer = 0f;
        hasFoundPlayer = false;
        // Convert the layer name to its corresponding layer index
        layerIndex = LayerMask.NameToLayer("Obstacle");
        // Use LayerMask.GetMask() to create a bitmask for the layer
        layerMask = 1 << layerIndex;
    }

    public override void UpdateState(EnemyStateManager enemy)
    {
        // Check if the search timer has expired
        if (searchTimer > searchTimeout)
        {
            anim.SetBool("isSearch", false);
            enemy.SwitchState(enemy.patrolState);
            return;
        }

        // Move towards the last known position of the player
        MoveTowardsLastKnownPosition(enemy);

        if (Vector3.Distance(enemy.transform.position, lastKnownPosition) < 0.1f)
        {
            // Rotate 360 degrees to check for the player using a raycast
            RaycastForPlayer(enemy);
        }

        // If the player is detected, change state (for example, attack or chase)
        if (hasFoundPlayer)
        {
            anim.SetBool("isSearch", false);
            enemy.SwitchState(enemy.chaseState);
        }

        // Increment the search timer
        searchTimer += Time.deltaTime;
    }

    // Move towards the last known position of the player
    private void MoveTowardsLastKnownPosition(EnemyStateManager enemy)
    {
        // Move the enemy towards the last known position
        Vector3 direction = (lastKnownPosition - enemy.transform.position).normalized;
        enemy.transform.position += direction * searchSpeed * Time.deltaTime;

        // Vector from enemy to player
        Vector3 directionToPlayer = player.position - enemy.transform.position;
        // Check if the player is within the field of view angle
        float angle = Vector3.Angle(directionToPlayer, enemy.transform.forward);
        if (angle < 140f / 2)
        {
            // Check if the player is within sight range
            if (directionToPlayer.magnitude < 16f)
            {
                // Raycast to check if there is any obstacle between the enemy and the player
                RaycastHit hit;
                if (Physics.Raycast(enemy.transform.position, directionToPlayer.normalized, out hit, 16f))
                {
                    if (hit.transform == player)
                    {
                        anim.SetBool("isSearch", false);
                        enemy.SwitchState(enemy.chaseState);
                    }
                }
            }
        }

        RaycastHit hitInfo;
        // Cast a ray to check for obstacles in front of the enemy
        if (Physics.Raycast(enemy.transform.position, enemy.transform.forward, out hitInfo, 2f, layerMask))
        {
            if (hitInfo.collider.CompareTag("Obstacle"))
            {
                // Calculate direction away from the obstacle
                Vector3 directionAway = (enemy.transform.position - hitInfo.point).normalized;

                // Move the enemy in the opposite direction of the obstacle
                Vector3 currentDirection = (directionAway + new Vector3(2, 0, -5)).normalized;

                // Apply the movement to the enemy
                enemy.transform.position += currentDirection * searchSpeed * Time.deltaTime;
            }
        }
    }

    // Rotate 360 degrees and use a raycast to check for the player
    private void RaycastForPlayer(EnemyStateManager enemy)
    {
        // Rotate the enemy around Y axis to simulate searching
        enemy.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Perform a raycast to check if the player is in sight
        RaycastHit hit;
        if (Physics.Raycast(enemy.transform.position, enemy.transform.forward, out hit, raycastDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                // Player detected
                hasFoundPlayer = true;
            }
        }
    }
}
