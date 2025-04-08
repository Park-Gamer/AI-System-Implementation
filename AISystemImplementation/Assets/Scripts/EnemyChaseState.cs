using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{
    private Transform player;   // Reference to the player’s transform
    float chaseSpeed = 4.5f;    // Speed of chase movement

    private GameObject light;
    private Animator anim;
    public override void EnterState(EnemyStateManager enemy)
    {
        light = GameObject.Find("EnemyLight");
        anim = light.GetComponent<Animator>();
        anim.SetBool("isChase", true);

        GameObject playerLocation = GameObject.FindGameObjectWithTag("Player");
        player = playerLocation.transform;
    }

    public override void UpdateState(EnemyStateManager enemy)
    {
        // Move the enemy towards the player
        enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, player.position, chaseSpeed * Time.deltaTime);
        Vector3 direction = (player.position - enemy.transform.position).normalized;
        if (direction != Vector3.zero) // Ensure we have a valid direction
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation, targetRotation, 200f * Time.deltaTime);
        }

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
                        return;
                    }
                    else
                    {
                        anim.SetBool("isChase", false);
                        enemy.SwitchState(enemy.searchState);
                    }
                }
            }
        }
    }
}
