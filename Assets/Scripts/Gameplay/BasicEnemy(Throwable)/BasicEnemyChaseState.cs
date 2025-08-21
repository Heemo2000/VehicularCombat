using UnityEngine;
using Game.StateMachineHandling;

namespace Game.Gameplay
{
    public class BasicEnemyChaseState : IState
    {
        private BasicEnemy enemy = null;

        public BasicEnemyChaseState(BasicEnemy enemy)
        {
            this.enemy = enemy;
        }

        public void OnEnter()
        {
            this.enemy.UnapplyBrakes();
            this.enemy.SetSpeed(this.enemy.ChaseSpeed);
        }


        public void OnUpdate()
        {
            this.enemy.HandleMovement(this.enemy.Target.position);
        }

        public void OnFixedUpdate()
        {

        }

        public void OnLateUpdate()
        {

        }

        public void OnExit()
        {

        }
    }
}
