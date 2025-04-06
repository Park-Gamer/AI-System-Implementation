using UnityEngine;

public class EnemyPatrolState : EnemyBaseState
{
    private Transform pointA;        // First patrol point
    private Transform pointB;        // Second patrol point
    float patrolSpeed = 2f;  // Speed of patrol movement
    private Transform targetPoint;  // The current target patrol point

    private Transform player;        // Reference to the player’s transform
    private float sightRange = 60f;  // Range of sight
    private float fieldOfViewAngle = 180f; // FOV of the enemy

    public override void EnterState(EnemyStateManager enemy)
    {
        Debug.Log("Patrol State");

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
            // Toggle between point A and point B
            targetPoint = (targetPoint == pointA) ? pointB : pointA;
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
