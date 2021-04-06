using Assets.Scripts.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bullet Data", menuName = "ProjectKiwi/Weapons And Bullets/Bullets Data")]

public class BulletData : ScriptableObject
{
    [Tooltip("The bullet travel speed")]
    public float speed;

    [Tooltip("When the bullet dies it will explode?")]
    public bool explodeWhenDie;
    [Tooltip("The power of the explosion")]
    public float explosionPower;
    [Tooltip("The radius of the explosion")]
    public float explosionRadius = 0;
}
