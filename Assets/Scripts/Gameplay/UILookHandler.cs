using UnityEngine;

namespace Game.Gameplay
{
    public class UILookHandler : MonoBehaviour
    {
        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            transform.LookAt(transform.position + mainCamera.transform.forward);
        }
    }
}
