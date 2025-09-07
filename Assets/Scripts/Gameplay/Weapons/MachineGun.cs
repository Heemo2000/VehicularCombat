using Game.ObjectPoolHandling;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Gameplay.Weapons
{
    public class MachineGun : Weapon
    {
        [Header("Main Settings: ")]
        [Min(0.01f)]
        [SerializeField] private float fireInterval = 1.0f;
        [SerializeField] private Transform[] firePoints;
        [SerializeField] private LinearBullet bulletPrefab;
        [Min(100)]
        [SerializeField] private int initialBulletsCount = 500;
        [Header("Barrel Settings: ")]
        [SerializeField] private Transform barrel;
        [Min(0.1f)]
        [SerializeField] private float maxBarrelRotationSpeed = 10.0f;
        [Min(0.01f)]
        [SerializeField] private float barrelBuildupSpeed = 10.0f;
        [Min(0.01f)]
        [SerializeField] private float barrelRecoilSpeed = 2.0f;
        [Min(0.01f)]
        [SerializeField] private float barrelRecoilDistance = 0.2f;

        private float nextFireTime = 0.0f;
        private bool shouldRotateBarrel = false;
        private bool shouldRecoil = false;
        private float targetBarrelSpeed = 0.0f;
        private Vector3 originalBarrelPosition = Vector3.zero;
        private float recoilDelta = 1.0f;
        private ObjectPool<LinearBullet> bulletPool;

        public UnityEvent OnFireDone;
        public void MakeBarrelRoll()
        {
            shouldRotateBarrel = true;
        }

        public void MakeBarrelUnroll()
        {
            shouldRotateBarrel = false;
        }

        public void StartRecoil()
        {
            shouldRecoil = true;
        }

        public void StopRecoil()
        {
            shouldRecoil = false;
        }

        public override void Fire()
        {
            if(nextFireTime < Time.time)
            {
                foreach(var firePoint in firePoints)
                {
                    var bullet = bulletPool.Get();
                    bullet.BulletPool = bulletPool;
                    bullet.transform.position = firePoint.position;
                    bullet.transform.forward = firePoint.forward;
                }

                OnFireDone?.Invoke();
                nextFireTime = Time.time + fireInterval;
            }
        }

        private LinearBullet CreateBullet()
        {
            LinearBullet bullet = Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity);
            bullet.transform.parent = transform;
            bullet.gameObject.SetActive(false);
            return bullet;
        }

        private void OnGetBullet(LinearBullet bullet)
        {
            bullet.gameObject.SetActive(true);
        }

        private void OnReturnBullet(LinearBullet bullet)
        {
            bullet.gameObject.SetActive(false);
        }

        private void OnDestroyBullet(LinearBullet bullet)
        {
            Destroy(bullet.gameObject);
        }

        private void Awake()
        {
            if(barrel != null)
            {
                originalBarrelPosition = barrel.localPosition;
            }
        }

        private void Start()
        {
            if(bulletPool == null)
            {
                bulletPool = new ObjectPool<LinearBullet>(CreateBullet, 
                                                          OnGetBullet, 
                                                          OnReturnBullet, 
                                                          OnDestroyBullet, 
                                                          initialBulletsCount, true);
            }
        }

        private void LateUpdate()
        {
            if (barrel == null)
            {
                return;
            }

            if(shouldRotateBarrel)
            {
                targetBarrelSpeed += barrelBuildupSpeed * Time.deltaTime;
            }
            else
            {
                targetBarrelSpeed -= barrelBuildupSpeed * Time.deltaTime;
            }

            targetBarrelSpeed = Mathf.Clamp(targetBarrelSpeed, 0.0f, maxBarrelRotationSpeed);

            barrel.Rotate(Vector3.forward * targetBarrelSpeed * Time.deltaTime);
            
            if(shouldRecoil)
            {
                recoilDelta += barrelRecoilSpeed * Time.deltaTime;
            }
            else
            {
                recoilDelta = Mathf.Lerp(recoilDelta, 1.0f, barrelBuildupSpeed * Time.deltaTime);
            }

            barrel.localPosition = Vector3.Lerp(originalBarrelPosition - Vector3.forward * barrelRecoilDistance,
                                               originalBarrelPosition,
                                               Mathf.Sin(Mathf.PI/2.0f * recoilDelta));
        }

        public override WeaponType GetWeaponType()
        {
            return WeaponType.Linear;
        }

        public override void Activate()
        {
            
        }

        public override void Deactivate()
        {
            
        }
    }
}
