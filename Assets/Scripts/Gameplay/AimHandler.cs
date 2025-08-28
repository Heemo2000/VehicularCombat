using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Game.Gameplay
{
    public class AimHandler : MonoBehaviour
    {
        [Min(0.01f)]
        [SerializeField] private float rotationSpeed = 20f; // degrees per second
        private Vector3 aimPosition = Vector3.zero;

        public Vector3 AimPosition { get => aimPosition; set => aimPosition = value; }

        void Update()
        {
            // Ensure aiming only in the XZ plane
            Vector3 targetPosition = new Vector3(aimPosition.x, transform.position.y, aimPosition.z);
            Vector3 direction = targetPosition - transform.position;

            // Prevent zero direction errors
            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }
}
