using UnityEngine;

namespace Game.Gameplay
{
    public class Player : MonoBehaviour
    {
        private Vehicle vehicle;
        private GameInput input;
        private Vector2 moveInput;
        private void Awake()
        {
            vehicle = GetComponent<Vehicle>();
            input = GetComponent<GameInput>();
        }

        // Update is called once per frame
        void Update()
        {
            moveInput = input.GetMoveInput();
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
        }
    }
}
