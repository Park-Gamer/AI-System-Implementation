using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{
    private Transform player;   // Reference to the player’s transform
    float chaseSpeed = 4.2f;     // Speed of chase movement

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

        // If the enemy reaches the player
        if (Vector3.Distance(enemy.transform.position, player.position) < 1f)
        {
            Debug.Log("Player Chaught!");
        }
    }

    public override void OnCollisionEnter(EnemyStateManager enemy)
    {

    }
}
