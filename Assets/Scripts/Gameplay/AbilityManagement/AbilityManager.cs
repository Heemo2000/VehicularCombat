using UnityEngine;
using UnityEngine.InputSystem.HID;

namespace Game.Gameplay.AbilityManagement
{
    public class AbilityManager : MonoBehaviour
    {
        [SerializeField] private GameInput gameInput;
        [SerializeField] private AbilityData[] commonAbilities;
        [SerializeField] private AbilityData specialAbility;
        
        private Camera mainCamera;
        private RaycastHit hit;
        private void Awake()
        {
            mainCamera = Camera.main;
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            commonAbilities[0].ability.OnEquip();
        }

        // Update is called once per frame
        void Update()
        {
            Ray ray = mainCamera.ScreenPointToRay(gameInput.GetPointerPosition());
            if (Physics.Raycast(ray, out hit))
            {
                commonAbilities[0].ability.OnAim(hit.point);
            }

        }

        private void OnDestroy()
        {
            commonAbilities[0].ability.OnWithHold();
        }
    }
}
