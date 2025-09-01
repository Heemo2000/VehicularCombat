using UnityEngine;
using Game.ObjectPoolHandling;

namespace Game.Gameplay.Weapons
{
    public class LinearBullet : Bullet
    {
        [SerializeField] private float moveSpeed = 10.0f;
        private ObjectPool<LinearBullet> bulletPool = null;

        public ObjectPool<LinearBullet> BulletPool { get => bulletPool; set => bulletPool = value; }


        public override void Destroy()
        {
            if (bulletPool != null)
            {
                bulletPool.ReturnToPool(this);
                return;
            }

            Destroy(gameObject);
        }

        private void FixedUpdate()
        {
            base.bulletRB.MovePosition(base.bulletRB.position + 
                                       transform.forward * 
                                       moveSpeed * 
                                       Time.fixedDeltaTime);
        }
    }
}
