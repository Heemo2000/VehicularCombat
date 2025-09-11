using UnityEngine;
using Unity.AI.Navigation;
using Game.Core;
using System.Collections;

namespace Game.Gameplay
{
    public class NavmeshSurfaceModifier : MonoBehaviour
    {
        private NavMeshSurface surface;
        private Coroutine buildCoroutine;
        public void Bake()
        {
            StartCoroutine(BakeSlowly());
        }

        private IEnumerator BakeSlowly()
        {
            yield return new WaitForSeconds(1.0f);
            surface.BuildNavMesh();
        }
        private void Awake()
        {
            surface = GetComponent<NavMeshSurface>();
        }

        private void Start()
        {
            ServiceLocator.ForSceneOf(this).Register<NavmeshSurfaceModifier>(this);
        }
    }
}
