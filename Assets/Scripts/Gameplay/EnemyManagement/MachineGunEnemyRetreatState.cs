using Game.StateMachineHandling;
using UnityEngine;

namespace Game.Gameplay.EnemyManagement
{
    public class MachineGunEnemyRetreatState : IState
    {
        private MachineGunEnemy enemy;
        private bool isCooldownStarted = false;
        private float currentTime = 0.0f;
        
        public MachineGunEnemyRetreatState(MachineGunEnemy enemy)
        {
            this.enemy = enemy;
        }

        public bool IsCooldownCompleted()
        {
            if (isCooldownStarted)
            {
                if(currentTime < this.enemy.RetreatCooldownTime)
                {
                    currentTime += Time.deltaTime;
                    return false;
                }

                currentTime = 0.0f;
                isCooldownStarted = false;
                return true;
            }
            return false;
        }

        public void OnEnter()
        {
            this.enemy.UnapplyBrakes();
            this.enemy.SetSpeed(this.enemy.RetreatSpeed);
        }

        public void OnUpdate()
        {

        }

        public void OnFixedUpdate()
        {

        }

        public void OnLateUpdate()
        {

        }

        public void OnExit()
        {
            isCooldownStarted=true;
        }
    }
}
