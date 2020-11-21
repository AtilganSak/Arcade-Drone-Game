using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState { Stop, Play, Pause }

    public bool noEntry;

    public GameState gameState { get; private set; }

    UIManager uiManager;
    DroneController drone;
    BatteryModule droneBattery;
    DroneCamera droneCamera;
    Player player;
    CameraManager cameraManager;

    private void OnEnable()
    {
        uiManager = FindObjectOfType<UIManager>();
        player = FindObjectOfType<Player>();
        drone = FindObjectOfType<DroneController>();
        cameraManager = FindObjectOfType<CameraManager>();
        if (drone != null)
        {
            droneBattery = drone.GetComponent<BatteryModule>();
            if (droneBattery != null)
                droneBattery.BatteryIsOverEvent.AddListener(DroneBatteryIsOver);
            droneCamera = FindObjectOfType<DroneCamera>();
        }
        if (player != null)
        {
            player.OnDead += PlayerIsDead;
        }
        if (cameraManager)
        {
            cameraManager.AdjustDamageComponent(player.GetComponent<Damage>());
        }

        if (!noEntry)
        {
            gameState = GameState.Stop;

            DroneControlls(false);
        
            StopTime();
        }
        else
        {
            StartGame();
        }
    }
    private void OnDestroy()
    {
        if (droneBattery != null)
        {
            droneBattery.BatteryIsOverEvent.RemoveListener(DroneBatteryIsOver);
        }
    }
    public void StartGame()
    {
        GC.Collect();

        gameState = GameState.Play;

        DroneControlls(true);

        PlayTime();
    }
    public void PauseGame()
    {
        gameState = GameState.Pause;

        DroneControlls(false);

        StopTime();
    }
    public void FinishGame()
    {
        gameState = GameState.Stop;

        DroneControlls(false);

        uiManager.ClearEffects();

        StopTime();

        uiManager.gameCanvas.ShowEndPanel();
    }

    void DroneBatteryIsOver()
    {
        FinishGame();
    }
    void PlayerIsDead()
    {
        FinishGame();
    }
    void StopTime()
    {
        Time.timeScale = 0.0f;
    }
    void PlayTime()
    {
        Time.timeScale = 1.0f;
    }
    void DroneControlls(bool state)
    {
        drone.isActive = state;
        droneCamera.isActive = state;
        droneBattery.isActive = state;
    }
}
