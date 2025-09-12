using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Game.Gameplay.AbilityManagement
{
    [System.Serializable]
    public class CrashDashAbility : Ability
    {
        public Material material;
        public LineRenderer lineRenderer;
        public AnimationCurve lineWidthCurve;
        public AimHandler aimHandler;
        public Vehicle vehicle;
        public LayerMask collisionLayerMask;
        public Transform aimingPoint;
        public Player player;
        public CollisionTrigger trigger;
        [Min(0.1f)]
        public float aimingSpeed = 10.0f;
        [Min(0.1f)]
        public float maxAimDistance = 5.0f;
        [Min(0.1f)]
        public float carSpeed = 30.0f;
        [Min(10000.0f)]
        public float requiredTorque = 30000.0f;

        private float currentYRotation = 0.0f;
        private Vector3 endingPoint = Vector3.zero;
        private RaycastHit hit;
        private Rigidbody vehicleRB;
        private bool collisionHappened = false;
        private bool addedCollisionListener = false;

        private Coroutine dashCoroutine;

        public override void Execute()
        {
            //vehicleRB.isKinematic = false;
            //vehicleRB.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            //vehicle.enabled = true;
            if(dashCoroutine == null)
            {
                dashCoroutine = vehicle.StartCoroutine(Dash());
            }
            
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
            lineRenderer.material = material;
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

            if(!addedCollisionListener)
            {
                trigger.OnCollision += MakeCollisionHappenedTrue;
                addedCollisionListener = true;
            }
            
        }

        public override void OnWithHold()
        {
            lineRenderer.enabled = false;
            vehicleRB.isKinematic = false;
            vehicleRB.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            vehicle.enabled = true;
            aimHandler.enabled = true;
            
            //trigger.OnCollision -= MakeCollisionHappenedTrue;
        }

        private void MakeCollisionHappenedTrue()
        {
            collisionHappened = true;
        }

        private IEnumerator Dash()
        {
            yield return null;
            vehicleRB.isKinematic = false;
            vehicleRB.collisionDetectionMode = CollisionDetectionMode.Discrete;
            vehicle.enabled = true;
            player.enabled = false;
            float defaultSpeed = vehicle.ForwardSpeed;
            float defaultTorque = vehicle.ForwardTorque;

            vehicle.ForwardSpeed = carSpeed;
            vehicle.ForwardTorque = requiredTorque;
            
            vehicle.Input = new Vector2(0.0f, 1.0f);
            collisionHappened = false;

            while(collisionHappened == false)
            {
                vehicle.Input = new Vector2(0.0f, 1.0f);
                yield return null;
            }

            vehicle.Input = Vector2.zero;

            player.enabled = true;
            vehicle.ForwardSpeed = defaultSpeed;
            vehicle.ForwardTorque = defaultTorque;

            dashCoroutine = null;
        }
    }
}
