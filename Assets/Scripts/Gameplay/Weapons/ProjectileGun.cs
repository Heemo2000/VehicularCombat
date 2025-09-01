using Game.ObjectPoolHandling;
using UnityEngine;

namespace Game.Gameplay.Weapons
{
    public class ProjectileGun : Weapon
    {
        [Header("Bullet Settings: ")]
        [SerializeField] private float gravity = 10.0f;
        [Min(0.001f)]
        [SerializeField] private float speed = 0.01f;
        [Min(0.1f)]
        [SerializeField] private float mass = 1.0f;
        [Min(0.1f)]
        [SerializeField] private float throwStrength = 0.0f;

        [Header("Other Settings:")]
        [SerializeField] private Transform firePoint;
        [SerializeField] private ProjectileBullet bulletPrefab;
        [Min(50)]
        [SerializeField] private int maxBulletCount = 100;

        private ObjectPool<ProjectileBullet> bulletPool;

        public override void Fire()
        {
            var bullet = bulletPool.Get();
            bullet.BulletPool = bulletPool;
            bullet.Gravity = gravity;
            bullet.Speed = speed;
            bullet.Mass = mass;
            bullet.ThrowStrength = throwStrength;
            bullet.ThrowStartPosition = firePoint.position;
            bullet.ThrowDirection = firePoint.forward;
            bullet.CalculateStartVelocity();
            bullet.SetTimeToZero();
        }

        private ProjectileBullet CreateBullet()
        {
            ProjectileBullet bullet = Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity);
            bullet.transform.parent = transform;
            bullet.gameObject.SetActive(false);
            return bullet;
        }

        private void OnGetBullet(ProjectileBullet bullet)
        {
            bullet.gameObject.SetActive(true);
        }

        private void OnReturnBullet(ProjectileBullet bullet)
        {
            bullet.gameObject.SetActive(false);
        }

        private void OnDestroyBullet(ProjectileBullet bullet)
        {
            Destroy(bullet.gameObject);
        }

        private void Awake()
        {
            bulletPool = new ObjectPool<ProjectileBullet>(CreateBullet, 
                                                          OnGetBullet, 
                                                          OnReturnBullet, 
                                                          OnDestroyBullet, 
                                                          maxBulletCount);
        }
    }
}
