using UnityEngine;

namespace Game.Gameplay.Weapons
{
    public class MachineGun : Weapon
    {
        [Header("Main Settings: ")]
        [Min(5)]
        [SerializeField] private int fireRate = 50;
        [SerializeField] private Transform firePoint;

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
                nextFireTime = Time.time + 1.0f/(float)fireRate;
            }
        }

        private void Awake()
        {
            originalBarrelPosition = barrel.localPosition;
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
                recoilDelta = 1.0f;
            }

            barrel.localPosition = Vector3.Lerp(originalBarrelPosition - Vector3.forward * barrelRecoilDistance,
                                               originalBarrelPosition,
                                               Mathf.Sin(Mathf.PI/2.0f * recoilDelta));
        }
    }
}
