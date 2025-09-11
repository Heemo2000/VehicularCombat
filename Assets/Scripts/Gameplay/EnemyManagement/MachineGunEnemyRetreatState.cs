using Game.StateMachineHandling;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Gameplay.EnemyManagement
{
    public class MachineGunEnemyRetreatState : IState
    {
        private MachineGunEnemy enemy;
        private bool isCooldownStarted = false;
        private float currentTime = 0.0f;
        private NavMeshPath path;
        private int currentWaypointIndex = -1;
        private Transform closestPoint = null;
        private Coroutine retreatCoroutine;
        private NavMeshHit hit;

        public MachineGunEnemyRetreatState(MachineGunEnemy enemy)
        {
            this.enemy = enemy;
            path = new NavMeshPath();
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
            if (this.enemy.RetreatPoints.Length == 0)
            {
                return;
            }

            closestPoint = null;
            float sqrDistance = float.MaxValue;

            foreach(var point in this.enemy.RetreatPoints)
            {
                float currentSqrDistance = Vector3.SqrMagnitude(point.position - this.enemy.transform.position);
                if(sqrDistance > currentSqrDistance)
                {
                    sqrDistance = currentSqrDistance;
                    closestPoint = point;
                }
            }

            retreatCoroutine = this.enemy.StartCoroutine(Retreat());
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
    
        public IEnumerator Retreat()
        {
            currentWaypointIndex = 0;
            Vector3 destination = this.closestPoint.position;

            NavMesh.CalculatePath(this.enemy.transform.position, this.closestPoint.position, NavMesh.AllAreas, path);

            while(currentWaypointIndex < this.path.corners.Length)
            {
                Vector3 currentWaypoint = this.path.corners[currentWaypointIndex];
                float waypointCheckDistanceSqr = this.enemy.WaypointCheckDistance * this.enemy.WaypointCheckDistance;

                if (Vector3.SqrMagnitude(currentWaypoint - this.enemy.transform.position) > waypointCheckDistanceSqr)
                {
                    this.enemy.UnapplyBrakes();
                    this.enemy.GoToPoint(currentWaypoint);
                }
                else
                {
                    this.enemy.ApplyBrakes();
                    currentWaypointIndex++;
                }
                yield return null;
            }
        }
    }
}
