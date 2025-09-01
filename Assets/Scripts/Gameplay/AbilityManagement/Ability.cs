using UnityEngine;

namespace Game.Gameplay.AbilityManagement
{
    public abstract class Ability
    {
        [Min(0.0f)]
        public float coolDownTime = 8.0f;

        protected bool coolDownHappening = false;

        public abstract void OnEquip();
        public abstract void OnBeforeExecute();
        public abstract void Execute();
        public bool IsReady()
        {
            return coolDownHappening == false;
        }

    }
}
