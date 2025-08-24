using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Game.StateMachineHandling;
using Game.Core;

namespace Game.Gameplay.EnemyManagement
{
    public class MachineGunEnemyPatrolState : IState
    {
        private MachineGunEnemy enemy;
        private float currentTime = 0.0f;
        private NavMeshPath path = null;
        private Coroutine patrolCoroutine = null;
        private GameObject destinationGO = null;

        public MachineGunEnemyPatrolState(MachineGunEnemy enemy)
        {
            this.enemy = enemy;
            this.destinationGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            this.destinationGO.GetComponent<Collider>().enabled = false;
            path = new NavMeshPath();
        }

        public void OnEnter()
        {
            this.enemy.SetSpeed(this.enemy.PatrolSpeed);
            patrolCoroutine = this.enemy.StartCoroutine(Patrol());
            this.destinationGO.SetActive(true);
            Random.InitState((int)System.DateTime.Now.Ticks);
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
            this.enemy.StopCoroutine(patrolCoroutine);
            patrolCoroutine = null;
            this.destinationGO.SetActive(false);
        }

        private IEnumerator Patrol()
        {
            yield return null;
            float randomTime = Random.Range(this.enemy.MinPatrolTime, this.enemy.MaxPatrolTime);
            Vector2 random2D = Vector3.zero;
            Vector3 randomPatrolPosition = Vector3.zero;

            NavMeshHit hit;

            while (this.enemy.enabled)
            {
                currentTime = 0.0f;
                randomTime = Random.Range(this.enemy.MinPatrolTime, this.enemy.MaxPatrolTime);

                yield return null;
                //Debug.Log("Waiting for " + randomTime + " seconds");
                while (currentTime < randomTime)
                {

                    this.enemy.ApplyBrakes();
                    currentTime += Time.fixedDeltaTime;
                    yield return null;
                }

                //Debug.Log("Calculating random patrol position");

                random2D = Random.insideUnitCircle;
                randomPatrolPosition = this.enemy.transform.position +
                                               new Vector3(random2D.x, 0.0f, random2D.y) *
                                               this.enemy.PatrolRange;

                while (!NavMesh.SamplePosition(randomPatrolPosition, out hit, 1.0f, 1 << Constants.NAVMESH_WALKABLE))
                {
                    Debug.Log("Finding random patrol position");
                    random2D = Random.insideUnitCircle;
                    randomPatrolPosition = this.enemy.transform.position +
                                               new Vector3(random2D.x, 0.0f, random2D.y) *
                                               this.enemy.PatrolRange;

                    //this.destinationGO.transform.position = randomPatrolPosition;
                    yield return null;
                }

                //randomPatrolPosition = hit.position;
                this.destinationGO.transform.position = randomPatrolPosition;

                //Debug.Log("Calculating path");
                NavMesh.CalculatePath(this.enemy.transform.position, randomPatrolPosition, NavMesh.AllAreas, path);

                int pathIndex = 0;

                //Debug.Log("Now moving on a path");
                while (pathIndex < this.path.corners.Length)
                {
                    this.enemy.UnapplyBrakes();
                    Vector3 currentWaypoint = this.path.corners[pathIndex];
                    Debug.DrawLine(this.enemy.transform.position, currentWaypoint, Color.red);
                    Utility.DrawPath(path, Color.red);
                    float waypointCheckDistanceSqr = this.enemy.WaypointCheckDistance * this.enemy.WaypointCheckDistance;
                    if (Vector3.SqrMagnitude(currentWaypoint - this.enemy.transform.position) > waypointCheckDistanceSqr)
                    {
                        this.enemy.UnapplyBrakes();
                        this.enemy.GoToPoint(currentWaypoint);
                    }
                    else
                    {
                        this.enemy.ApplyBrakes();
                        pathIndex++;
                    }

                    yield return null;
                }

            }




        }
    }
}
