using UnityEngine;
using UnityEngine.Events;

namespace Game.Gameplay
{
    public class Vehicle : MonoBehaviour
    {
        [Header("Movement Settings: ")]
        [Min(0.01f)]
        [SerializeField] private float movingSpeed = 10.0f;

        [Min(0.01f)]
        [SerializeField] private float rotationSpeed = 10.0f;
        
        [Min(5.0f)]
        [SerializeField] private float moveTransitioningSpeed = 20.0f;

        [Min(5.0f)]
        [SerializeField] private float rotateTransitioningSpeed = 20.0f;

        [Header("Event Settings: ")]
        public UnityEvent<Vector2> OnMove;

        private bool isMoving = false;
        private float currentSpeed = 0.0f;
        private float targetSpeed = 0.0f;
        private float currentRotationSpeed = 0.0f;
        private Vector2 moveInput = Vector2.zero;
        private Rigidbody vehicleRB;
        public void HandleMovement(Vector2 moveInput)
        {
            isMoving = moveInput.y != 0.0f;
            this.moveInput = moveInput;
            targetSpeed = (isMoving) ? movingSpeed : 0.0f;
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, moveTransitioningSpeed * Time.fixedDeltaTime);

            Vector3 horizontalVector = transform.forward * moveInput.y * currentSpeed;
            
            //Debug.Log("Horizontal vector: " + horizontalVector);
            
            vehicleRB.linearVelocity = horizontalVector;
            OnMove?.Invoke(moveInput);

            //Quaternion requiredRotation = (isMoving) ? Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(horizontalVector), rotateTransitioningSpeed * Time.fixedDeltaTime) : transform.rotation;
            //vehicleRB.MoveRotation(requiredRotation);

            float targetRotationSpeed = (isMoving) ? moveInput.x * rotationSpeed : 0.0f;
            currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, targetRotationSpeed, rotateTransitioningSpeed * Time.fixedDeltaTime);
            vehicleRB.AddTorque(Vector3.up * currentRotationSpeed, ForceMode.Impulse);
        }

        private void Awake()
        {
            vehicleRB = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            vehicleRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }
}
