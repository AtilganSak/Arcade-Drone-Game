using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DroneUI : MonoBehaviour
{
    public TMP_Text batteryPercentageText;
    public TMP_Text minuteText;
    public TMP_Text secondText;

    public void UpdateBatteryPercentageText(string text)
    {
        batteryPercentageText.text = text;
    }
    public void UpdateRemainingTimeText(string minute, string second)
    {
        minuteText.text = minute;
        secondText.text = second;
    }
}
