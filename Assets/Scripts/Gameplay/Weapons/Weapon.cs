using UnityEngine;

namespace Game.Gameplay.Weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        public abstract WeaponType GetWeaponType();
        public abstract void Activate();
        public abstract void Deactivate();
        public abstract void Fire();
    }
}
