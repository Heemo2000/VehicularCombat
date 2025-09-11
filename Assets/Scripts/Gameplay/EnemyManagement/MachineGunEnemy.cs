using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using Game.StateMachineHandling;
using Game.Core;
using Game.Gameplay.Weapons;

namespace Game.Gameplay.EnemyManagement
{
    public class MachineGunEnemy : BaseEnemy
    {
        [Header("AI Settings: ")]
        [Min(0.01f)]
        [SerializeField] private float minPatrolTime = 2.0f;
        [Min(0.01f)]
        [SerializeField] private float maxPatrolTime = 4.0f;
        [Min(0.01f)]
        [SerializeField] private float patrolRange = 10.0f;
        [Min(0.01f)]
        [SerializeField] private float chaseRange = 5.0f;
        [Min(0.01f)]
        [SerializeField] private float fireRange = 2.5f;

        [Min(0.01f)]
        [SerializeField] private float patrolSpeed = 10.0f;
        [Min(0.01f)]
        [SerializeField] private float chaseSpeed = 20.0f;
        [Min(0.01f)]
        [SerializeField] private float retreatSpeed = 30.0f;
        [Min(0.01f)]
        [SerializeField] private float retreatCooldownTime = 10.0f;

        [Min(0.01f)]
        [SerializeField] private float minHealth = 25.0f;
        [Min(0.01f)]
        [SerializeField] private float updateWaypointsTime = 1.0f;
        [SerializeField] private Transform[] retreatPoints;

        [Header("Driving Settings: ")]
        [Min(0.01f)]
        [SerializeField] private float waypointCheckDistance = 5.0f;
        [SerializeField] private PointFollower pointFollower;

        [Header("Weapon Settings: ")]
        [SerializeField] private AimHandler aimHandler;
        [SerializeField] private Weapon weapon;

        public float MinPatrolTime { get => minPatrolTime; }
        public float MaxPatrolTime { get => maxPatrolTime; }
        
        public float PatrolRange { get => patrolRange; }
        public float ChaseRange { get => chaseRange; }
        public float FireRange { get => fireRange; }
        public float WaypointCheckDistance { get => waypointCheckDistance; }
        public float PatrolSpeed { get => patrolSpeed; }
        public float ChaseSpeed { get => chaseSpeed; }
        public float RetreatSpeed { get => retreatSpeed; }
        public float RetreatCooldownTime { get => retreatCooldownTime; }
        public float UpdateWaypointsTime { get => updateWaypointsTime;}
        public AimHandler AimHandler { get => aimHandler;}
        public Weapon Weapon { get => weapon;}
        public Transform[] RetreatPoints { get => retreatPoints;}

        private Vehicle vehicle;
        private Health health;
        private StateMachine stateMachine;
        private MachineGunEnemyPatrolState patrolState;
        private MachineGunEnemyChaseState chaseState;
        private MachineGunEnemyFireState fireState;
        private MachineGunEnemyRetreatState retreatState;


        public void GoToPoint(Vector3 point)
        {
            pointFollower.HandleMovement(point);
        }

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

        //This method is responsible for applying brakes instantly.
        public void ApplyInstantBrakes()
        {
            vehicle.ApplyInstantBrakes();
        }

        private void DestroyVehicle()
        {
            if(ServiceLocator.ForSceneOf(this).TryGetService<ParticlesGenerator>(out var generator))
            {
                generator.Spawn(ParticlesType.Explosion, transform.position, Quaternion.identity);
            }

            gameObject.SetActive(false);
        }

        private void Awake()
        {
            vehicle = GetComponent<Vehicle>();
            health = GetComponent<Health>();
            //Created state machine
            //Added states such as patrol, chase, fire and retreat
            stateMachine = new StateMachine();
            
            patrolState = new MachineGunEnemyPatrolState(this);
            chaseState = new MachineGunEnemyChaseState(this);
            fireState = new MachineGunEnemyFireState(this);
            retreatState = new MachineGunEnemyRetreatState(this);

            stateMachine.AddTransition(patrolState, chaseState, 
                                       new FuncPredicate(
                                       ()=> !base.ignoreTarget && Utility.IsUnderDistance(transform.position, 
                                            base.target.position, chaseRange)));

            stateMachine.AddTransition(chaseState, patrolState,
                                       new FuncPredicate(
                                       () => !Utility.IsUnderDistance(transform.position,
                                            base.target.position, chaseRange)));

            stateMachine.AddTransition(chaseState, retreatState,
                                       new FuncPredicate(() =>
                                       health.CurrentAmount < minHealth));

            stateMachine.AddTransition(chaseState, fireState,
                                       new FuncPredicate(()=>
                                       !base.ignoreTarget && Utility.IsUnderDistance(transform.position, 
                                                               target.position, 
                                                               fireRange)));

            stateMachine.AddTransition(retreatState, patrolState,
                                       new FuncPredicate(() =>
                                       retreatState.IsCooldownCompleted() &&
                                       !Utility.IsUnderDistance(transform.position,
                                                                target.position,
                                                                fireRange)
                                       &&
                                       !Utility.IsUnderDistance(transform.position,
                                                                target.position,
                                                                chaseRange)
                                       ));

            stateMachine.AddTransition(retreatState, fireState,
                                       new FuncPredicate(() =>
                                       retreatState.IsCooldownCompleted() &&
                                       Utility.IsUnderDistance(transform.position,
                                                                target.position,
                                                                fireRange)
                                       ));

            stateMachine.AddTransition(retreatState, chaseState,
                                       new FuncPredicate(() =>
                                       retreatState.IsCooldownCompleted() &&
                                       !Utility.IsUnderDistance(transform.position,
                                                                target.position,
                                                                fireRange)
                                       ));

            stateMachine.AddTransition(fireState, patrolState,
                                       new FuncPredicate(() => !Utility.IsUnderDistance(transform.position,
                                                                target.position,
                                                                fireRange)
                                       &&
                                       !Utility.IsUnderDistance(transform.position,
                                                                target.position,
                                                                chaseRange)
                                       ));

            stateMachine.AddTransition(fireState, patrolState,
                                       new FuncPredicate(() => !Utility.IsUnderDistance(transform.position,
                                                                target.position,
                                                                fireRange)
                                       &&
                                       !Utility.IsUnderDistance(transform.position,
                                                                target.position,
                                                                chaseRange)
                                       ));

            stateMachine.AddTransition(fireState, chaseState,
                                       new FuncPredicate(() => !Utility.IsUnderDistance(transform.position,
                                                                target.position,
                                                                fireRange)));

            stateMachine.AddTransition(fireState, retreatState,
                                       new FuncPredicate(() => health.CurrentAmount < minHealth)
                                       );
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            stateMachine.SetState(patrolState);
            health.OnDeath.AddListener(DestroyVehicle);
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

        private void OnDisable()
        {
            health.OnDeath.RemoveListener(DestroyVehicle);
        }

        private void OnDrawGizmosSelected()
        {
            
            

#if UNITY_EDITOR
            //Draw gizmos for patrol range
            Handles.color = Color.yellow;
            Handles.DrawWireDisc(transform.position, transform.up, patrolRange);

            //Draw gizmos for chase range
            Handles.color = Color.magenta;
            Handles.DrawWireDisc(transform.position, transform.up, chaseRange);

            //Draw gizmos for attack range
            Handles.color = Color.red;
            Handles.DrawWireDisc(transform.position, transform.up, fireRange);

            //Draw gizmos for waypoint check distance
            Handles.color = Color.blue;
            Handles.DrawWireDisc(transform.position, transform.up, waypointCheckDistance);
#endif
        }
    }
}
