using System.Collections;
using UnityEngine;
namespace Game.Gameplay.AbilityManagement
{
    public class ProjectileIndicator : MonoBehaviour
    {
        [SerializeField] private Transform indicatorSphere;
        [Min(1.0f)]
        [SerializeField] private float sphereScale = 10.0f;
        [Min(0.1f)]
        [SerializeField] private float speed = 10.0f;
        public void Activate()
        {
            StartCoroutine(ActivateSphere(true));
        }

        public void Deactivate()
        {
            StartCoroutine(ActivateSphere(false));
        }

        private IEnumerator ActivateSphere(bool shouldIncreaseScale)
        {
            float delta = 0.0f;
            Vector3 initialScale = (shouldIncreaseScale == true) ? Vector3.zero : Vector3.one * sphereScale;
            Vector3 finalScale = (shouldIncreaseScale == true) ? Vector3.one * sphereScale: Vector3.zero;

            indicatorSphere.gameObject.SetActive(true);
            while (delta <= 1.0f)
            {
                indicatorSphere.localScale = Vector3.Lerp(initialScale, finalScale, delta);
                delta += speed * Time.deltaTime;
                yield return null;
            }

            yield return null;

            indicatorSphere.localScale = finalScale;

            if(indicatorSphere.localScale.x < 0.1f)
            {
                indicatorSphere.gameObject.SetActive(false);
            }
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            indicatorSphere.gameObject.SetActive(false);
        }
    }
}
