using Game.Gameplay.Weapons;
using UnityEngine;

namespace Game.Gameplay.AbilityManagement
{
    [System.Serializable]
    public class GrenadeAbility : Ability
    {
        public ProjectileGun projectileGun;
        public AimHandler aimHandler;
        public LineRenderer lineRenderer;
        public override void Execute()
        {
            
        }

        public override void OnAim(Vector3 aimPosition)
        {
            
        }

        public override void OnEquip()
        {
            
        }

        public override void OnWithHold()
        {
            
        }
    }
}
