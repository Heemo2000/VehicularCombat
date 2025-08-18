using UnityEngine;

namespace Game.Gameplay
{
    public class CarModelRotator : MonoBehaviour
    {
        [SerializeField] private float speed = 10.0f;

        private void FixedUpdate()
        {
            transform.Rotate(Vector3.up * speed * Time.fixedDeltaTime);
        }
    }
}
