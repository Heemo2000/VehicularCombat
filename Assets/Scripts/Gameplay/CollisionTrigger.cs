using UnityEngine;
using System;

namespace Game.Gameplay
{
    public class CollisionTrigger : MonoBehaviour
    {
        public Action OnCollision;        
        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("Collided with " + collision.gameObject.name);
            OnCollision?.Invoke();
        }
    }
}
