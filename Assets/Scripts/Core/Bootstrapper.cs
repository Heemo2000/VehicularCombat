using UnityEngine;

namespace Game.Core
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ServiceLocator))]
    public abstract class Bootstrapper : MonoBehaviour
    {
        private ServiceLocator container;
        private bool hasBeenBootstrapped = false;
        internal ServiceLocator Container
        {
            get
            {
                if(container != null)
                {
                    return container;
                }

                container = GetComponent<ServiceLocator>();
                return container;
            }
        }

        public void BootstrapOnDemand()
        {
            if(hasBeenBootstrapped)
            {
                return;
            }

            hasBeenBootstrapped = true;
            Bootstrap();
        }

        protected abstract void Bootstrap();

        private void Awake()
        {
            BootstrapOnDemand();
        }
    }
}
