using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{
    private GameObject light;
    private Animator anim;
    public override void EnterState(EnemyStateManager enemy)
    {
        light = GameObject.Find("EnemyLight");
        anim = light.GetComponent<Animator>();
        anim.SetBool("isChase", true);
    }

    public override void UpdateState(EnemyStateManager enemy)
    {

    }

    public override void OnCollisionEnter(EnemyStateManager enemy)
    {

    }
}
