using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CheckpointManager : MonoBehaviour {
    public static float maxTimeToReachNextCheckpoint = 30f;

    private Dictionary<VehicleAgent, int> vehicleAgentsIds;
    public int[] currentCheckpointIndexes;
    public List<Checkpoint> checkpoints;

    void Start() {
        CheckNbCheckpointsMatch();

        RegisterVehicleAgentsIds();

        ResetCheckpoints();
    }

    private void RegisterVehicleAgentsIds() {
        VehicleAgent[] vehicleAgentsFound = GetVehicleAgentsInSameRoad();
        int nbVehicles = vehicleAgentsFound.Length;
        currentCheckpointIndexes = new int[nbVehicles];

        vehicleAgentsIds = new Dictionary<VehicleAgent, int>(nbVehicles);
        for (int i = 0; i < nbVehicles; i++) {
            vehicleAgentsIds.Add(vehicleAgentsFound[i], i);
        }
    }

    private VehicleAgent[] GetVehicleAgentsInSameRoad() {
        return gameObject.transform.parent.Find("Vehicles").GetComponentsInChildren<VehicleAgent>();
    }

    private void CheckNbCheckpointsMatch() {
        Checkpoint[] checkpointsFound = gameObject.GetComponentsInChildren<Checkpoint>();

        if(checkpoints.Count != checkpointsFound.Length) {
            Debug.LogError("Unmatching number of checkpoints, " + checkpoints.Count + " referenced, " + checkpointsFound.Length + " found");
        }
    }

    public void ResetCheckpoints() {
        foreach (VehicleAgent vehicleAgent in vehicleAgentsIds.Keys) {
            ResetCheckpointsForVehicle(vehicleAgent);
            UpdateNextCheckpointForVehicle(vehicleAgent);
        }
    }

    public void ResetCheckpointsForVehicle(VehicleAgent vehicleAgent) {
        int vehicleId = vehicleAgentsIds[vehicleAgent];
        currentCheckpointIndexes[vehicleId] = 0;
        UpdateNextCheckpointForVehicle(vehicleAgent);
    }

    public void ValidateCheckpointCrossed(Checkpoint checkpoint, VehicleAgent vehicleAgentCrossing, bool isHitRightDirection) {
        if (checkpoint != vehicleAgentCrossing.nextCheckPointToReach) {
            vehicleAgentCrossing.AddReward(VehicleAgent.Rewards.WrongCheckpointTaken);
            return;
        }

        if (!isHitRightDirection) {
            vehicleAgentCrossing.AddReward(VehicleAgent.Rewards.WrongCheckpointDirection);
            return;
        }

        bool isLoopDone = currentCheckpointIndexes[vehicleAgentsIds[vehicleAgentCrossing]] == checkpoints.Count - 1;
        AddReachingCheckpointRewardForVehicle(vehicleAgentCrossing, isLoopDone);
    }

    private void AddReachingCheckpointRewardForVehicle(VehicleAgent vehicleAgentCrossing, bool isLoopDone) {
        int vehicleAgentCrossingId = vehicleAgentsIds[vehicleAgentCrossing];
        UpdateVehicleAgentLogsValues(vehicleAgentCrossing);

        if (isLoopDone) {
            currentCheckpointIndexes[vehicleAgentCrossingId] = 0;

            vehicleAgentCrossing.AddReward(VehicleAgent.Rewards.LoopEnded);
            vehicleAgentCrossing.EndEpisode();
        }
        else {
            currentCheckpointIndexes[vehicleAgentCrossingId]++;
            vehicleAgentCrossing.AddReward(VehicleAgent.Rewards.checkpointReached);
            UpdateNextCheckpointForVehicle(vehicleAgentCrossing);
        }
    }

    private static void UpdateVehicleAgentLogsValues(VehicleAgent vehicleAgentCrossing) {
        vehicleAgentCrossing.nbCheckpointsReachedThisEpisode++;
        if (vehicleAgentCrossing.nbCheckpointsReachedThisEpisode > vehicleAgentCrossing.maxCheckpointReached) {
            vehicleAgentCrossing.maxCheckpointReached = vehicleAgentCrossing.nbCheckpointsReachedThisEpisode;
            VehicleAgent.sumMaxChekpointReached++;
        }
    }

    private void UpdateNextCheckpointForVehicle(VehicleAgent vehicleAgentCrossing) {
        if (checkpoints.Count > 0) {
            int vehicleId = vehicleAgentsIds[vehicleAgentCrossing];
            int checkpointToReachId = currentCheckpointIndexes[vehicleId];
            vehicleAgentCrossing.nextCheckPointToReach = checkpoints[checkpointToReachId];
        }
    }

    public bool IsCheckpointInCheckpointManager(Checkpoint checkpointToCompare) {
        return checkpoints.IndexOf(checkpointToCompare) != -1;
    }

}