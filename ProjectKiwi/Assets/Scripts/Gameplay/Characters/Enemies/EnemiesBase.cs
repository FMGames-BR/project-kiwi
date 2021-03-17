using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesBase : CharacterBase
{
    public Animator anim;
    private BehaviorTree behaviorTree;
    private UILifebar myLifebar;

    protected override void Awake()
    {
        base.Awake();
        behaviorTree = GetComponent<BehaviorTree>();
    }

    public void OnAttack()
    {
        // Debug.Log("Enemy is attacking..");
    }

    public override void OnBorn()
    {
        base.OnBorn();

        myLifebar = SpawnerController.instance.OnSpawnUILifebar(transform);
    }

    public override void OnHit(int damage)
    {
        base.OnHit(damage);

        myLifebar.OnUpdateValue((float)life / (float)initialLife);
    }

    public override void OnDie()
    {
        GameObject target = behaviorTree.FindTask<Seek>().target.Value;
        Vector2 randomPos = Random.insideUnitCircle * 10f;
        transform.position = target.transform.position + new Vector3(randomPos.x, 0, randomPos.y);

        myLifebar.OnRemove();

        OnBorn();
    }
}
