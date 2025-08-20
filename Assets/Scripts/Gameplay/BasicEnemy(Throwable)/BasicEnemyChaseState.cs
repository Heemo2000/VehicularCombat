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

        }


        public void OnUpdate()
        {
            Vector3 moveDirection = (this.enemy.Target.position - this.enemy.transform.position).normalized;
            this.enemy.HandleMovement(moveDirection);
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
