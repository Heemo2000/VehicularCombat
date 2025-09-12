using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Game.Core;

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
        [Min(0.1f)]
        public float damage = 300.0f;
        [Min(0.1f)]
        public float damageRadius = 10.0f;
        [Min(1.0f)]
        public float throwForce = 300.0f;
        public LayerMask damageLayerMask;

        private float currentYRotation = 0.0f;
        private Vector3 endingPoint = Vector3.zero;
        private RaycastHit hit;
        private Rigidbody vehicleRB;
        private bool collisionHappened = false;
        private bool firstTimeSetup = false;
        private Collider[] detected;

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

            if(!firstTimeSetup)
            {
                detected = new Collider[Constants.MAX_COLLIDER_COUNT];
                firstTimeSetup = true;
            }
            trigger.OnCollision += MakeCollisionHappenedTrue;
            trigger.OnCollision += ThrowOtherCarsAndDamageThem;
            trigger.OnCollision += ShowVisual;
        }

        public override void OnWithHold()
        {
            lineRenderer.enabled = false;
            vehicleRB.isKinematic = false;
            vehicleRB.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            vehicle.enabled = true;
            aimHandler.enabled = true;
            collisionHappened = false;
            
            //trigger.OnCollision -= MakeCollisionHappenedTrue;
        }

        private void MakeCollisionHappenedTrue()
        {
            collisionHappened = true;
        }

        private void ThrowOtherCarsAndDamageThem()
        {
            int count = Physics.OverlapSphereNonAlloc(vehicle.transform.position, damageRadius, detected, damageLayerMask.value);
            Debug.Log("Detected count: " + count);
            for (int i = 0; i < count; i++)
            {
                Vector3 current = detected[i].transform.position;
                Health health = detected[i].GetComponent<Health>();
                
                if (health != null)
                {
                    health.OnHealthDamaged?.Invoke(damage);
                    Vector3 direction = (current - vehicle.transform.position).normalized;
                    if(health.TryGetComponent<Rigidbody>(out var rb))
                    {
                        rb.AddForce(direction * throwForce, ForceMode.Impulse);
                    }
                }
            }
        }
        
        private void ShowVisual()
        {
            if (ServiceLocator.ForSceneOf(vehicle).TryGetService<ParticlesGenerator>(out var generator))
            {
                generator.Spawn(ParticlesType.Explosion, vehicle.transform.position, Quaternion.identity);
            }
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

            while(vehicle.enabled && collisionHappened == false)
            {
                vehicle.Input = new Vector2(0.0f, 1.0f);
                yield return null;
            }

            vehicle.Input = Vector2.zero;

            player.enabled = true;
            vehicle.ForwardSpeed = defaultSpeed;
            vehicle.ForwardTorque = defaultTorque;

            
            trigger.OnCollision -= MakeCollisionHappenedTrue;
            trigger.OnCollision -= ThrowOtherCarsAndDamageThem;
            trigger.OnCollision -= ShowVisual;
            dashCoroutine = null;
        }
    }
}
