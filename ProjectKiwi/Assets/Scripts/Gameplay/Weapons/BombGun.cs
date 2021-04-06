using Gameplay.Weapons;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Gameplay.Weapons
{
    public class BombGun : WeaponBase
    {
        private Vector3 _pointToLook;

        public override void OnCalculateAim(Vector3 lookingDelta)
        {
            Vector2 aimingDelta = Vector2.zero;

            if (PlayerController.instance.playerInput.currentControlScheme == "Keyboard and Mouse")
            {
                // Construct a ray from the current mouse coordinates
                Ray ray = Camera.main.ScreenPointToRay(lookingDelta);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Vector2 clickDelta = (new Vector2(hit.point.x, hit.point.z) - new Vector2(transform.position.x, transform.position.z));
                    clickDelta.x = Mathf.Clamp(clickDelta.x, -data.shotForce.x, data.shotForce.x);
                    clickDelta.y = Mathf.Clamp(clickDelta.y, -data.shotForce.x, data.shotForce.x);
                    aimingDelta = clickDelta / data.shotForce.x;
                }
            }
            else //GamePad or Touch
            {
                aimingDelta = lookingDelta;
            }

            _pointToLook = new Vector3(data.shotForce.x * aimingDelta.x, data.shotForce.y, data.shotForce.x * aimingDelta.y);
        }

        public override void OnAim(LineRenderer lineRenderer, Transform weaponPosition, Transform lookAtPoint)
        {
            if (data.linePoints == 0)
                return;

            if (!lineRenderer.enabled)
                lineRenderer.enabled = true;
            lineRenderer.gameObject.SetActive(true);

            Vector3[] points = Trajectory.OnUpdateTrajectory(transform.position, _pointToLook, data.linePoints, 0.1f);
            lineRenderer.positionCount = points.Length + 1;
            lineRenderer.SetPosition(0, weaponPosition.position);

            for (int i = 0; i < points.Length; i++)
            {
                lineRenderer.SetPosition(i + 1, points[i]);
            }
        }
	}
}
