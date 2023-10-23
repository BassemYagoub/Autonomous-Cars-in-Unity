using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour {
    private static GameManager gameManagerSingleton;

    public Camera mainCamera;
    private Camera[] vehiclesCameras;

    private TextMeshProUGUI nbCheckpointsReachedText;
    private TextMeshProUGUI maxCheckpointReachedText;
    private TextMeshProUGUI episodeDurationMeanText;

    private CheckpointManager[] checkpointManagers;
    private List<VehicleAgent> vehicleAgents;

    private void Start() {
        gameManagerSingleton = this;
        nbCheckpointsReachedText = GameObject.Find("NbCheckpointsReached").GetComponent<TextMeshProUGUI>();
        maxCheckpointReachedText = GameObject.Find("MaxCheckpointReached").GetComponent<TextMeshProUGUI>();
        episodeDurationMeanText = GameObject.Find("EpisodeDurationMean").GetComponent<TextMeshProUGUI>();
        checkpointManagers = GameObject.FindObjectsOfType<CheckpointManager>();
        VehicleAgent[] vehicleAgentsFound = GameObject.FindObjectsOfType<VehicleAgent>();
        int nbVehicles = vehicleAgentsFound.Length;
        vehicleAgents = new List<VehicleAgent>(vehicleAgentsFound);

        LogErrorWhenMissingComponents();

        InstantiateVehicleCamera(nbVehicles);
    }

    private void InstantiateVehicleCamera(int nbVehicles) {
        if (nbVehicles > 0) {
            vehiclesCameras = vehicleAgents[0].gameObject.GetComponentsInChildren<Camera>();
        }
    }

    private void LogErrorWhenMissingComponents() {
        if (checkpointManagers == null) {
            Debug.LogError("No CheckpointManager component found");
        }
        if (vehicleAgents == null) {
            Debug.LogError("No VehicleAgent component found");
        }
    }

    private void Update() {
        UpdateUIValues();
        EndEpisodeIfTimeElapsed();

        if (Input.GetKeyDown(KeyCode.C)) {
            SwitchCameraView();
        }
    }

    private void EndEpisodeIfTimeElapsed() {
        foreach (VehicleAgent vehicleAgent in vehicleAgents) {
            if (vehicleAgent.timeLeftForEpisode < 0f) {
                vehicleAgent.timeLeftForEpisode = CheckpointManager.maxTimeToReachNextCheckpoint;
                vehicleAgent.AddReward(VehicleAgent.Rewards.TimeElapsed);
                vehicleAgent.RegisterEpisodeDuration();
                vehicleAgent.EndEpisode();
            }
        }
    }

    /// <summary>
    /// Switches Camera View from the first vehicleAgent found.
    /// Main Camera --> FPS Camera --> TPS Camera --> Main Camera...
    /// </summary>
    private void SwitchCameraView() {
        if (mainCamera.enabled) {
            SwitchCameraFromMainToFPSView();
        }
        else if (vehiclesCameras[0].enabled) {
            SwitchCameraFromFPSToTPSView();
        }
        else {
            SwitchCameraFromTPSToMainView();
        }
    }

    private void SwitchCameraFromTPSToMainView() {
        vehiclesCameras[1].enabled = false;
        mainCamera.enabled = true;
    }

    private void SwitchCameraFromFPSToTPSView() {
        vehiclesCameras[0].enabled = false;
        vehiclesCameras[1].enabled = true;
    }

    private void SwitchCameraFromMainToFPSView() {
        mainCamera.enabled = false;
        vehiclesCameras[0].enabled = true;
    }

    public static void ResetEnvironmentForVehicle(VehicleAgent vehicleAgentReseting) {
        CheckpointManager checkpointManagerForVehicle = GetCorrespondingCheckpointManager(vehicleAgentReseting.nextCheckPointToReach);

        if(checkpointManagerForVehicle == null) {
            Debug.LogError("No CheckpointManager associated with " + vehicleAgentReseting + " found");
        }

        checkpointManagerForVehicle.ResetCheckpointsForVehicle(vehicleAgentReseting);
        vehicleAgentReseting.timeLeftForEpisode = vehicleAgentReseting.episodeMaxDuration;
    }

    private static CheckpointManager GetCorrespondingCheckpointManager(Checkpoint checkpointCrossed) {
        CheckpointManager checkpointManagerForVehicle = checkpointCrossed.gameObject.GetComponentInParent<CheckpointManager>();

        if(checkpointManagerForVehicle == null) {
            Debug.LogError("No CheckpointManager parent found for checkpoint " + checkpointCrossed);
        }

        return checkpointManagerForVehicle;
    }

    public static void ValidateCheckpointCrossed(Checkpoint checkpointCrossed, VehicleAgent vehicleAgentCrossing, bool isHitRightDirection) {
        CheckpointManager checkpointManagerOfVehicle = GetCorrespondingCheckpointManager(vehicleAgentCrossing.nextCheckPointToReach);
        CheckpointManager checkpointManagerOfCheckpoint = GetCorrespondingCheckpointManager(checkpointCrossed);

        if(checkpointManagerOfVehicle != checkpointManagerOfCheckpoint) {
            vehicleAgentCrossing.AddReward(VehicleAgent.Rewards.WrongCheckpointTaken);
        }
        else {
            checkpointManagerOfVehicle.ValidateCheckpointCrossed(checkpointCrossed, vehicleAgentCrossing, isHitRightDirection);
        }
    }

    public static void UpdateUIValues() {
        float meanMaxCheckpointReached = (VehicleAgent.sumMaxChekpointReached / gameManagerSingleton.vehicleAgents.Count);
        float meanNbCheckpointsReached = (VehicleAgent.sumCheckpointsReachedPerEpisode / gameManagerSingleton.vehicleAgents.Count);
        float meanEpisodeDuration = (VehicleAgent.sumEpisodeDurations / gameManagerSingleton.vehicleAgents.Count);
        gameManagerSingleton.nbCheckpointsReachedText.text = "Nb Checkpoints Reached Per Ep: " + meanNbCheckpointsReached;
        gameManagerSingleton.maxCheckpointReachedText.text = "Max Checkpoint Reached: " + meanMaxCheckpointReached;
        gameManagerSingleton.episodeDurationMeanText.text = "Episode Duration Mean: " + meanEpisodeDuration;
    }

    public static void QuitGame() {
        Application.Quit();
    }

}
