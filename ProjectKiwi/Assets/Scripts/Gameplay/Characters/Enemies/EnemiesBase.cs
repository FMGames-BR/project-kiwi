using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tactical;
using BehaviorDesigner.Runtime.Tactical.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesBase : CharacterBase, IAttackAgent
{
    public Animator anim;
    private UILifebar myLifebar;

    //behaviour variables
    public float attackDistance = 2f;
    public float attackDelay = 1.5f;
    private float lastAttackTime;
    public float attackAngle = 1f;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void OnBorn()
    {
        base.OnBorn();

        myLifebar = SpawnerController.instance.OnSpawnUILifebar(transform);
    }

    public override void OnHit(int damage)
    {
        if (life == 0)
            return;

        base.OnHit(damage);

        myLifebar.OnUpdateValue((float)life / (float)initialLife);
    }

    public override void OnDie()
    {
        Vector2 randomPos = Random.insideUnitCircle * 10f;
        transform.position = PlayerController.instance.transform.position + new Vector3(randomPos.x, 0, randomPos.y);

        myLifebar.OnRemove();

        OnBorn(); //temporary
    }

    //Behaviour Interface

    public float AttackDistance()
    {
        return attackDistance;
    }

    public bool CanAttack()
    {
        return lastAttackTime + attackDelay < Time.time;
    }

    public float AttackAngle()
    {
        return attackAngle;
    }

    public void Attack(Vector3 targetPosition)
    {
        //throw new System.NotImplementedException();
        lastAttackTime = Time.time;
    }
}
