using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BatteryModule : MonoBehaviour
{
    public enum OccupancyState
    {
        Perfect,
        Ideal,
        Bad,
        Over
    }
    public enum ConsumtionState
    {
        Idle,
        Move
    }

    public bool noBattery;

    public float maxBattery = 1000;
    public float currentBattery = 100;

    public float powerCutThreshold = 10;
    public float powerOnThreshold = 10;

    [Tooltip("Per second")]
    public float idleBatteryConsumtion = 3;
    [Tooltip("Per second")]
    public float movingBatteryConsumtion = 5;

    public int lifeForMinute;
    public int lifeForSecond;

    float currentConsumtion;
    float secondCounter;

    public bool batteryIsOver;

    public OccupancyState occupancyState;
    public ConsumtionState consumtionState;

    public UnityEvent BatteryIsOverEvent;

    DroneController drone;

    private void Start()
    {
        drone = GetComponent<DroneController>();
    }
    private void Update()
    {
        if (!noBattery)
        {
            if (!batteryIsOver)
            {
                SetConsumtionState();

                UseBattery();

                CalculateBatteryLife();

                SetOccupancyState();
            }
        }
    }
    void SetConsumtionState()
    {
        if (drone.AnyInput)
        {
            consumtionState = ConsumtionState.Move;
        }
        else
        {
            consumtionState = ConsumtionState.Idle;
        }
    }
    void UseBattery()
    {
        secondCounter += Time.deltaTime;
        switch (consumtionState)
        {
            case ConsumtionState.Idle:
                if(secondCounter > 1)
                {
                    secondCounter = 0;
                    currentBattery -= idleBatteryConsumtion;
                    currentConsumtion = idleBatteryConsumtion;
                }
                break;
            case ConsumtionState.Move:
                if (secondCounter > 1)
                {
                    secondCounter = 0;
                    currentBattery -= movingBatteryConsumtion;
                    currentConsumtion = movingBatteryConsumtion;
                }
                break;
        }
        if (currentBattery < 0)
            currentBattery = 0;
    }
    void CalculateBatteryLife()
    {
        lifeForSecond = (int)(currentBattery / currentConsumtion);
        lifeForMinute = (lifeForSecond / 60);
    }
    void SetOccupancyState()
    {
        if (currentBattery > 65 && currentBattery <= 100)
        {
            occupancyState = OccupancyState.Perfect;
        }
        else if (currentBattery > 45 && currentBattery <= 65)
        {
            occupancyState = OccupancyState.Ideal;
        }
        else if (currentBattery > powerCutThreshold && currentBattery <= 45)
        {
            occupancyState = OccupancyState.Bad;
        }
        else if (currentBattery >= 0 && currentBattery < powerCutThreshold)
        {
            occupancyState = OccupancyState.Over;

            batteryIsOver = true;

            BatteryIsOverEvent.Invoke();
        }
    }

    public void ChargeBattery(float amount)
    {
        currentBattery += amount;
        if (currentBattery > maxBattery)
        {
            currentBattery = maxBattery;
        }
        if (currentBattery > powerOnThreshold)
        {
            batteryIsOver = false;
        }
    }
}
