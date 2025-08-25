using Game.StateMachineHandling;
using UnityEngine;

namespace Game.Gameplay.EnemyManagement
{
    public class MachineGunEnemyFireState : IState
    {
        private MachineGunEnemy enemy;

        public MachineGunEnemyFireState(MachineGunEnemy enemy)
        {
            this.enemy = enemy;
        }

        public void OnEnter()
        {
            this.enemy.ApplyBrakes();
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
            this.enemy.UnapplyBrakes();
        }
    }
}
