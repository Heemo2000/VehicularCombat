using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Game.Input;
using Game.Core;
using System;

namespace Game.Gameplay
{
    public class GameInput : MonoBehaviour
    {
        [SerializeField] private RectTransform joystickOuter;
        [SerializeField] private bool simulateAndroid = false;
        private GameControls controls;
        private bool brakeApplied = false;
        private bool shouldFire = false;
        public UnityEvent OnWebInitialize;
        public UnityEvent OnMobileInitialize;
        public UnityEvent OnFireStarted;
        public UnityEvent OnFirePerformed;
        public UnityEvent OnFireReleased;

        public bool BrakeApplied { get => brakeApplied;}

        public Vector2 GetPointerPosition()
        {
            if(Utility.IsEditor() || Application.platform == RuntimePlatform.WebGLPlayer || !simulateAndroid)
            {
                return controls.Web.AimPosition.ReadValue<Vector2>();
            }

            Vector2 start = joystickOuter.position;
            Vector2 end = controls.Mobile.AimAndShoot.ReadValue<Vector2>();

            Vector2 direction = (end - start).normalized;
            Vector2 centre = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);

            return centre + direction * Constants.DISTANCE_FROM_SCREEN_CENTRE;
        }

        public Vector2 GetMoveInput()
        {
            if(Utility.IsEditor() || Application.platform == RuntimePlatform.WebGLPlayer || !simulateAndroid)
            {
                return controls.Web.Move.ReadValue<Vector2>();
            }

            return controls.Mobile.Move.ReadValue<Vector2>();
        }

        private void Awake()
        {
            controls = new GameControls();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            controls.Enable();
            if (Utility.IsEditor() || Application.platform == RuntimePlatform.WebGLPlayer || !simulateAndroid)
            {
                controls.Web.Enable();
                controls.Web.Fire.started += Fire_started;
                controls.Web.Fire.canceled += Fire_canceled;
                controls.Web.PreviousCommonAbility.started += PreviousCommonAbility_started;
                controls.Web.NextCommonAbility.started += NextCommonAbility_started;
                OnWebInitialize?.Invoke();
            }
            else if(Application.platform == RuntimePlatform.Android)
            {
                controls.Mobile.Enable();
                controls.Mobile.AimAndShoot.started += Fire_started;
                controls.Mobile.AimAndShoot.canceled += Fire_canceled;
                controls.Mobile.PreviousCommonAbility.started += PreviousCommonAbility_started;
                controls.Mobile.NextCommonAbility.started += NextCommonAbility_started;
                OnMobileInitialize?.Invoke();
            }
        }

        private void NextCommonAbility_started(InputAction.CallbackContext context)
        {
            
        }

        private void PreviousCommonAbility_started(InputAction.CallbackContext context)
        {
            
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

        private void OnDestroy()
        {
            controls.Disable();
        }
    }
}
