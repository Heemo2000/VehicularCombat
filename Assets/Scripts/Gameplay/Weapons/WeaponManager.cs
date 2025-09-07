using UnityEngine;

namespace Game.Gameplay.Weapons
{
    public class WeaponManager : MonoBehaviour
    {
        [SerializeField] private Weapon[] weapons;

        private Weapon currentWeapon = null;
        public void Activate(WeaponType type)
        {
            foreach (Weapon weapon in weapons)
            {
                weapon.Deactivate();
            }

            foreach (Weapon weapon in weapons)
            {
                if (type == weapon.GetWeaponType())
                {
                    weapon.Activate();
                    currentWeapon = weapon;
                }
            }
        }

        public void Activate(Weapon weapon)
        {
            foreach (Weapon current in weapons)
            {
                current.Deactivate();
            }

            weapon.Activate();
            currentWeapon = weapon;
        }

        public void Fire()
        {
            if(currentWeapon != null)
            {
                currentWeapon.Fire();
            }
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Activate(weapons[0]);
        }
    }
}
