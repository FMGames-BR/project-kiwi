using Assets.Scripts.Enums;
using UnityEngine;

namespace Gameplay.Weapons
{
	public interface IWeapon
	{
		void OnAim(LineRenderer lineRenderer, Transform weaponPosition, Transform lookAtPoint);
		void OnAttack();
	}

}