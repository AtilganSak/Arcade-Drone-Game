using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState { Stop, Play, Pause }

    public bool noEntry;

    public GameState gameState { get; private set; }

    UIManager uiManager;
    CargoSystem cargoSystem;
    DroneController drone;
    BatteryModule droneBattery;
    DroneCamera droneCamera;

    private void OnEnable()
    {
        uiManager = FindObjectOfType<UIManager>();
        cargoSystem = FindObjectOfType<CargoSystem>();

        drone = FindObjectOfType<DroneController>();
        if (drone != null)
        {
            droneBattery = drone.GetComponent<BatteryModule>();
            if (droneBattery != null)
                droneBattery.BatteryIsOverEvent.AddListener(DroneBatteryIsOver);
            droneCamera = FindObjectOfType<DroneCamera>();
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

        StopTime();

        uiManager.gameCanvas.ShowEndPanel();
    }

    void DroneBatteryIsOver()
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
