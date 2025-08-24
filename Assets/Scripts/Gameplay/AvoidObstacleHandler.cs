using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

namespace Game.Gameplay
{
    public class AvoidObstacleHandler : MonoBehaviour
    {
        [Header("Obstacle Avoidance Settings: ")]
        [SerializeField] private Transform obstacleAvoidTransform;
        [Min(0.01f)]
        [SerializeField] private float avoidDistance = 5.0f;
        [Min(15.0f)]
        [SerializeField] private float avoidAngle = 90.0f;
        [Range(3, 8)]
        [SerializeField] private int avoidRayCount = 8;
        [SerializeField] private LayerMask avoidLayerMask;

        private Vector3[] interests;
        private bool[] dangers;
        
        private float result = 0.0f;
        private Vector3 origin = Vector3.zero;
        private int middleRayIndex = -1;
        private float angle = 0.0f;
        private Vector3 checkDirection = Vector3.zero;
        private Vector3 requiredInterest = Vector3.zero;
        private bool hitSomething = false;
        private Vector3 total = Vector3.zero;
        

        //This one is responsible for calculating on how much steer input
        //should be there so that obstacles can be avoided.
        public float CalculateObstacleAvoidSteerInput()
        {
            origin = obstacleAvoidTransform.position;
            middleRayIndex = Mathf.CeilToInt(avoidRayCount / 2.0f);

            for (int i = 0; i < avoidRayCount; i++)
            {
                if (i == middleRayIndex)
                {
                    interests[i] = Vector3.zero;
                    continue;
                }

                angle = Mathf.Lerp(-avoidAngle/2, avoidAngle/2, (float)i / (float)avoidRayCount);

                checkDirection = Quaternion.AngleAxis(angle, transform.up) * obstacleAvoidTransform.forward;

                hitSomething = Physics.Linecast(origin, origin + checkDirection * avoidDistance, avoidLayerMask.value);

                requiredInterest = checkDirection;

                dangers[i] = hitSomething;

                if (dangers[i])
                {
                    requiredInterest = Vector3.zero;
                }

                interests[i] = requiredInterest;
            }

            result = 0.0f;
            total = Vector3.zero;
            for (int i = 0; i < avoidRayCount; i++)
            {
                total += interests[i];
            }

            total.Normalize();
            result = Mathf.Clamp(Vector3.SignedAngle(transform.forward, total, transform.up) / avoidAngle,
                                                  -1.0f,
                                                  1.0f);

            return result;
        }

        private void Awake()
        {
            interests = new Vector3[avoidRayCount];
            dangers = new bool[avoidRayCount];
        }

        private void OnValidate()
        {
            if(interests != null && interests.Length != avoidRayCount)
            {
                interests = new Vector3[avoidRayCount];
                dangers = new bool[avoidRayCount];
            }
        }

        private void OnDrawGizmosSelected()
        {
            //Draw gizmos for obstacle avoidance.(Context based steering)
            if (obstacleAvoidTransform != null)
            {
                float angle = 0.0f;

                Gizmos.color = Color.red;
                Vector3 avoidObstacleOrigin = obstacleAvoidTransform.position;
                int middleRayIndex = Mathf.RoundToInt(avoidRayCount / 2);
                for (int i = 0; i < avoidRayCount; i++)
                {
                    if (i == middleRayIndex)
                    {
                        continue;
                    }

                    angle = Mathf.Lerp(-avoidAngle / 2.0f, avoidAngle / 2.0f, (float)i / (float)avoidRayCount);
                    Vector3 direction = Quaternion.AngleAxis(angle, transform.up) * obstacleAvoidTransform.forward;
                    Gizmos.DrawLine(avoidObstacleOrigin, avoidObstacleOrigin + direction * avoidDistance);
                }
            }
        }
    }
}
