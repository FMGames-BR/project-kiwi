using UnityEngine;
using Gameplay.Bullets;
using Assets.Scripts.Enums;

namespace Gameplay.Weapons
{
    public class WeaponBase : MonoBehaviour, IWeapon
    {
        public WeaponData data;

        [Header("Bullet")]
        public Transform bulletSpawnPoint;

        //protected and private variables
        protected bool _isFiring;

        protected virtual void Update()
        {

        }

        public virtual void OnAttack()
        {
            _isFiring = true;
        }

        public virtual void OnAim(LineRenderer lineRenderer, Transform weaponPosition, Transform lookAtPoint)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Spawn weapon bullets and spread
        /// </summary>
        protected virtual void OnSpawnBullets()
        {
            var totalSpread = data.spreadHorizontal / data.numberOfBullets;
            var spreadHalf = data.spreadHorizontal / 2f;
            for (var i = 0; i < data.numberOfBullets; i++)
            {
                float spreadX = 0;
                float spreadY = 0;

                switch (data.spreadType)
                {
                    case SpreadType.Equally:
                        var spreadN = totalSpread * (i + 1);
                        spreadY = spreadHalf - spreadN + (totalSpread / 2);
                        if (i % 2 == 0)
                        {
                            spreadX = data.spreadVertical;
                        }
                        else
                        {
                            spreadX = data.spreadVertical * -1;
                        }
                        break;
                    case SpreadType.Random:
                        spreadY = Random.Range(spreadHalf * -1, spreadHalf);
                        spreadX = Random.Range((data.spreadVertical * -1), (data.spreadVertical));
                        break;
                    default:
                        break;
                }

                Quaternion rotation = Quaternion.Euler(new Vector3(spreadX,
                        spreadY + bulletSpawnPoint.eulerAngles.y, 0));

                BulletBase b = SpawnerController.instance.OnSpawnBullet(bulletSpawnPoint.position, rotation);
                b.speed = data.bulletSpeed;
                b.lifeRange = data.bulletLifeRange;
            }
        }
    }
}