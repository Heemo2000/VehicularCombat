using UnityEngine;
using UnityEngine.AI;
using Game.StateMachineHandling;
using System.Collections;
namespace Game.Gameplay
{
    public class BasicEnemyPatrolState : IState
    {
        private BasicEnemy enemy = null;
        private float currentTime = 0.0f;
        private NavMeshPath path = null;
        private Coroutine patrolCoroutine = null;
        private GameObject destinationGO = null;
        public BasicEnemyPatrolState(BasicEnemy enemy)
        {
            this.enemy = enemy;
            this.destinationGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            this.destinationGO.GetComponent<Collider>().enabled = false;
        }
        public void OnEnter()
        {
            path = new NavMeshPath();
            patrolCoroutine = this.enemy.StartCoroutine(Patrol());
            this.destinationGO.SetActive(true);
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
            Vector2 random2D = Random.insideUnitCircle;
            Vector3 randomPatrolPosition = this.enemy.CheckTarget.position +
                                           new Vector3(random2D.x, 0.0f, random2D.y) *
                                           Random.Range(this.enemy.MinPatrolRadius, this.enemy.MaxPatrolRadius);
            NavMeshHit hit;

            while (true)
            {
                currentTime = 0.0f;
                randomTime = Random.Range(this.enemy.MinPatrolTime, this.enemy.MaxPatrolTime);

                yield return null;
                Debug.Log("Waiting for " + randomTime + " seconds");
                while (currentTime < randomTime)
                {
                    
                    this.enemy.HandleMovement(Vector3.zero);
                    currentTime += Time.fixedDeltaTime;
                    yield return null;
                }

                Debug.Log("Calculating random patrol position");

                randomPatrolPosition = this.enemy.CheckTarget.position +
                                               new Vector3(random2D.x, 0.0f, random2D.y) *
                                               Random.Range(this.enemy.MinPatrolRadius, this.enemy.MaxPatrolRadius);

                while (!NavMesh.SamplePosition(randomPatrolPosition, out hit, 1.0f, NavMesh.AllAreas))
                {
                    this.enemy.HandleMovement(Vector3.zero);
                    random2D = Random.insideUnitCircle;
                    randomPatrolPosition = this.enemy.CheckTarget.position +
                                               new Vector3(random2D.x, 0.0f, random2D.y) *
                                               Random.Range(this.enemy.MinPatrolRadius, this.enemy.MaxPatrolRadius);

                    //this.destinationGO.transform.position = randomPatrolPosition;
                    yield return null;
                }

                this.destinationGO.transform.position = randomPatrolPosition;

                Debug.Log("Calculating path");
                while(!NavMesh.CalculatePath(this.enemy.transform.position, randomPatrolPosition, NavMesh.AllAreas, path))
                {
                    yield return null;
                }

                int pathIndex = 0;

                Debug.Log("Now moving on a path");
                while(pathIndex < this.path.corners.Length)
                {
                    Vector3 currentWaypoint = this.path.corners[pathIndex];
                    Vector3 moveDirection = (currentWaypoint - this.enemy.transform.position).normalized;

                    if(Vector3.SqrMagnitude(currentWaypoint - this.enemy.transform.position) > 4.0f * 4.0f)
                    {
                        moveDirection = (currentWaypoint - this.enemy.transform.position).normalized;
                        this.enemy.HandleMovement(moveDirection);
                    }
                    else
                    {
                        pathIndex++;
                    }
                    yield return null;
                }

            }
            


            
        }
    }
}
