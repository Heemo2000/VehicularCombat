using UnityEngine;
using UnityEngine.InputSystem;
using Game.Input;

namespace Game.Gameplay
{
    public class GameInput : MonoBehaviour
    {
        private GameControls controls;

        public Vector2 GetMoveInput()
        {
            return controls.Player.Move.ReadValue<Vector2>();
        }

        private void Awake()
        {
            controls = new GameControls();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            controls.Enable();
        }

        private void OnDestroy()
        {
            controls.Disable();
        }
    }
}
