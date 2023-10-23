using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {
    private Collider hitDirectionIndicator;

    private void Start() {

        hitDirectionIndicator = transform.Find("HitDirectionIndicator")?.gameObject.GetComponent<Collider>();

        if (hitDirectionIndicator == null) {
            Debug.LogError("hitDirectionCollider not found in " + gameObject.name);
        }

    }

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<VehicleAgent>() == null) {
            return;
        }

        VehicleAgent vehicleAgentCrossing = other.gameObject.GetComponent<VehicleAgent>();

        bool isHitRightDirection = CheckHitRightDirection(other);
        GameManager.ValidateCheckpointCrossed(this, vehicleAgentCrossing, isHitRightDirection);
    }

    private bool CheckHitRightDirection(Collider other) {
        Vector3 vehiclePosition = other.transform.position;
        Vector3 hitDirectionIndicatorPoint = hitDirectionIndicator.ClosestPointOnBounds(vehiclePosition);
        Vector3 checkpointColliderPoint = gameObject.GetComponent<MeshCollider>().ClosestPointOnBounds(vehiclePosition);

        float hitDirectionDistance = Vector3.Distance(hitDirectionIndicatorPoint, vehiclePosition);
        float meshDistance = Vector3.Distance(checkpointColliderPoint, vehiclePosition);

        bool isHitRightDirection = meshDistance >= hitDirectionDistance;

        return isHitRightDirection;
    }
}