using UnityEngine;
using Game.ObjectPoolHandling;
using Game.Core;

namespace Game.Gameplay.Weapons
{ 
    public class ProjectileBullet : Bullet
    {
        [SerializeField] private float gravity = 10.0f;
        [Min(0.001f)]
        [SerializeField] private float speed = 0.01f;
        [Min(0.1f)]
        [SerializeField] private float mass = 1.0f;
        [Min(0.1f)]
        [SerializeField]private float throwStrength = 0.0f;
        [SerializeField]private Vector3 throwStartPosition = Vector3.zero;
        [SerializeField]private Vector3 throwDirection = Vector3.zero;
        [Min(0.1f)]
        [SerializeField] private float attackRange = 5.0f;

        private float time = 0.0f;
        private Vector3 currentPosition = Vector3.zero;
        private Vector3 pastPosition = Vector3.zero;
        private Vector3 direction = Vector3.zero;
        private Quaternion requiredRotation = Quaternion.identity;
        private Vector3 startVelocity = Vector3.zero;
        
        private ObjectPool<ProjectileBullet> bulletPool;

        public float Gravity { get => gravity; set => gravity = value; }
        public float Speed { get => speed; set => speed = value; }
        public float Mass { get => mass; set => mass = value; }
        public float ThrowStrength { get => throwStrength; set => throwStrength = value; }
        public Vector3 ThrowStartPosition { get => throwStartPosition; set => throwStartPosition = value; }
        public Vector3 ThrowDirection { get => throwDirection; set => throwDirection = value; }
        public ObjectPool<ProjectileBullet> BulletPool { get => bulletPool; set => bulletPool = value; }

        public override void Destroy()
        {
            if(bulletPool != null)
            {
                bulletPool.ReturnToPool(this);
                return;
            }

            Destroy(gameObject);
        }
        public void CalculateStartVelocity()
        {
            startVelocity = throwStrength * throwDirection / mass;
        }
        public void SetTimeToZero()
        {
            time = 0.0f;
        }
        protected override void Start()
        {
            base.Start();
            SetTimeToZero();
            CalculateStartVelocity();
        }
        private void CalculatePosition()
        {
            currentPosition = Utility.CalculateProjectilePosition(throwStartPosition, startVelocity, gravity, time);
            
            base.bulletRB.MovePosition(currentPosition);
            if (time > 0.0f)
            {
                pastPosition = Utility.CalculateProjectilePosition(throwStartPosition, startVelocity, gravity, time - speed * Time.fixedDeltaTime);
                direction = (currentPosition - pastPosition).normalized;
                requiredRotation = Quaternion.FromToRotation(Vector3.forward, direction);
                base.bulletRB.MoveRotation(requiredRotation);
            }

            time += speed * Time.fixedDeltaTime;
        }

        private void FixedUpdate()
        {
            CalculatePosition();
        }
    }
}
