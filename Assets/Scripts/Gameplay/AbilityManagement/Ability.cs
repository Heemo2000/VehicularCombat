using UnityEngine;

namespace Game.Gameplay.AbilityManagement
{

    [System.Serializable]
    public abstract class Ability
    {
        [Min(0.0f)]
        public float coolDownTime = 8.0f;

        [SerializeField]protected bool coolDownHappening = false;

        public abstract void OnEquip();
        public abstract void OnAim(Vector3 aimPosition);
        public abstract void Execute();
        public abstract void OnWithHold();
        public bool IsReady()
        {
            return coolDownHappening == false;
        }

    }
}
