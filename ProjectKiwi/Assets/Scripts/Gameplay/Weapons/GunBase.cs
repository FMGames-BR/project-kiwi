using Gameplay.Bullets;
using UnityEngine;

namespace Gameplay.Weapons
{

    public enum SpreadType
    {
        Random,
        Equally
    }
    
    public class GunBase : WeaponBase
    {
        private bool _isFiring;
        private float _shotCounter;
        public float timeBetweenShots = 5f;
        public BulletBase bulletBase;
        public float bulletSpeed;
        public Transform bulletSpawnPoint;

        public float spreadHorizontal = 30; //// 0 to 360 degrees to spread the bullets 
        public float spreadVertical = 0;
        public SpreadType spreadType = SpreadType.Random;

        public float bulletLifeRange = 5f;
        
        // shotgun 
        public int numberOfBullets = 16;

        private void Awake()
        {
            _isFiring = true;
        }


        private void Update()
        {
            if (_isFiring)
            {
                _shotCounter -= Time.deltaTime;
                if (_shotCounter <= 0)
                {
                    _shotCounter = timeBetweenShots;


                    // simple pistol
                    // Bullet b = Instantiate(bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                    // b.speed = bulletSpeed;

                    // Shotgun - spreading bullets

                    var totalSpread = spreadHorizontal / numberOfBullets;
                    var spreadHalf = spreadHorizontal / 2f;
                    for (var i = 0; i < numberOfBullets; i++)
                    {
                        float spreadX = 0;
                        float spreadY = 0;
                        switch (spreadType)
                        {
                            case SpreadType.Equally:
                                var spreadN = totalSpread * (i + 1);
                                spreadY = spreadHalf - spreadN + (totalSpread / 2);
                                if (i % 2 == 0)
                                {
                                    spreadX = spreadVertical;
                                }
                                else
                                {
                                    spreadX = spreadVertical * -1;
                                }
                                break;
                            case SpreadType.Random:
                                spreadY = Random.Range(spreadHalf*-1, spreadHalf);
                                spreadX = Random.Range((spreadVertical * -1), (spreadVertical));
                                break;
                            default:
                                break;
                        }
                        var rotation = Quaternion.Euler(new Vector3( spreadX,
                            spreadY + bulletSpawnPoint.eulerAngles.y,0));
                        
                        var b = SpawnerController.instance.OnSpawnBullet(bulletSpawnPoint.position, rotation);
                        b.speed = bulletSpeed;
                        b.lifeRange = bulletLifeRange;

                        // var b = Instantiate(bullet, bulletSpawnPoint.position, rotation);
                        // b.speed = bulletSpeed;
                    }
                }
            }
            else
            {
                _shotCounter = timeBetweenShots;
            }
        }
    }
}