using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Dot_Truck : System.Object
{
    //public SteamVR_TrackedController script;
	public WheelCollider leftWheel;
	public GameObject leftWheelMesh;
	public WheelCollider rightWheel;
	public GameObject rightWheelMesh;
	public bool steering;
	public bool reverseTurn;
    public bool motor;


}

public class Dot_Truck_Controller : MonoBehaviour {

    public SteamVR_TrackedObject leftcontroller;
    public SteamVR_TrackedObject rightcontroller;
    public float maxMotorTorque;
	public float maxSteeringAngle;
	public List<Dot_Truck> truck_Infos;
    public float motor1;
    public Rigidbody carbody;
    public Vector3 carVelocity;

    private SteamVR_Controller.Device device;

	public void VisualizeWheel(Dot_Truck wheelPair)
	{
		Quaternion rot;
		Vector3 pos;
		wheelPair.leftWheel.GetWorldPose ( out pos, out rot);
		wheelPair.leftWheelMesh.transform.position = pos;
		wheelPair.leftWheelMesh.transform.rotation = rot;
		wheelPair.rightWheel.GetWorldPose ( out pos, out rot);
		wheelPair.rightWheelMesh.transform.position = pos;
		wheelPair.rightWheelMesh.transform.rotation = rot;
	}

    
    
    public void Start()
    {
        //controller = GetComponent<SteamVR_TrackedObject>();
        //device = SteamVR_Controller.Input((int)controller.index);
        carbody = GetComponent<Rigidbody>();
    }
   
    public void Update()
	{
        carVelocity = carbody.velocity;


        device = SteamVR_Controller.Input((int)leftcontroller.index);
        //Debug.Log("What?:" + leftcontroller);
        if (device == null)
        {
            //Debug.Log("TF");
        }
       
        if (device.GetAxis().x != 0 || device.GetAxis().y != 0)
        {
            //Debug.Log(device.GetAxis().x + " " + device.GetAxis().y);
            //Debug.Log("get axis ! = 0");

            motor1 = maxMotorTorque * -(Input.GetAxis("Vertical"));

            //Debug.Log("hello" + Input.GetAxis("Vertical"));
            //Debug.Log("TRIGGER VAL: " + SteamVR_Controller.ButtonMask.Trigger);

            
        }

        //Debug.Log("motor: "+ motor1);
       

        float steering = (maxSteeringAngle * Input.GetAxis("Horizontal")) / 5;
        //Debug.Log("steering: "+steering);
        float brakeTorque = Mathf.Abs(Input.GetAxis("Jump"));
        //Debug.Log("BRAKETORQUE: " + brakeTorque);
        if (brakeTorque > 0.001) {
			brakeTorque = maxMotorTorque;
            //Debug.Log("POST BRAKETORQUE: " + brakeTorque);
            //motor1 = 0;
        }
        else {
			brakeTorque = 0;
		}
       
        foreach (Dot_Truck truck_Info in truck_Infos)
		{
            //Debug.Log(device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger));
            if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger) == true) 
            {
               
                //When trigger is down you are not able to move
                truck_Info.motor = false;
               
            }
            if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger) == false)
            {
                //When Trigger is not Pulled you are able to drive 
                truck_Info.motor = true;
    
            }
            if (truck_Info.motor == false)
            {

                //When Trigger is Pulled there is a brake 
                truck_Info.leftWheel.brakeTorque = 30000000000000000;
                truck_Info.rightWheel.brakeTorque = 30000000000000000;
                truck_Info.leftWheel.motorTorque = 0;
                truck_Info.rightWheel.motorTorque = 0;
          
                Debug.Log("BRAKE: " + truck_Info.leftWheel.motorTorque);
                motor1 = 0;
            }
            if (truck_Info.steering == true) {
				truck_Info.leftWheel.steerAngle = truck_Info.rightWheel.steerAngle = ((truck_Info.reverseTurn)?-1:1)*steering;
			}

			if (truck_Info.motor == true)
			{
                truck_Info.leftWheel.brakeTorque = 0; 
                truck_Info.rightWheel.brakeTorque = 0;

                truck_Info.leftWheel.motorTorque = motor1;
				truck_Info.rightWheel.motorTorque = motor1;
			}

		
			VisualizeWheel(truck_Info);
		}

	}


}