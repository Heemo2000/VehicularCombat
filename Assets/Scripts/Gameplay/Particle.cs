using UnityEngine;

namespace Game.Gameplay
{
    public class Particle : MonoBehaviour
    {
        [SerializeField] private ParticlesType particleType;
        private ParticleSystem particle;
        private ParticlesGenerator particlesGenerator;
        public ParticlesType ParticleType { get => particleType;}
        public ParticlesGenerator ParticlesGenerator { get => particlesGenerator; set => particlesGenerator = value; }

        public void Play()
        {
            particle.Play();
        }

        private void Awake()
        {
            particle = GetComponent<ParticleSystem>();
        }

        private void Start()
        {
            var main = particle.main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }

        private void OnParticleSystemStopped()
        {
            particlesGenerator.ReturnToPool(this);
        }
    }
}
