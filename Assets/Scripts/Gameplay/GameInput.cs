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
        [SerializeField] private bool simulateAndroid = false;
        private GameControls controls;
        private bool brakeApplied = false;
        private bool shouldFire = false;
        public UnityEvent OnWebInitialize;
        public UnityEvent OnMobileInitialize;
        public UnityEvent OnFireStarted;
        public UnityEvent OnFirePerformed;
        public UnityEvent OnFireReleased;
        public UnityEvent OnCommonAbilityToggle;
        public UnityEvent OnSpecialAbilityToggle;

        public bool BrakeApplied { get => brakeApplied;}
        public Vector2 GetRotateInput()
        {
            Vector2 finalPosition = Vector2.zero;
            if(Utility.IsEditor() || Application.platform == RuntimePlatform.WebGLPlayer || !simulateAndroid)
            {
                finalPosition = controls.Web.Aim.ReadValue<Vector2>();
            }
            else
            {
                finalPosition = controls.Mobile.AimAndShoot.ReadValue<Vector2>();
            }

            return finalPosition.normalized;
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
            Debug.Log("Current Platform: " + Application.platform);
            controls.Enable();
            if (Utility.IsEditor() || Application.platform == RuntimePlatform.WebGLPlayer || !simulateAndroid)
            {
                controls.Web.Enable();
                controls.Web.Fire.started += Fire_started;
                controls.Web.Fire.canceled += Fire_canceled;
                controls.Web.CommonAbility.started += CommonAbility_started;
                controls.Web.SpecialAbility.started += SpecialAbility_started;
                OnWebInitialize?.Invoke();
            }
            else if(Application.platform == RuntimePlatform.Android)
            {
                controls.Mobile.Enable();
                controls.Mobile.AimAndShoot.started += Fire_started;
                controls.Mobile.AimAndShoot.canceled += Fire_canceled;
                controls.Mobile.CommonAbility.started += CommonAbility_started;
                controls.Mobile.SpecialAbility.started += SpecialAbility_started;
                OnMobileInitialize?.Invoke();
            }
        }

        private void CommonAbility_started(InputAction.CallbackContext context)
        {
            OnCommonAbilityToggle?.Invoke();
        }

        private void SpecialAbility_started(InputAction.CallbackContext context)
        {
            OnSpecialAbilityToggle?.Invoke();
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
