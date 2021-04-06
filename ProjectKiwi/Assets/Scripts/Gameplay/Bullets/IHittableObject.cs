using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittableObject
{
    void OnHit(int damage);
}
