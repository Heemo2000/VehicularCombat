using UnityEngine;

namespace Game.Gameplay
{
    public class AimHandler : MonoBehaviour
    {
        [SerializeField] private Vector2 rotationSpeed = new Vector2(10.0f, 10.0f); // degrees per second
        [SerializeField] private bool allowXRotation;
        [SerializeField] private Transform xAxisRotator;
        [SerializeField] private float minAngle = -90.0f;
        [SerializeField] private float maxAngle = 90.0f;

        private float xInput = 0.0f;
        private float yInput = 0.0f;

        private float currentXAngle = 0.0f;
        private float currentYAngle = 0.0f;

        public bool AllowXRotation { get => allowXRotation; set => allowXRotation = value; }
        public float XInput { get => xInput; set => xInput = value; }

        public float YInput { get => yInput; set => yInput = value; }
        void LateUpdate()
        {
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
