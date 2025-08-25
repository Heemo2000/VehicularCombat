using Game;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


public class CarController : MonoBehaviour
{
    [Header("values")]
    public float motorForce = 100f;
    public float brakeForce = 1000f;
    public float SteeringAngle = 10;

    [Header("Wheel Coliders")]
    [SerializeField] WheelCollider frontRightWheelCollider;
    [SerializeField] WheelCollider frontleftWheelCollider;
    [SerializeField] WheelCollider RearRightWheelCollider;
    [SerializeField] WheelCollider RearLeftWheelCollider;

    [Header("Transform")]
   
    [SerializeField] Transform  frontRightWheelTransform;
    [SerializeField] Transform frontleftWheelTransform;
    [SerializeField] Transform RearRightWheelTransform;
    [SerializeField] Transform RearLeftWheelTransform;


    [Header("Movement")]
    public bool IsMobile= false;
    [SerializeField] Joystick MovementJoystick;
   
    [SerializeField] Button brakebutton;
    private float HorizontalInput;
    private float VerticalInput;
    private float currentSteeringAngle;
    private float currentBrakeForce;
    private bool IsBraking;

    [Header("Weapon")]
    [SerializeField] Joystick RotationJoystick;
    [SerializeField] GameObject weaponMachine;
    [SerializeField] int rotationSpeed;

    
 void Start()
 {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0,0.05f, 0);
    }

        
 void Update()
 {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        GunMachineRotation();
 }

    void GetInput()
    {
        if (!IsMobile)
        {
            HorizontalInput = Input.GetAxis("Horizontal");
            VerticalInput = Input.GetAxis("Vertical");
            IsBraking = Input.GetKey(KeyCode.Space);
        }

        else if (IsMobile)
        {
            HorizontalInput = MovementJoystick.Horizontal;
            VerticalInput = MovementJoystick.Vertical;

            

        }

        
    }
    
    private void HandleMotor()
    {
        frontleftWheelCollider.motorTorque = motorForce * VerticalInput;
        frontRightWheelCollider.motorTorque = motorForce * VerticalInput;

        currentBrakeForce = IsBraking ? brakeForce : 0F; 
        ApplyBrakes();
    }
    
    public void ApplyBrakes()
    {
        frontleftWheelCollider.brakeTorque = currentBrakeForce;
        frontRightWheelCollider.brakeTorque = currentBrakeForce;
        RearLeftWheelCollider.brakeTorque = currentBrakeForce;
        RearRightWheelCollider.brakeTorque = currentBrakeForce; 
    }

    private void HandleSteering()
    {
        currentSteeringAngle = SteeringAngle * HorizontalInput;
        frontRightWheelCollider.steerAngle = currentSteeringAngle;
        frontleftWheelCollider.steerAngle = currentSteeringAngle;
    }

    public void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontleftWheelCollider,frontleftWheelTransform); 
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(RearLeftWheelCollider, RearLeftWheelTransform);
        UpdateSingleWheel(RearRightWheelCollider, RearRightWheelTransform);

    }

    private void GunMachineRotation()
    {
  
        float horizontalInputRot = RotationJoystick.Horizontal;
   // Rotate object on Y-axis
        weaponMachine.transform.Rotate(Vector3.up * horizontalInputRot * rotationSpeed * Time.deltaTime);
    }
}

