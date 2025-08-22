using UnityEngine;
using Game.StateMachineHandling;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Gameplay
{
    public class BasicEnemy : MonoBehaviour
    {
        [Header("AI Settings: ")]
        [SerializeField] private Transform target;
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
        [SerializeField] private float patrolSpeed = 10.0f;
        [SerializeField] private float chaseSpeed = 20.0f;
        
        [Header("Blocking Settings: ")]

        [SerializeField] private Transform forwardBlockingTransform;
        [Min(0.01f)]
        [SerializeField] private float forwardBlockingDistance = 2.0f;
        [SerializeField] private Transform backwardBlockingTransform;
        [Min(0.01f)]
        [SerializeField] private float backwardBlockingDistance = 2.0f;
        [SerializeField] private LayerMask blockingLayerMask;

        [Header("Driving Settings: ")]
        [Min(15.0f)]
        [SerializeField] private float ignoreRotationAngle = 45.0f;
        [Min(0.01f)]
        [SerializeField] private float minSpeedToReduceAtCloseDist = 20.0f;
        [Min(0.01f)]
        [SerializeField] private float reduceSpeedDistance = 5.0f;
        [Min(0.01f)]
        [SerializeField] private float waypointCheckDistance = 5.0f;
        public Transform Target { get => target; set => target = value; }
        public float MinPatrolTime { get => minPatrolTime;}
        public float MaxPatrolTime { get => maxPatrolTime;}
        public float MinPatrolRadius { get => minPatrolRadius; }
        public float MaxPatrolRadius { get => maxPatrolRadius; }
        public float ChaseRadius { get => chaseRadius; }
        public float AttackRadius { get => attackRadius; }
        public Transform CheckTarget { get => checkTarget;}
        public float WaypointCheckDistance { get => waypointCheckDistance;}
        public float PatrolSpeed { get => patrolSpeed;}
        public float ChaseSpeed { get => chaseSpeed;}

        private Vehicle vehicle;
        private StateMachine stateMachine;
        private BasicEnemyPatrolState patrolState;
        private BasicEnemyChaseState chaseState;
        private BasicEnemyAttackState attackState;

        //This method is responsible for setting the speed of a car
        public void SetSpeed(float speed)
        {
            this.vehicle.ForwardSpeed = speed;
        }

        //This method is responsible for turning the brakes on
        public void ApplyBrakes()
        {
            this.vehicle.BrakesApplied = true;
        }

        //This method is responsible for turning the brakes off
        public void UnapplyBrakes()
        {
            this.vehicle.BrakesApplied = false;
        }

        //This method is responsible for moving towards the point.
        public void HandleMovement(Vector3 destination)
        {
            //Calculate move direction
            Vector3 moveDirection = (destination - this.transform.position).normalized;

            Vector2 moveInput = Vector2.zero;
            //Calculate the angle between car's forward direction and move direction.
            //If angle is under range of ignoreRotationAngle, then don't steer.
            //(this is done to avoid zigzag movement at the end of destination).
            //Otherwise, calculate the steer input based on angle and max steer angle.

            float angle = Vector3.SignedAngle(transform.forward, moveDirection, transform.up);
            if (angle >= -ignoreRotationAngle / 2.0f && angle <= ignoreRotationAngle / 2.0f)
            {
                moveInput.x = 0.0f;
            }
            else
            {
                float leftAngle = Mathf.Abs(vehicle.GetNormalLeftWheelAngle());
                float rightAngle = Mathf.Abs(vehicle.GetNormalRightWheelAngle());
                float steerInput = Mathf.Clamp(angle / Mathf.Max( leftAngle, rightAngle), -1, 1);//Mathf.Sign(angle);//Vector3.Dot(transform.right, moveDirection);
                moveInput.x = steerInput;
            }

            //Now, calculate acceleration input.
            float sqrDistance = Vector3.SqrMagnitude(destination - transform.position);

            //Check if it's there's anything blocking in forward direction,
            //if yes, then move backward.
            if (IsBlockingForward())
            {
                Debug.Log("Blocking forward, now reversing");
                moveInput.y = -1.0f;
            }
            //Else if, Check if it's there's anything blocking in backward direction,
            //if yes, then move forward.
            else if (IsBlockingBackward())
            {
                Debug.Log("Blocking backward, now accelerating");
                moveInput.y = 1.0f;
            }
            //Else if, check if our car's speed is greater than or equal to a specific amount
            //And also the distance between our's and destination is less than specific amount.
            //Then, we will reduce the speed of a car 
            else if(Mathf.Abs(this.vehicle.GetCurrentSpeed()) >= minSpeedToReduceAtCloseDist && sqrDistance < reduceSpeedDistance * reduceSpeedDistance)
            {
                Debug.Log("Reducing speed");
                moveInput.y = -Vector3.Dot(transform.forward, moveDirection);
                
            }
            //Else, if all goes well, then we will calculate accelerationInput based on
            //dot product between our car's forward direction and move direction
            else
            {
                Debug.Log("Now moving");
                moveInput.y = Vector3.Dot(transform.forward, moveDirection);
            }

            vehicle.Input = moveInput;
        }

        //This method is responsible for applying brakes instantly.
        public void ApplyInstantBrakes()
        {
            vehicle.ApplyInstantBrakes();
        }

        //This method is responsible for checking if something is blocking in forward direction
        private bool IsBlockingForward()
        {
            return Physics.Raycast(forwardBlockingTransform.position, 
                                   forwardBlockingTransform.forward, 
                                   forwardBlockingDistance, 
                                   blockingLayerMask.value);
        }

        //This method is responsible for checking if something is blocking in backward direction
        private bool IsBlockingBackward()
        {
            return Physics.Raycast(backwardBlockingTransform.position,
                                   backwardBlockingTransform.forward,
                                   backwardBlockingDistance,
                                   blockingLayerMask.value);
        }

        private void Awake()
        {
            vehicle = GetComponent<Vehicle>();
            
            //Created state machine
            //Added states such as patrol, chase and attack
            stateMachine = new StateMachine();
            patrolState = new BasicEnemyPatrolState(this);
            chaseState = new BasicEnemyChaseState(this);
            attackState = new BasicEnemyAttackState(this);


            //State machine will go from patrol to chase state
            //Only when the distance between target and the check target's position is less than chase radius
            stateMachine.AddTransition(patrolState, 
                                       chaseState, 
                                       new FuncPredicate(() =>
                                       this.target.gameObject.activeInHierarchy && Vector3.SqrMagnitude(target.position - checkTarget.position) <= 
                                       chaseRadius * chaseRadius));

            //State machine will go from chase to attack state
            //Only when the distance between target and the check target's position is less than attack radius
            stateMachine.AddTransition(chaseState,
                                       attackState,
                                       new FuncPredicate(() =>
                                       this.target.gameObject.activeInHierarchy && Vector3.SqrMagnitude(target.position - checkTarget.position) <=
                                       attackRadius * attackRadius));

            //State machine will go from attack to chase state
            //Only when the distance between target and the check target's position is greater than attack radius

            stateMachine.AddTransition(attackState,
                                       chaseState,
                                       new FuncPredicate(() =>
                                       Vector3.SqrMagnitude(target.position - checkTarget.position) >
                                       attackRadius * attackRadius
                                       ));

            //State machine will go from chase to patrol state
            //Only when the distance between target and the check target's position is greater than chase radius

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
            if(forwardBlockingTransform != null)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(forwardBlockingTransform.position,
                                forwardBlockingTransform.position +
                                forwardBlockingTransform.forward * forwardBlockingDistance);
            }

            if (backwardBlockingTransform != null)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(backwardBlockingTransform.position,
                                backwardBlockingTransform.position +
                                backwardBlockingTransform.forward * backwardBlockingDistance);
            }
            if(checkTarget == null)
            {
                return;
            }

            #if UNITY_EDITOR
            //Draw gizmos for patrol radius
            Handles.color = Color.yellow;
            Handles.DrawWireDisc(checkTarget.position, transform.up, minPatrolRadius);
            Handles.DrawWireDisc(checkTarget.position, transform.up, maxPatrolRadius);

            //Draw gizmos for chase radius
            Handles.color = Color.magenta;
            Handles.DrawWireDisc(checkTarget.position, transform.up, chaseRadius);

            //Draw gizmos for attack radius
            Handles.color = Color.red;
            Handles.DrawWireDisc(checkTarget.position, transform.up, attackRadius);

            #endif

            //Draw gizmos for look range
            Vector3 leftDirection = Quaternion.AngleAxis(-ignoreRotationAngle/2.0f, transform.up) * transform.forward;
            Vector3 rightDirection = Quaternion.AngleAxis(ignoreRotationAngle / 2.0f, transform.up) * transform.forward;

            
            Gizmos.color = Color.black;
            Gizmos.DrawLine(checkTarget.position, checkTarget.position + leftDirection * 5.0f);
            Gizmos.DrawLine(checkTarget.position, checkTarget.position + rightDirection * 5.0f);


        }
    }
}
