using Game.StateMachineHandling;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Game.Core;

namespace Game.Gameplay.EnemyManagement
{
    public class MachineGunEnemyChaseState : IState
    {
        private MachineGunEnemy enemy;
        private Coroutine updateWaypointsCoroutine;
        private NavMeshPath path;
        private int currentWaypointIndex = -1;

        public MachineGunEnemyChaseState(MachineGunEnemy enemy)
        {
            this.enemy = enemy;
            path = new NavMeshPath();
        }

        public void OnEnter()
        {
            this.enemy.UnapplyBrakes();
            this.enemy.SetSpeed(this.enemy.ChaseSpeed);
            currentWaypointIndex = 0;
            updateWaypointsCoroutine = this.enemy.StartCoroutine(UpdateWaypoints());
        }

        public void OnUpdate()
        {
            //If we already reached destination, no need to drive
            if(currentWaypointIndex >= this.path.corners.Length)
            {
                return;
            }

            bool shouldChangeWayPt = Utility.IsUnderDistance(this.path.corners[currentWaypointIndex], 
                                                             this.enemy.transform.position, 
                                                             this.enemy.WaypointCheckDistance);

            if (shouldChangeWayPt)
            {
                currentWaypointIndex++;
            }

            this.enemy.GoToPoint(this.path.corners[currentWaypointIndex]);
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

        private IEnumerator UpdateWaypoints()
        {
            while(this.enemy.enabled)
            {
                NavMesh.CalculatePath(this.enemy.transform.position, this.enemy.Target.position, NavMesh.AllAreas, path);
                yield return new WaitForSeconds(this.enemy.UpdateWaypointsTime);
            }
        }
    }
}
