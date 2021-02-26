using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesBase : MonoBehaviour, ICharacter
{
    public Animator anim;

    //Interface call
    public void OnBorn()
    {
        throw new System.NotImplementedException();
    }

    public void OnDie()
    {
        throw new System.NotImplementedException();
    }

    public void OnHit()
    {
        throw new System.NotImplementedException();
    }

    public void OnWalk()
    {
        throw new System.NotImplementedException();
    }
}
