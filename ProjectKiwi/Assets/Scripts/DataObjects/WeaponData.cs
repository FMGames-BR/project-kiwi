using Assets.Scripts.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Data", menuName = "ProjectKiwi/Weapons And Bullets/Weapons Data")]
public class WeaponData : ScriptableObject
{
    [Tooltip("The type of the weapon")]
    public Weapon type;
    [Tooltip("The maximum distance of the shot or throw")]
    public Vector2 shotForce;
    [Tooltip("The delay between shots")]
	public float delayBetweenShots = 5f;
    [Tooltip("The number of bullets of a shot, use the gun magazine to spawn at once")]
    public int numberOfBullets = 16; // shotgun
    [Tooltip("The total points to draw the line renderer")]
    public int linePoints = 2;

	//TODO: Change to BulletData scriptable object
	public float bulletSpeed;
    public float spreadHorizontal = 30; //// 0 to 360 degrees to spread the bullets 
    public float spreadVertical = 0;
    public Assets.Scripts.Enums.SpreadType spreadType = Assets.Scripts.Enums.SpreadType.Random;
    public float bulletLifeRange { get { return shotForce.x; } }
}
