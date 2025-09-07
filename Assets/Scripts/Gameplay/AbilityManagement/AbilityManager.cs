using Game.Gameplay.Weapons;
using UnityEngine;

namespace Game.Gameplay.AbilityManagement
{
    public class AbilityManager : MonoBehaviour
    {
        [SerializeField] private GameInput gameInput;
        [SerializeField] private MachineGun machineGun;
        [Min(0.1f)]
        [SerializeField] private float waitingTime = 2.0f;
        [SerializeField] private AbilityData commonAbility;
        [SerializeField] private AbilityData specialAbility;
        
        private AbilityData currentActiveAbility;
        private float currentTime = 0.0f;
        private void Fire()
        {
            if(currentActiveAbility != null)
            {
                currentActiveAbility.ability.Execute();
                currentActiveAbility.ability.OnWithHold();
                currentActiveAbility = null;
            }
            else
            {
                machineGun.Fire();
            }
        }

        private void CheckAndActivateCommonAbility()
        {
            if(currentActiveAbility == null)
            {
                currentActiveAbility = commonAbility;
                currentActiveAbility.ability.OnEquip();
            }
            else
            {
                currentActiveAbility.ability.OnWithHold();
                currentActiveAbility = null;
            }
        }

        private void CheckAndActivateSpecialAbility()
        {
            if (currentActiveAbility == null)
            {
                currentActiveAbility = specialAbility;
                currentActiveAbility.ability.OnEquip();
            }
            else
            {
                currentActiveAbility.ability.OnWithHold();
                currentActiveAbility = null;
            }
        }

        private void StartMachineGunRolling()
        {
            if(currentActiveAbility == null)
            {
                machineGun.MakeBarrelRoll();
            }
        }

        private void StartMachineGunRecoil()
        {
            if (currentActiveAbility == null)
            {
                machineGun.StartRecoil();
            }
        }

        private void StopMachineGunRolling()
        {
            if(currentActiveAbility == null)
            {
                machineGun.MakeBarrelUnroll();
            }
        }

        private void StopMachineGunRecoil()
        {
            if(currentActiveAbility == null)
            {
                machineGun.StopRecoil();
            }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            gameInput.OnCommonAbilityToggle.AddListener(CheckAndActivateCommonAbility);
            gameInput.OnSpecialAbilityToggle.AddListener(CheckAndActivateSpecialAbility);
            gameInput.OnFireStarted.AddListener(StartMachineGunRolling);
            gameInput.OnFireStarted.AddListener(StartMachineGunRecoil);
            gameInput.OnFireReleased.AddListener(StopMachineGunRolling);
            gameInput.OnFireReleased.AddListener(StopMachineGunRecoil);
            gameInput.OnFirePerformed.AddListener(Fire);

        }

        // Update is called once per frame
        void Update()
        {
            if(currentTime < waitingTime)
            {
                currentTime += Time.deltaTime;
                return;
            }

            Vector2 rotateInput = gameInput.GetRotateInput();
            //Debug.Log("Rotate Input: " + rotateInput);
            if(currentActiveAbility != null && currentActiveAbility.ability != null)
            {
                currentActiveAbility?.ability.OnAim(rotateInput);
            }
        }

        private void OnDestroy()
        {
            gameInput.OnCommonAbilityToggle.RemoveListener(CheckAndActivateCommonAbility);
            gameInput.OnSpecialAbilityToggle.RemoveListener(CheckAndActivateSpecialAbility);
            gameInput.OnFireStarted.RemoveListener(StartMachineGunRolling);
            gameInput.OnFireStarted.RemoveListener(StartMachineGunRecoil);
            gameInput.OnFireReleased.RemoveListener(StopMachineGunRolling);
            gameInput.OnFireReleased.RemoveListener(StopMachineGunRecoil);
            gameInput.OnFirePerformed.RemoveListener(Fire);
        }
    }
}
