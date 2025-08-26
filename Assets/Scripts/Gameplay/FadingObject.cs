using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay
{
    public class FadingObject : MonoBehaviour
    {
        [SerializeField]
        private List<Renderer> renderers = new List<Renderer>();
        
        [SerializeField]
        private Vector3 position;
        
        [SerializeField]private List<Material> materials = new List<Material>();
        
        [SerializeField] private float initialAlpha;

        public List<Renderer> Renderers { get => renderers;}
        public List<Material> Materials { get => materials;}
        public float InitialAlpha { get => initialAlpha;}

        private void Awake()
        {
            position = transform.position;

            if (renderers.Count == 0)
            {
                renderers.AddRange(GetComponentsInChildren<Renderer>());
            }
            foreach (Renderer renderer in renderers)
            {
                materials.AddRange(renderer.materials);
            }

            initialAlpha = materials[0].color.a;
        }

        public bool Equals(FadingObject other)
        {
            return position.Equals(other.position);
        }

        public override int GetHashCode()
        {
            return position.GetHashCode();
        }
    }
}
