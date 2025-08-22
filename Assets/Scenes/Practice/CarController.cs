using UnityEngine;
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
    private float HorizontalInput;
    private float VerticalInput;
    private float currentSteeringAngle;
    private float currentBrakeForce;
    private bool IsBraking;


    
 void Start()
 {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0,0.5f, 0);
    }

        
 void Update()
 {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
 }

    void GetInput()
    {
        HorizontalInput = Input.GetAxis("Horizontal");
        VerticalInput = Input.GetAxis("Vertical");
        IsBraking = Input.GetKey(KeyCode.Space);
    }

    void HandleMotor()
    {
        frontleftWheelCollider.motorTorque = motorForce * VerticalInput;
        frontRightWheelCollider.motorTorque = motorForce * VerticalInput;

        currentBrakeForce = IsBraking ? brakeForce : 0F; 
        ApplyBrakes();
    }
    
    void ApplyBrakes()
    {
        frontleftWheelCollider.brakeTorque = currentBrakeForce;
        frontRightWheelCollider.brakeTorque = currentBrakeForce;
        RearLeftWheelCollider.brakeTorque = currentBrakeForce;
        RearRightWheelCollider.brakeTorque = currentBrakeForce; 
    }

    void HandleSteering()
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

}

