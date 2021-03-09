using Gameplay.Bullets;
using UnityEngine;

namespace Gameplay.Weapons
{
    public class GunBase : WeaponBase
    {
        private bool _isFiring;
        private float _shotCounter;
        public float timeBetweenShots = 5f;
        public BulletBase bulletBase;
        public float bulletSpeed;
        public Transform bulletSpawnPoint;

        public float bulletsSpread = 30; //// 0 to 360 degrees to spread the bullets 

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

                    float totalSpread = bulletsSpread / numberOfBullets;
                    for (var i = 0; i < numberOfBullets; i++)
                    {
                        // spread equally
                        var spreadA = totalSpread * (i + 1);
                        var spreadB = bulletsSpread / 2f;
                        var spread = spreadB - spreadA + (totalSpread / 2);
                        var rotation = Quaternion.Euler(new Vector3(0,
                             spread + bulletSpawnPoint.eulerAngles.y, 0));

                        // spread randomly
                        //var spreadA = bulletsSpread / 2;
                        //var spread = Random.Range(spreadA*-1, spreadA);
                        //var rotation = Quaternion.Euler(new Vector3(Random.Range(-2.5f, 2.5f),
                        //    spread + bulletSpawnPoint.eulerAngles.y,0));
                        //
                        
                        var b = SpawnerController.instance.OnSpawnBullet(bulletSpawnPoint.position, rotation);
                        b.speed = bulletSpeed;

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