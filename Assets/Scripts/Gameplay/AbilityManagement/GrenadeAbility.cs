using UnityEngine;
using Game.Core;
using Game.Gameplay.Weapons;

namespace Game.Gameplay.AbilityManagement
{
    [System.Serializable]
    public class GrenadeAbility : Ability
    {
        public GameObject originalWeaponMesh;
        public GameObject targetWeaponMesh;
        public ProjectileGun projectileGun;
        public WeaponManager weaponManager;
        public AimHandler aimHandler;
        public Material material;
        public LineRenderer lineRenderer;
        public AnimationCurve lineWidthCurve;
        public LayerMask collisionLayerMask;
        public ProjectileIndicator indicator;
        
        [Range(10, 500)]
        public int linePoints = 25;
        [Range(0.01f, 0.3f)]
        public float timeBetweenPoints = 0.1f;

        public override void Execute()
        {
            weaponManager.Fire();
        }

        public override void OnAim(Vector2 aimInput)
        {
            aimHandler.XInput = aimInput.y;
            aimHandler.YInput = aimInput.x;
            DrawProjection();
        }

        public override void OnEquip()
        {
            originalWeaponMesh.SetActive(false);
            targetWeaponMesh.SetActive(true);
            lineRenderer.enabled = true;
            aimHandler.AllowXRotation = true;
            lineRenderer.material = material;
            lineRenderer.widthCurve = lineWidthCurve;
            weaponManager.Activate(projectileGun);
            indicator.Activate();
        }

        public override void OnWithHold()
        {
            lineRenderer.enabled = false;
            aimHandler.AllowXRotation = false;
            originalWeaponMesh.SetActive(true);
            targetWeaponMesh.SetActive(false);
            weaponManager.Activate(WeaponType.Linear);
            indicator.Deactivate();
        }

        private void DrawProjection()
        {
            
            lineRenderer.positionCount = Mathf.CeilToInt(linePoints / timeBetweenPoints) + 1;
            Vector3 startPosition = projectileGun.FirePoint.position;
            Vector3 startVelocity = projectileGun.ThrowStrength * projectileGun.FirePoint.forward / projectileGun.Mass;
            int i = 0;
            lineRenderer.SetPosition(i, startPosition);
            for (float time = 0; time < linePoints; time += timeBetweenPoints)
            {
                i++;
                Vector3 point = Utility.CalculateProjectilePosition(startPosition, startVelocity, projectileGun.Gravity, time);

                lineRenderer.SetPosition(i, point);

                Vector3 lastPosition = lineRenderer.GetPosition(i - 1);

                if (Physics.Raycast(lastPosition,
                    (point - lastPosition).normalized,
                    out RaycastHit hit,
                    (point - lastPosition).magnitude,
                    collisionLayerMask))
                {
                    lineRenderer.SetPosition(i, hit.point);
                    indicator.transform.position = hit.point;
                    lineRenderer.positionCount = i + 1;
                    return;
                }
            }
        }
    }
}
