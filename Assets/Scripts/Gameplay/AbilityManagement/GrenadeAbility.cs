using Game.Gameplay.Weapons;
using UnityEngine;

namespace Game.Gameplay.AbilityManagement
{
    [System.Serializable]
    public class GrenadeAbility : Ability
    {
        public ProjectileGun projectileGun;
        public AimHandler aimHandler;
        public LineRenderer lineRenderer;
        public LayerMask collisionLayerMask;
        public override void Execute()
        {
            
        }

        public override void OnAim(Vector3 aimPosition)
        {
            DrawProjection();

        }

        public override void OnEquip()
        {
            
        }

        public override void OnWithHold()
        {
            
        }

        private void DrawProjection()
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 101;
            Vector3 startPosition = projectileGun.FirePoint.position;
            Vector3 startVelocity = projectileGun.ThrowStrength * projectileGun.FirePoint.forward / projectileGun.Mass;
            int i = 0;
            lineRenderer.SetPosition(i, startPosition);
            for (float time = 0; time < 101; time += 0.01f)
            {
                i++;
                Vector3 point = startPosition + time * startVelocity;
                point.y = startPosition.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);

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
