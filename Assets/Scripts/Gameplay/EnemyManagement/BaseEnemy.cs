using UnityEngine;

namespace Game.Gameplay.EnemyManagement
{
    public abstract class BaseEnemy : MonoBehaviour
    {
        [SerializeField] protected Transform target;
        [SerializeField] protected bool ignoreTarget = false;
        public Transform Target { get => target; set => target = value; }
    }
}
