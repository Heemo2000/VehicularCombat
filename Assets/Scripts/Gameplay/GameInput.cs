using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Game.Input;
using Game.Core;

namespace Game.Gameplay
{
    public class GameInput : MonoBehaviour
    {
        [SerializeField] private RectTransform joystickOuter;
        [SerializeField] private bool forceJoystick = false;
        private GameControls controls;
        private bool brakeApplied = false;
        private bool shouldFire = false;
        public UnityEvent OnFireStarted;
        public UnityEvent OnFirePerformed;
        public UnityEvent OnFireReleased;

        public bool BrakeApplied { get => brakeApplied;}

        public Vector2 GetPointerPosition()
        {
            if(Mouse.current != null && !forceJoystick)
            {
                return controls.Player.AimPosition.ReadValue<Vector2>();
            }

            Vector2 start = joystickOuter.position;
            Vector2 end = controls.Player.AimPosition.ReadValue<Vector2>();

            Vector2 direction = (end - start).normalized;
            Vector2 centre = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);

            return centre + direction * Constants.DISTANCE_FROM_SCREEN_CENTRE;
        }

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
            controls.Player.Fire.started += Fire_started;
            controls.Player.Fire.canceled += Fire_canceled;
        }

        private void Update()
        {
            if(shouldFire)
            {
                OnFirePerformed?.Invoke();
            }
        }

        private void Fire_canceled(InputAction.CallbackContext context)
        {
            OnFireReleased?.Invoke();
            shouldFire = false;
        }

        private void Fire_started(InputAction.CallbackContext context)
        {
            OnFireStarted?.Invoke();
            shouldFire = true;
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
