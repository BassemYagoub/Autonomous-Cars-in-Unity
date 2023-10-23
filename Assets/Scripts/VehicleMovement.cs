using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VehicleMovement : MonoBehaviour {
    private Rigidbody vehicleRigidbody;
    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // maximum steer angle the wheel can have

    private void Start() {
        vehicleRigidbody = this.GetComponent<Rigidbody>();
    }

    public void Steer(float horizontalAxisInput) {
        float steering = maxSteeringAngle * horizontalAxisInput;
        foreach (AxleInfo axleInfo in axleInfos) {
            if (axleInfo.steering) {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
    }

    public void Accelerate(float verticalAxisInput) {
        float motor = maxMotorTorque * verticalAxisInput;

        foreach (AxleInfo axleInfo in axleInfos) {
            if (axleInfo.motor) {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
    }

    public void ResetWheels() {
        vehicleRigidbody.velocity = new Vector3(0,0,0);
        vehicleRigidbody.angularVelocity = new Vector3(0, 0, 0);

        foreach (AxleInfo axleInfo in axleInfos) {
            axleInfo.leftWheel.motorTorque = 0;
            axleInfo.leftWheel.steerAngle = 0;

            axleInfo.rightWheel.motorTorque = 0;
            axleInfo.rightWheel.steerAngle = 0;

            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }

    }

    public void ApplyLocalPositionToVisuals(WheelCollider collider) {
        if (collider.transform.childCount == 0) {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

}