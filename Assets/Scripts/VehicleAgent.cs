using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class VehicleAgent : Agent {
    private VehicleMovement vehicleMovement;
    public Checkpoint nextCheckPointToReach;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    public bool trainingMode = true;
    public float timeLeftForEpisode;
    public readonly float episodeMaxDuration = 60f;

    private RaycastHit hit;
    private Color hitColor = Color.blue;

    public static float sumCheckpointsReachedPerEpisode;
    public float nbCheckpointsReachedThisEpisode;
    public float nbCheckpointsReachedPreviousEpisode;

    public static float sumMaxChekpointReached;
    public float maxCheckpointReached;

    public static float sumEpisodeDurations;
    public float episodeStartTime;
    public float episodeDuration;
    public float previousEpisodeDuration;

    public static class Rewards {
        //negative rewards
        public const float SpeedPromoter = -0.001f;
        public const float TimeElapsed = -1f;
        public const float WrongCheckpointDirection = -0.1f;
        public const float WrongCheckpointTaken = -0.1f;
        public const float UpsideDown = -0.2f;
        public const float DoingOffRoad = -0.01f;

        //positive rewards
        public const float checkpointReached = 0.1f;
        public const float LoopEnded = 0.5f;
    }

    private void Start() {
        vehicleMovement = gameObject.GetComponent<VehicleMovement>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        timeLeftForEpisode = episodeMaxDuration;
        episodeStartTime = Time.time;

        LogErrorWhenMissingComponents();
    }

    private void LogErrorWhenMissingComponents() {
        if (vehicleMovement == null) {
            Debug.LogError("No VehicleMovement found in Vehicle GameObject");
        }
        if (nextCheckPointToReach == null) {
            Debug.LogError("No Checkpoint referenced for NextCheckpointToReach");
        }
    }

    private void Update() {
        timeLeftForEpisode -= Time.deltaTime;

        AddRewardIfOffRoad();
        EndEpisodeIfUpsideDown();
    }

    private void EndEpisodeIfUpsideDown() {
        if (Vector3.Dot(transform.up, Vector3.down) > 0) {
            AddReward(Rewards.UpsideDown);
            RegisterEpisodeDuration();
            EndEpisode();
        }
    }

    public void RegisterEpisodeDuration() {
        episodeDuration = Time.time - episodeStartTime;
    }

    private void AddRewardIfOffRoad() {
        Debug.DrawRay(transform.position, Vector3.down * hit.distance, hitColor);
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f)) {
            if (hit.transform.CompareTag("OffRoad")) {
                AddReward(Rewards.DoingOffRoad);
                hitColor = Color.red;
            }
            else {
                hitColor = Color.blue;
            }
        }
    }

    public override void Initialize() {
        if (!trainingMode) {
            MaxStep = 0;
        }
    }

    private void UpdateLogsValues() {
        sumCheckpointsReachedPerEpisode += (nbCheckpointsReachedThisEpisode - nbCheckpointsReachedPreviousEpisode);
        nbCheckpointsReachedPreviousEpisode = nbCheckpointsReachedThisEpisode;
        nbCheckpointsReachedThisEpisode = 0;

        previousEpisodeDuration = episodeDuration;
        episodeDuration = Time.time - episodeStartTime;
        episodeStartTime = Time.time;
        sumEpisodeDurations += (episodeDuration - previousEpisodeDuration);
    }

    public override void OnEpisodeBegin() {
        UpdateLogsValues();
        vehicleMovement.ResetWheels();
        RandomizeVehicleStartPosition();
        transform.rotation = initialRotation;

        GameManager.ResetEnvironmentForVehicle(gameObject.GetComponent<VehicleAgent>());
    }

    private void RandomizeVehicleStartPosition() {
        float offsetRange = 2f;
        float randomPosXOffset = Random.Range(-offsetRange, offsetRange);
        float randomPosZOffset = Random.Range(-offsetRange, offsetRange);
        transform.position = new Vector3(initialPosition.x + randomPosXOffset, initialPosition.y, initialPosition.z + randomPosZOffset);
    }

    public override void CollectObservations(VectorSensor sensor) {
        Vector3 distanceToNextCheckpoint = nextCheckPointToReach.transform.position - transform.position;
        sensor.AddObservation(distanceToNextCheckpoint.normalized);

        AddReward(Rewards.SpeedPromoter);
    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }

    public override void OnActionReceived(ActionBuffers actions) {
        ActionSegment<float> continuousActionsInputs = actions.ContinuousActions;
        vehicleMovement.Steer(continuousActionsInputs[0]);
        vehicleMovement.Accelerate(continuousActionsInputs[1]);
    }
}
