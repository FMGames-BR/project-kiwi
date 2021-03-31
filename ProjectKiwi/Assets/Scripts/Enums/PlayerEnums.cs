using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Enums
{
	public enum PlayerWeapon
	{
		None, //for mobile only
		PrimaryWeapon,
		Shotgun,
		Grenade,
		SpecialSkill
	}

	public enum PlayerAttackType
	{
		Shoot,
		Throw
	}
}