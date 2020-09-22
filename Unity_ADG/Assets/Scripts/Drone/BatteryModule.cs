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

    public bool isActive = true;

    public bool noBattery;

    public float maxBattery = 1000;
    public float currentBattery = 100;

    public float batteryPercentage
    {
        get => (currentBattery / maxBattery) * 100;
    }

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

    float singleSecond;

    public bool batteryIsOver;

    public OccupancyState occupancyState;
    public ConsumtionState consumtionState;

    public UnityEvent BatteryIsOverEvent;

    DroneController drone;
    DroneUI droneUI;

    string[] numberArray;

    private void OnEnable()
    {
        numberArray = new string[101];
        for (int i = 0; i < 101; i++)
        {
            if (i < 10)
                numberArray[i] = "0" + i;
            else
                numberArray[i] = i.ToString();
        }
    }
    private void Start()
    {
        droneUI = FindObjectOfType<DroneUI>();
        drone = GetComponent<DroneController>();

        UpdateUI();
    }
    private void Update()
    {
        if (!isActive) return;

        if (!noBattery)
        {
            if (!batteryIsOver)
            {
                SetConsumtionState();

                UseBattery();

                CalculateBatteryLife();

                UpdateUI();

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
        singleSecond = currentBattery / currentConsumtion;
        lifeForMinute = (int)(singleSecond / 60F);
        lifeForSecond = (int)(singleSecond % 60F);
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
        else if (currentBattery >= 0 && currentBattery <= powerCutThreshold)
        {
            occupancyState = OccupancyState.Over;

            batteryIsOver = true;

            BatteryIsOverEvent.Invoke();
        }
    }
    void UpdateUI()
    {
        droneUI.UpdateBatteryPercentageText(numberArray[(int)batteryPercentage]);
        //if(lifeForMinute >= 0 && lifeForSecond >= 0)
        //    droneUI.UpdateRemainingTimeText(numberArray[lifeForMinute], numberArray[lifeForSecond]);
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
