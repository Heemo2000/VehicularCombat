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
        private Ray ray;
        private RaycastHit hit;
        private Camera mainCamera;

        private void Awake()
        {
            vehicle = GetComponent<Vehicle>();
            input = GetComponent<GameInput>();
            mainCamera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            moveInput = input.GetMoveInput();
            aimPosition = input.GetPointerPosition();
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

            if(aimHandler != null)
            {
                Ray ray = mainCamera.ScreenPointToRay(aimPosition);
                if(Physics.Raycast(ray,out hit))
                {
                    Debug.Log("Hitting level");
                    aimHandler.AimPosition = hit.point;
                }

            }
        }
    }
}
