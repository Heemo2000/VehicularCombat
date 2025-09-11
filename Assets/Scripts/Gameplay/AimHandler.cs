using UnityEngine;

namespace Game.Gameplay
{
    public class AimHandler : MonoBehaviour
    {
        [SerializeField] private bool allowLookAtPosition = false;
        [SerializeField] private Vector2 rotationSpeed = new Vector2(10.0f, 10.0f); // degrees per second
        [SerializeField] private bool allowXRotation;
        [SerializeField] private Transform xAxisRotator;
        [SerializeField] private float minAngle = -90.0f;
        [SerializeField] private float maxAngle = 90.0f;

        private float xInput = 0.0f;
        private float yInput = 0.0f;

        private float currentXAngle = 0.0f;
        private float currentYAngle = 0.0f;
        private Vector3 aimPosition = Vector3.zero;
        private Vector3 currentLookPosition = Vector3.zero;
        private Vector3 targetLookPosition = Vector3.zero;
        public bool AllowXRotation { get => allowXRotation; set => allowXRotation = value; }
        public float XInput { get => xInput; set => xInput = value; }

        public float YInput { get => yInput; set => yInput = value; }
        public bool AllowLookAtPosition { get => allowLookAtPosition; set => allowLookAtPosition = value; }
        public Vector3 AimPosition { get => aimPosition; set => aimPosition = value; }

        void LateUpdate()
        {
            if(allowLookAtPosition)
            {
                targetLookPosition.x = aimPosition.x;
                targetLookPosition.y = transform.position.y;
                targetLookPosition.z = aimPosition.z;

                currentLookPosition = Vector3.Lerp(currentLookPosition, targetLookPosition, rotationSpeed.x * Time.deltaTime);
                transform.LookAt(currentLookPosition, Vector3.up);
                return;
            }
            currentXAngle = (allowXRotation == true) ? currentXAngle + xInput * rotationSpeed.x : 0.0f;
            currentXAngle = Mathf.Clamp(currentXAngle, minAngle, maxAngle);
            currentYAngle += yInput * rotationSpeed.y;
            
            transform.localRotation = Quaternion.Euler(0.0f, currentYAngle, 0.0f);
            if(xAxisRotator != null)
            {
                xAxisRotator.localRotation = Quaternion.Euler(currentXAngle, 0.0f, 0.0f);
            }
        }
    }
}
