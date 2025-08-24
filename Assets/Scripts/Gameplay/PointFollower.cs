using UnityEngine;

namespace Game.Gameplay
{
    public class PointFollower : MonoBehaviour
    {
        [Header("Driving Settings: ")]
        [SerializeField] private Transform forwardBlockingCheck;
        [Min(0.01f)]
        [SerializeField] private float forwardBlockingCheckDistance = 1.0f;
        [SerializeField] private Transform backwardBlockingCheck;
        [Min(0.01f)]
        [SerializeField] private float backwardBlockingCheckDistance = 1.0f;
        [Min(15.0f)]
        [SerializeField] private float ignoreRotationAngle = 45.0f;
        [Min(0.01f)]
        [SerializeField] private float minSpeedToReduceAtCloseDist = 20.0f;
        [Min(0.01f)]
        [SerializeField] private float reduceSpeedDistance = 5.0f;
        [SerializeField] private LayerMask blockingLayerMask;
        [SerializeField] private AvoidObstacleHandler avoidObstacleHandler;

        private Vehicle vehicle;

        //This method is responsible for moving towards the point.
        public void HandleMovement(Vector3 destination)
        {
            //Calculate move direction
            Vector3 moveDirection = (destination - this.transform.position).normalized;

            Vector2 moveInput = Vector2.zero;

            

            //Now, calculate acceleration input.
            float sqrDistance = Vector3.SqrMagnitude(destination - transform.position);

            //Check if it's there's anything blocking in forward direction,
            //if yes, then move backward.
            if (IsBlockingForward())
            {
                Debug.Log("Blocking forward, now reversing");
                moveInput.y = -1.0f;
            }
            //Else if, Check if it's there's anything blocking in backward direction,
            //if yes, then move forward.
            else if (IsBlockingBackward())
            {
                Debug.Log("Blocking backward, now accelerating");
                moveInput.y = 1.0f;
            }
            //Else if, check if our car's speed is greater than or equal to a specific amount
            //And also the distance between our's and destination is less than specific amount.
            //Then, we will reduce the speed of a car 
            else if (this.vehicle.GetCurrentSpeed() >= minSpeedToReduceAtCloseDist && sqrDistance < reduceSpeedDistance * reduceSpeedDistance)
            {
                Debug.Log("Reducing speed");
                moveInput.y = 0.0f;

            }
            //Else, if all goes well, then we will calculate accelerationInput based on
            //dot product between our car's forward direction and move direction
            else
            {
                Debug.Log("Now moving");
                float dot = Vector3.Dot(transform.forward, moveDirection);
                moveInput.y = (dot >= 0.0f && dot <= 0.5f) ? 1.0f - dot : dot;
            }

            //Calculate the angle between car's forward direction and move direction.
            //If angle is under range of ignoreRotationAngle, then don't steer.
            //(this is done to avoid zigzag movement at the end of destination).
            //Otherwise, calculate the steer input based on angle and max steer angle.

            float angle = Vector3.SignedAngle(transform.forward, moveDirection, transform.up);
            if (angle >= -ignoreRotationAngle / 2.0f && angle <= ignoreRotationAngle / 2.0f)
            {
                moveInput.x = 0.0f;
            }
            else
            {
                float leftAngle = Mathf.Abs(vehicle.GetNormalLeftWheelAngle());
                float rightAngle = Mathf.Abs(vehicle.GetNormalRightWheelAngle());


                float steerInput = angle / Mathf.Max(leftAngle, rightAngle);
                float avoidObstacleInput = avoidObstacleHandler.CalculateObstacleAvoidSteerInput();

                if (moveInput.y < 0.0f)
                {
                    steerInput = -steerInput;
                }

                Debug.Log("Avoid obstacle input: " + avoidObstacleInput);
                moveInput.x = Mathf.Clamp(steerInput + avoidObstacleInput, -1.0f, 1.0f);
            }

            vehicle.Input = moveInput;
        }

        //This method is responsible for checking if something is blocking in forward direction
        private bool IsBlockingForward()
        {
            return Physics.Raycast(forwardBlockingCheck.position,
                                   forwardBlockingCheck.forward,
                                   forwardBlockingCheckDistance,
                                   blockingLayerMask.value);
        }

        //This method is responsible for checking if something is blocking in backward direction
        private bool IsBlockingBackward()
        {
            return Physics.Raycast(backwardBlockingCheck.position,
                                   backwardBlockingCheck.forward,
                                   backwardBlockingCheckDistance,
                                   blockingLayerMask.value);
        }


        private void Awake()
        {
            vehicle = GetComponent<Vehicle>();
        }

        private void OnDrawGizmosSelected()
        {
            
            if (forwardBlockingCheck!= null)
            {
                //Draw gizmos for forward blocking.
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(forwardBlockingCheck.position,
                                forwardBlockingCheck.position +
                                forwardBlockingCheck.forward * forwardBlockingCheckDistance);

                //Draw gizmos for look range
                Vector3 leftDirection = Quaternion.AngleAxis(-ignoreRotationAngle / 2.0f, transform.up) * transform.forward;
                Vector3 rightDirection = Quaternion.AngleAxis(ignoreRotationAngle / 2.0f, transform.up) * transform.forward;


                Gizmos.color = Color.black;
                Gizmos.DrawLine(forwardBlockingCheck.position, forwardBlockingCheck.position + leftDirection * 5.0f);
                Gizmos.DrawLine(forwardBlockingCheck.position, forwardBlockingCheck.position + rightDirection * 5.0f);

            }

            //Draw gizmos for backward blocking.
            if (backwardBlockingCheck != null)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(backwardBlockingCheck.position,
                                backwardBlockingCheck.position +
                                backwardBlockingCheck.forward * backwardBlockingCheckDistance);
            }


            
            
        }
    }
}
