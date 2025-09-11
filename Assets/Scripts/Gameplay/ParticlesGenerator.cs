using UnityEngine;
using Game.ObjectPoolHandling;
using System.Collections.Generic;
using Game.Core;

namespace Game.Gameplay
{
    public class ParticlesGenerator : MonoBehaviour
    {
        [System.Serializable]
        public class ParticlesData
        {
            public ParticlesType type;
            public Particle[] particles;
            [Min(10)]
            public int count = 50;
        }

        [SerializeField] private ParticlesData[] particles;

        private Dictionary<ParticlesType, ObjectPool<Particle>> particlesDict;


        public void Spawn(ParticlesType type, Vector3 position, Quaternion rotation)
        {
            Particle particle = particlesDict[type].Get();
            if (particle != null) 
            { 
                particle.transform.position = position;
                particle.transform.rotation = rotation;
            }
            
        }

        public void ReturnToPool(Particle particle)
        {
            particlesDict[particle.ParticleType].ReturnToPool(particle);
        }

        private Particle CreateParticle(Particle[] prefabs)
        {
            var particle = Instantiate(prefabs[Random.Range(0, prefabs.Length)], Vector3.zero, Quaternion.identity);
            particle.ParticlesGenerator = this;
            particle.gameObject.SetActive(false);
            particle.transform.parent = transform;
            return particle;
            
        }

        private void OnGetParticle(Particle particle)
        {
            particle.gameObject.SetActive(true);
        }

        private void OnReturnToPoolParticle(Particle particle)
        {
            particle.gameObject.SetActive(false);
        }

        private void OnDestroyParticle(Particle particle)
        {
            Destroy(particle.gameObject);
        }

        private void Awake()
        {
            particlesDict = new Dictionary<ParticlesType, ObjectPool<Particle>>();
        }
        private void Start()
        {
            if(particlesDict.Count == 0)
            {
                foreach(var data in particles)
                {
                    var pool = new ObjectPool<Particle>(() => CreateParticle(data.particles), 
                                                        OnGetParticle, 
                                                        OnReturnToPoolParticle, 
                                                        OnDestroyParticle, 
                                                        data.count, 
                                                        true);
                
                    particlesDict.Add(data.type, pool);
                }
            }

            ServiceLocator.ForSceneOf(this).Register<ParticlesGenerator>(this);
        }


    }
}
