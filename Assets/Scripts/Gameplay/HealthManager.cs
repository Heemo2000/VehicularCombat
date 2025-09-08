using UnityEngine;

namespace Game.Gameplay
{
    public class HealthManager : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private Health shield;

        public void TakeDamage(float damage)
        {
            if(shield.CurrentAmount > 0.0f)
            {
                float remainingDamage = 0.0f;
                shield.OnHealthDamaged?.Invoke(damage);
                remainingDamage = shield.CurrentAmount;
                if (remainingDamage < 0.0f)
                {
                    health.OnHealthDamaged?.Invoke(Mathf.Abs(remainingDamage));
                }
            }
            else
            {
                health.OnHealthDamaged?.Invoke(damage);
            }
        }
    }
}
