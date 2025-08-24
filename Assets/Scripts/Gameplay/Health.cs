using UnityEngine;
using UnityEngine.Events;

namespace Game.Gameplay
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 20f;
        private float currentAmount;

        public float MaxHealth { get => maxHealth; }
        public float CurrentAmount { get => currentAmount; }

        public UnityEvent<float> OnHealthDamaged;

        public UnityEvent<float> OnHealthHealed;
        public UnityEvent<float, float> OnCurrentHealthSet;

        public UnityEvent OnDeath;

        public UnityEvent OnHealthUpdated;


        // Start is called before the first frame update
        void Start()
        {
            currentAmount = maxHealth;
            OnCurrentHealthSet.AddListener(SetHealth);
            OnHealthDamaged.AddListener(TakeDamage);
            OnHealthHealed.AddListener(RepairHealth);
            OnCurrentHealthSet?.Invoke(maxHealth, maxHealth);
        }

        private void SetHealth(float amount, float maxAmount)
        {
            currentAmount = Mathf.Clamp(amount, 0f, maxAmount);
            if (currentAmount == 0f)
            {
                OnDeath?.Invoke();
            }
            OnHealthUpdated?.Invoke();
        }

        private void TakeDamage(float amount)
        {
            OnCurrentHealthSet?.Invoke(currentAmount - amount, maxHealth);
        }

        private void RepairHealth(float amount)
        {
            OnCurrentHealthSet?.Invoke(currentAmount + amount, maxHealth);
        }

        private void OnDestroy()
        {
            OnCurrentHealthSet.RemoveListener(SetHealth);
            OnHealthDamaged.RemoveListener(TakeDamage);
            OnHealthHealed.RemoveListener(RepairHealth);
        }
    }
}
