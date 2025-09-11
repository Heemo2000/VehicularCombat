using Game.Core;
using UnityEngine;

namespace Game.Gameplay
{
    public class DestructibleWood : MonoBehaviour
    {
        private Health health;

        private void ShowParticleEffect()
        {

        }

        private void Destroy()
        {
            Destroy(gameObject);
            if(ServiceLocator.ForSceneOf(this).TryGetService<NavmeshSurfaceModifier>(out var surfaceModifier))
            {
                surfaceModifier.Bake();
            }
        }

        private void Awake()
        {
            health = GetComponent<Health>();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            health.OnDeath.AddListener(ShowParticleEffect);
            health.OnDeath.AddListener(Destroy);
        }

        private void OnDestroy()
        {
            health.OnDeath.RemoveListener(ShowParticleEffect);
            health.OnDeath.RemoveListener(Destroy);
        }
    }
}
