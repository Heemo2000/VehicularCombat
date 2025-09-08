using UnityEngine;
using Game.ObjectPoolHandling;
namespace Game.Gameplay.Weapons
{
    public abstract class Bullet : MonoBehaviour
    {
        [Min(0.01f)]
        [SerializeField] protected float destroyTime = 5.0f;
        [Min(0.1f)]
        [SerializeField] private float damage = 10.0f;
        private float currentTime = 0.0f;
        protected Rigidbody bulletRB;
        protected Collider bulletCollider;

        public abstract void Destroy();
        protected virtual void Awake()
        {
            bulletRB = GetComponent<Rigidbody>();
            bulletCollider = GetComponent<Collider>();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected virtual void Start()
        {
            bulletRB.isKinematic = true;
            bulletCollider.isTrigger = true;
            currentTime = 0.0f;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (currentTime >= destroyTime)
            {
                Destroy();
                return;
            }

            currentTime += Time.deltaTime;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            HealthManager health = other.GetComponent<HealthManager>();
            if(health != null)
            {
                health.TakeDamage(damage);
            }
            Destroy();
        }
    }
}
