using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacter
{
    void OnBorn();
    void OnWalk();
    void OnHit();
    void OnDie();
}
