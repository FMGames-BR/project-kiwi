using Assets.Scripts.Enums;
using Gameplay.Bullets;
using UnityEngine;

namespace Gameplay.Weapons
{
    public class GunBase : WeaponBase
    {
        private float _shotCounter;

        private void Awake()
        {
            SetWeaponAsReady();
        }


        public override void OnAim(LineRenderer lineRenderer, Transform weaponPosition, Transform lookAtPoint)
        {
            if (!lineRenderer.enabled)
                lineRenderer.enabled = true;
            lineRenderer.gameObject.SetActive(true);

            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, weaponPosition.position);

            RaycastHit _attackHit;
            if (Physics.Raycast(lookAtPoint.position, lookAtPoint.forward, out _attackHit, data.shotForce.x))
            {
                lineRenderer.SetPosition(1, _attackHit.point);
            }
            else
            {
                lineRenderer.SetPosition(1, lookAtPoint.position + lookAtPoint.forward * data.shotForce.x);
            }
        }

        public void SetWeaponAsReady()
        {
            _shotCounter = data.delayBetweenShots;
            _isFiring = false;
        }
    }

}