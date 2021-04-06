using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour, ICharacter, IHittableObject
{
    public int life = 100;
    protected int initialLife;

    protected virtual void Awake()
    {
        initialLife = life;
    }

    private void Start()
    {
        OnBorn(); //temporary call, should be called on spawner controller
    }

    //Interface Call
    public virtual void OnBorn()
    {
        OnReset();
    }

    public virtual void OnDie()
    {
        
    }


    public virtual void OnWalk()
    {
        throw new System.NotImplementedException();
    }

    public void OnReset()
    {
        life = initialLife;
    }

    public virtual void OnHit(int damage)
    {
        life -= damage;

        life = Mathf.Clamp(life, 0, initialLife);

        if (life == 0)
            OnDie();
    }
}
