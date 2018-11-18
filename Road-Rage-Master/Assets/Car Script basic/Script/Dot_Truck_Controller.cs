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

    public SteamVR_TrackedObject controller = null;
	public float maxMotorTorque;
	public float maxSteeringAngle;
	public List<Dot_Truck> truck_Infos;
    public float motor1;

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
    }
   
    public void Update()
	{

        device = SteamVR_Controller.Input((int)controller.index);
        if (device == null)
        {
            Debug.Log("TF");
        }
       
        if (device.GetAxis().x != 0 || device.GetAxis().y != 0)
        {
            Debug.Log(device.GetAxis().x + " " + device.GetAxis().y);
            Debug.Log("get axis ! = 0");

            motor1 = maxMotorTorque * Input.GetAxis("Vertical");

            Debug.Log("hello" + Input.GetAxis("Vertical"));
            Debug.Log("TRIGGER VAL: " + SteamVR_Controller.ButtonMask.Trigger);

            
        }

        Debug.Log("motor: "+ motor1);
       

        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
		float brakeTorque = Mathf.Abs(Input.GetAxis("Jump"));
		if (brakeTorque > 0.001) {
			brakeTorque = maxMotorTorque;
			motor1 = 0;
		} else {
			brakeTorque = 0;
		}
       
        foreach (Dot_Truck truck_Info in truck_Infos)
		{
            if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                Debug.Log("TRIGGER IS DOWN");
                truck_Info.motor = false;
            }
            if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) == false)
            {
                Debug.Log("TRIGGER IS RELEASED");
                truck_Info.motor = true;
            }
            if (truck_Info.motor == false)
            {
                truck_Info.leftWheel.motorTorque = 0;
                truck_Info.rightWheel.motorTorque = 0;
            }
            if (truck_Info.steering == true) {
				truck_Info.leftWheel.steerAngle = truck_Info.rightWheel.steerAngle = ((truck_Info.reverseTurn)?-1:1)*steering;
			}

			if (truck_Info.motor == true)
			{
				truck_Info.leftWheel.motorTorque = motor1;
				truck_Info.rightWheel.motorTorque = motor1;
			}

			truck_Info.leftWheel.brakeTorque = 0; //change back to brakeTorque
			truck_Info.rightWheel.brakeTorque = 0;

			VisualizeWheel(truck_Info);
		}

	}


}