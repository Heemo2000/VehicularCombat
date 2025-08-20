using UnityEngine;
using Game.StateMachineHandling;

namespace Game.Gameplay
{
    public class BasicEnemyAttackState : IState
    {
        private BasicEnemy enemy = null;
        
        public BasicEnemyAttackState(BasicEnemy enemy)
        {
            this.enemy = enemy;
        }

        public void OnEnter()
        {

        }


        public void OnUpdate()
        {
            Debug.Log("Now, in attack state");
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
