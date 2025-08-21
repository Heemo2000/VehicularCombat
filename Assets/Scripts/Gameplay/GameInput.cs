using UnityEngine;
using UnityEngine.InputSystem;
using Game.Input;

namespace Game.Gameplay
{
    public class GameInput : MonoBehaviour
    {
        private GameControls controls;
        private bool brakeApplied = false;

        public bool BrakeApplied { get => brakeApplied;}

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
            controls.Player.Brakes.started += Brakes_started;
            controls.Player.Brakes.canceled += Brakes_canceled;
        }

        private void Brakes_canceled(InputAction.CallbackContext obj)
        {
            brakeApplied = false;
        }

        private void Brakes_started(InputAction.CallbackContext obj)
        {
            brakeApplied = true;
        }

        private void OnDestroy()
        {
            controls.Disable();
        }
    }
}
