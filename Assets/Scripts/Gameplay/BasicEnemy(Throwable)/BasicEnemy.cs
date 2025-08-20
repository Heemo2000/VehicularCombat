using UnityEngine;
using Game.StateMachineHandling;

namespace Game.Gameplay
{
    public class BasicEnemy : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [Min(15.0f)]
        [SerializeField] private float ignoreRotationAngle = 45.0f;
        [Min(0.01f)]
        [SerializeField] private float minPatrolTime = 2.0f;
        [Min(0.01f)]
        [SerializeField] private float maxPatrolTime = 4.0f;
        [Min(0.01f)]
        [SerializeField] private float minPatrolRadius = 2.0f;
        [Min(0.01f)]
        [SerializeField] private float maxPatrolRadius = 10.0f;
        [Min(0.01f)]
        [SerializeField] private float chaseRadius = 5.0f;
        [Min(0.01f)]
        [SerializeField] private float attackRadius = 2.5f;
        [SerializeField] private Transform checkTarget;

        public Transform Target { get => target; set => target = value; }
        public float MinPatrolTime { get => minPatrolTime; set => minPatrolTime = value; }
        public float MaxPatrolTime { get => maxPatrolTime; set => maxPatrolTime = value; }
        public float MinPatrolRadius { get => minPatrolRadius; }
        public float MaxPatrolRadius { get => maxPatrolRadius; }
        public float ChaseRadius { get => chaseRadius; }
        public float AttackRadius { get => attackRadius; }
        public Transform CheckTarget { get => checkTarget;}

        private Vehicle vehicle;
        private StateMachine stateMachine;
        private BasicEnemyPatrolState patrolState;
        private BasicEnemyChaseState chaseState;
        private BasicEnemyAttackState attackState;

        public void HandleMovement(Vector3 moveDirection)
        {
            if(moveDirection == Vector3.zero)
            {
                vehicle.Input = Vector2.zero;
                return;
            }
            Vector2 moveInput = Vector2.zero;
            float angle = Vector3.SignedAngle(transform.forward, moveDirection, transform.up);
            Debug.Log("Check angle: " + angle);

            if(angle >= -ignoreRotationAngle/2.0f && angle <= ignoreRotationAngle/2.0f)
            {
                moveInput.x = 0.0f;
            }
            else
            {
                moveInput.x = Mathf.Sign(angle);
            }

            moveInput.y = (angle > 180.0f || angle < -180.0f) ? -1.0f : 1.0f; 

            vehicle.Input = moveInput;
        }

        private void Awake()
        {
            vehicle = GetComponent<Vehicle>();
            stateMachine = new StateMachine();
            patrolState = new BasicEnemyPatrolState(this);
            chaseState = new BasicEnemyChaseState(this);
            attackState = new BasicEnemyAttackState(this);

            stateMachine.AddTransition(patrolState, 
                                       chaseState, 
                                       new FuncPredicate(() => 
                                       Vector3.SqrMagnitude(target.position - checkTarget.position) <= 
                                       chaseRadius * chaseRadius));

            stateMachine.AddTransition(chaseState,
                                       attackState,
                                       new FuncPredicate(() =>
                                       Vector3.SqrMagnitude(target.position - checkTarget.position) <=
                                       attackRadius * attackRadius));

            stateMachine.AddTransition(attackState,
                                       chaseState,
                                       new FuncPredicate(() =>
                                       Vector3.SqrMagnitude(target.position - checkTarget.position) >
                                       attackRadius * attackRadius
                                       ));

            stateMachine.AddTransition(chaseState,
                                       patrolState,
                                       new FuncPredicate(() =>
                                       Vector3.SqrMagnitude(target.position - checkTarget.position) >
                                       chaseRadius * chaseRadius
                                       ));

        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            stateMachine.SetState(patrolState);
        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log("Current state: " + stateMachine.GetCurrentStateName());
            stateMachine.OnUpdate();
        }

        private void FixedUpdate()
        {
            stateMachine.OnFixedUpdate();
        }

        private void LateUpdate()
        {
            stateMachine.OnLateUpdate();
        }

        private void OnDrawGizmosSelected()
        {
            if(checkTarget == null)
            {
                return;
            }

            //Draw gizmos for patrol radius
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(checkTarget.position, minPatrolRadius);
            Gizmos.DrawWireSphere(checkTarget.position, maxPatrolRadius);

            //Draw gizmos for chase radius
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(checkTarget.position, chaseRadius);

            //Draw gizmos for attack radius
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(checkTarget.position, attackRadius);

            //Draw gizmos for look range
            Vector3 leftDirection = Quaternion.AngleAxis(-ignoreRotationAngle/2.0f, transform.up) * transform.forward;
            Vector3 rightDirection = Quaternion.AngleAxis(ignoreRotationAngle / 2.0f, transform.up) * transform.forward;

            Gizmos.color = Color.black;
            Gizmos.DrawLine(checkTarget.position, checkTarget.position + leftDirection * 5.0f);
            Gizmos.DrawLine(checkTarget.position, checkTarget.position + rightDirection * 5.0f);


        }
    }
}
