using UnityEngine;

public class EnemyPatrolState : EnemyBaseState
{
    private Transform pointA;        // First patrol point
    private Transform pointB;        // Second patrol point
    float patrolSpeed = 2f;  // Speed of patrol movement
    private Transform targetPoint;  // The current target patrol point
    private float waitTimer = 4f;
    private bool isTimerActive;

    private Transform player;        // Reference to the player’s transform
    private float sightRange = 10f;  // Range of sight
    private float fieldOfViewAngle = 100f; // FOV of the enemy

    private GameObject light;
    private Animator anim;

    public override void EnterState(EnemyStateManager enemy)
    {
        light = GameObject.Find("EnemyLight");
        anim = light.GetComponent<Animator>();
        anim.SetBool("isPatrol", true);

        GameObject patrolSpotA = GameObject.Find("PatrolPointA");
        pointA = patrolSpotA.transform;
        GameObject patrolSpotB = GameObject.Find("PatrolPointB");
        pointB = patrolSpotB.transform;
        targetPoint = pointA;

        GameObject playerLocation = GameObject.FindGameObjectWithTag("Player");
        player = playerLocation.transform;
    }

    public override void UpdateState(EnemyStateManager enemy)
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
                waitTimer = 3f; // Reset the timer to 4 seconds
            }
        }
        // If the timer is active, start counting down
        if (isTimerActive)
        {
            waitTimer -= Time.deltaTime; // Decrease timer by the time passed each frame

            // When the timer reaches 0, switch the target point and reset the timer
            if (waitTimer <= 0f)
            {
                // Toggle between point A and point B
                targetPoint = (targetPoint == pointA) ? pointB : pointA;

                // Reset timer and stop the countdown
                isTimerActive = false;
                waitTimer = 4f; // Reset timer value
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
                    Debug.DrawRay(enemy.transform.position, directionToPlayer.normalized * hit.distance, Color.red);
                    if (hit.transform == player)
                    {
                        anim.SetBool("isPatrol", false);
                        enemy.SwitchState(enemy.chaseState);
                    }
                }
            }
        }
    }

    public override void OnCollisionEnter(EnemyStateManager enemy)
    {

    }
}
