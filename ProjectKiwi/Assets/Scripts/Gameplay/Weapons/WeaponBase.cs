using UnityEngine;

namespace Gameplay.Weapons
{
    public class WeaponBase : MonoBehaviour, IWeapon
    {
        public virtual void OnAttack()
        {
            throw new System.NotImplementedException();
        }
    }
}