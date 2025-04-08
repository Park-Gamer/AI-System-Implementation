using UnityEngine;

public class EnemyPatrolState : EnemyBaseState
{
    private Transform pointA;        // First patrol point
    private Transform pointB;        // Second patrol point
    private Transform pointC;        // Third patrol point
    private Transform pointD;        // Forth patrol point

    float patrolSpeed = 3f;         // Speed of patrol movement
    private Transform targetPoint;  // The current target patrol point
    private float waitTimer = 2f;   // Time the guard waits
    private bool isTimerActive;

    private Transform player;        // Reference to the player’s transform
    private float sightRange = 10f;  // Range of sight
    private float fieldOfViewAngle = 100f; // FOV of the enemy

    private GameObject light;
    private Animator anim;
    private int layerIndex;
    private LayerMask layerMask;

    public override void EnterState(EnemyStateManager enemy)
    {
        light = GameObject.Find("EnemyLight");
        anim = light.GetComponent<Animator>();
        anim.SetBool("isPatrol", true);

        GameObject patrolSpotA = GameObject.Find("PatrolPointA");
        pointA = patrolSpotA.transform;
        GameObject patrolSpotB = GameObject.Find("PatrolPointB");
        pointB = patrolSpotB.transform;
        GameObject patrolSpotC = GameObject.Find("PatrolPointC"); 
        pointC = patrolSpotC.transform;
        GameObject patrolSpotD = GameObject.Find("PatrolPointD"); 
        pointD = patrolSpotD.transform;
        targetPoint = pointA;

        GameObject playerLocation = GameObject.FindGameObjectWithTag("Player");
        player = playerLocation.transform;
        // Convert the layer name to its corresponding layer index
        layerIndex = LayerMask.NameToLayer("Obstacle");
        // Use LayerMask.GetMask() to create a bitmask for the layer
        layerMask = 1 << layerIndex;
    }

    public override void UpdateState(EnemyStateManager enemy)
    {
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
                enemy.transform.position += currentDirection * patrolSpeed * Time.deltaTime;
            }
        }
        else
        {
            // Move the enemy towards the target point
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, targetPoint.position, patrolSpeed * Time.deltaTime);

            Vector3 direction = (targetPoint.position - enemy.transform.position).normalized;
            if (direction != Vector3.zero) // Ensure we have a valid direction
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation, targetRotation, 200f * Time.deltaTime);
            }

            // If the enemy reaches the target point, switch to the other point
            if (Vector3.Distance(enemy.transform.position, targetPoint.position) < 0.1f)
            {
                if (!isTimerActive)
                {
                    // Start the timer countdown when the enemy is close enough
                    isTimerActive = true;
                    waitTimer = 2f; // Reset the timer to 4 seconds
                }
            }
            // If the timer is active, start counting down
            if (isTimerActive)
            {
                waitTimer -= Time.deltaTime; // Decrease timer by the time passed each frame

                // When the timer reaches 0, switch the target point and reset the timer
                if (waitTimer <= 0f)
                {
                    // Toggle between the points in a cyclic order (A -> B -> C -> D -> A)
                    if (targetPoint == pointA)
                        targetPoint = pointB;
                    else if (targetPoint == pointB)
                        targetPoint = pointC;
                    else if (targetPoint == pointC)
                        targetPoint = pointD;
                    else
                        targetPoint = pointA;

                    // Reset timer and stop the countdown
                    isTimerActive = false;
                    waitTimer = 2f; // Reset timer value
                }
            }

            // Vector from enemy to player
            Vector3 directionToPlayer = player.position - enemy.transform.position;

            // Check if the player is within the field of view angle
            float angle = Vector3.Angle(directionToPlayer, enemy.transform.forward);
            if (angle < fieldOfViewAngle / 2)
            {
                // Check if the player is within sight range
                if (directionToPlayer.magnitude < sightRange)
                {
                    // Raycast to check if there is any obstacle between the enemy and the player
                    RaycastHit hit;
                    if (Physics.Raycast(enemy.transform.position, directionToPlayer.normalized, out hit, sightRange))
                    {
                        Debug.DrawRay(enemy.transform.position, Vector3.forward * hit.distance, Color.red);
                        if (hit.transform == player)
                        {
                            anim.SetBool("isPatrol", false);
                            enemy.SwitchState(enemy.chaseState);
                        }
                    }
                }
            }
        }
    }
}
