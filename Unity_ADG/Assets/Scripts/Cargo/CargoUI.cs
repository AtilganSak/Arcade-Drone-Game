using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.CodeDom;

public class CargoUI : MonoBehaviour
{
    [SerializeField] TMP_Text totalCargoText;
    [SerializeField] TMP_Text remaningCargoCountText;
    [SerializeField] TMP_Text deliveredCargoCountText;

    public void UpdateTotalCargoCount(int value)
    {
        totalCargoText.text = value.ToString();
    }
    public void UpdateRemaningCargoCount(int value)
    {
        remaningCargoCountText.text = value.ToString();
    }
    public void UpdateDeliveredCargoCount(int value)
    {
        deliveredCargoCountText.text = value.ToString();
    }
}
