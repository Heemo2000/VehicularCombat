using UnityEngine;

namespace Game.Gameplay
{
    public class Player : MonoBehaviour
    {
        
        [SerializeField] private AimHandler aimHandler;
        [SerializeField] private Weapons.Weapon weapon;
        private Vehicle vehicle;
        private GameInput input;
        private Vector2 moveInput;
        private Vector2 aimPosition;

        private void Awake()
        {
            vehicle = GetComponent<Vehicle>();
            input = GetComponent<GameInput>();
        }

        // Update is called once per frame
        void Update()
        {
            moveInput = input.GetMoveInput();
            aimPosition = input.GetRotateInput();
        }

        private void FixedUpdate()
        {
            if (vehicle != null) 
            {
                if(moveInput.y == 0.0f)
                {
                    vehicle.BrakesApplied = true;
                }
                else
                {
                    vehicle.BrakesApplied = false;
                }
                
                vehicle.Input = moveInput;
                vehicle.BrakesApplied = input.BrakeApplied;        
            }

            if(aimHandler != null && aimHandler.gameObject.activeInHierarchy)
            {
                aimHandler.YInput = input.GetRotateInput().x;
            }
        }
    }
}
