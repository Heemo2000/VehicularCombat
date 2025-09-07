using UnityEngine;

namespace Game.Gameplay.AbilityManagement
{
    [System.Serializable]
    public class CrashDashAbility : Ability
    {
        public LineRenderer lineRenderer;
        public AnimationCurve lineWidthCurve;
        public AimHandler aimHandler;
        public Vehicle vehicle;
        public LayerMask collisionLayerMask;
        public Transform aimingPoint;
        [Min(0.1f)]
        public float aimingSpeed = 10.0f;
        [Min(0.1f)]
        public float maxAimDistance = 5.0f;
        [Min(0.1f)]
        public float carSpeed = 30.0f;

        private float currentYRotation = 0.0f;
        private Vector3 endingPoint = Vector3.zero;
        private RaycastHit hit;
        private Rigidbody vehicleRB;
        public override void Execute()
        {
            vehicleRB.isKinematic = true;
            vehicleRB.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            vehicle.enabled = false;
        }

        public override void OnAim(Vector2 aimInput)
        {
            currentYRotation += aimInput.x * aimingSpeed;
            aimingPoint.rotation = Quaternion.Euler(0.0f, currentYRotation, 0.0f);
            Vector3 eulerAngles = vehicle.transform.rotation.eulerAngles;
            vehicle.transform.rotation = Quaternion.Euler(eulerAngles.x, currentYRotation, eulerAngles.z);
            if(Physics.Raycast(aimingPoint.position, aimingPoint.forward, out hit, maxAimDistance, collisionLayerMask.value))
            {
                endingPoint = hit.point;
            }
            else
            {
                endingPoint = aimingPoint.position + aimingPoint.forward * maxAimDistance;
            }
                
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, aimingPoint.position);
            lineRenderer.SetPosition(1, endingPoint);
        }

        public override void OnEquip()
        {
            lineRenderer.enabled = true;
            lineRenderer.widthCurve = lineWidthCurve;
            if (vehicleRB == null)
            {
                vehicleRB = vehicle.GetComponent<Rigidbody>();
            }
            aimHandler.enabled = false;
            vehicleRB.isKinematic = true;
            vehicleRB.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            vehicle.enabled = false;
            
        }

        public override void OnWithHold()
        {
            lineRenderer.enabled = false;
            vehicleRB.isKinematic = false;
            vehicleRB.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            vehicle.enabled = true;
            aimHandler.enabled = true;
        }

        
    }
}
