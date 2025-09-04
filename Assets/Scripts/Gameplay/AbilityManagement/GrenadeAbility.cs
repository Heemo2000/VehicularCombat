using UnityEngine;
using Game.Core;
using Game.Gameplay.Weapons;

namespace Game.Gameplay.AbilityManagement
{
    [System.Serializable]
    public class GrenadeAbility : Ability
    {
        public ProjectileGun projectileGun;
        public AimHandler aimHandler;
        public LineRenderer lineRenderer;
        public LayerMask collisionLayerMask;
        [Range(10, 500)]
        public int linePoints = 25;
        [Range(0.01f, 0.3f)]
        public float timeBetweenPoints = 0.1f;
        public override void Execute()
        {
            projectileGun.Fire();
        }

        public override void OnAim(Vector3 aimPosition)
        {
            aimHandler.AimPosition = aimPosition;
            DrawProjection();
        }

        public override void OnEquip()
        {
            lineRenderer.enabled = true;
        }

        public override void OnWithHold()
        {
            lineRenderer.enabled = false;
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
                    lineRenderer.positionCount = i + 1;
                    return;
                }
            }
        }
    }
}
