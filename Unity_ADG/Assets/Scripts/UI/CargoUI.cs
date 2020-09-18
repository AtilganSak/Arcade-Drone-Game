using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.CodeDom;

public class CargoUI : MonoBehaviour
{
    [SerializeField] TMP_Text deliveredCargoCountText;

    public void UpdateDeliveredCargoCount(int value)
    {
        deliveredCargoCountText.text = value.ToString();
    }
}
