using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;


namespace Valve.VR.InteractionSystem
{

    [System.Serializable]
    public class Dot_Truck : System.Object
    {
        public WheelCollider leftWheel;
        public GameObject leftWheelMesh;
        public WheelCollider rightWheel;
        public GameObject rightWheelMesh;
        public bool motor;
        public bool steering;
        public bool reverseTurn;

    }

   

    public class Dot_Truck_Controller : MonoBehaviour
    {
        public LinearMapping linearMapping;
        public float maxMotorTorque;
        public float maxSteeringAngle;
        public List<Dot_Truck> truck_Infos;

        public void VisualizeWheel(Dot_Truck wheelPair)
        {
            Quaternion rot;
            Vector3 pos;
            wheelPair.leftWheel.GetWorldPose(out pos, out rot);
            wheelPair.leftWheelMesh.transform.position = pos;
            wheelPair.leftWheelMesh.transform.rotation = rot;
            wheelPair.rightWheel.GetWorldPose(out pos, out rot);
            wheelPair.rightWheelMesh.transform.position = pos;
            wheelPair.rightWheelMesh.transform.rotation = rot;
        }

        public void Update()
        {
            float motor = maxMotorTorque * Input.GetAxis("Vertical");
            float steering = maxSteeringAngle * (2*linearMapping.value -1);
            float brakeTorque = Input.GetAxis("LeftTriggerSqueeze") > 0.05 ? Input.GetAxis("LeftTriggerSqueeze") : 0;
            Debug.Log(Input.GetAxis("Vertical") + "    " + motor);
            Debug.Log("Brake:   " + Input.GetAxis("LeftTriggerSqueeze") + "    " + brakeTorque);

            if (brakeTorque > 0.001)
            {
                brakeTorque = maxMotorTorque;
                motor = 0;
            }
            else
            {
                brakeTorque = 0;
            }

            foreach (Dot_Truck truck_Info in truck_Infos)
            {
                if (truck_Info.steering == true)
                {
                    truck_Info.leftWheel.steerAngle = truck_Info.rightWheel.steerAngle = ((truck_Info.reverseTurn) ? -1 : 1) * steering;
                }

                if (truck_Info.motor == true)
                {
                    truck_Info.leftWheel.motorTorque = motor;
                    truck_Info.rightWheel.motorTorque = motor;
                }

                truck_Info.leftWheel.brakeTorque = brakeTorque;
                truck_Info.rightWheel.brakeTorque = brakeTorque;

                VisualizeWheel(truck_Info);
            }

        }


    }
}