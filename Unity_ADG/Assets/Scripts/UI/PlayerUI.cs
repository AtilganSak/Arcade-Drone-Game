﻿using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public TMP_Text moneyText;

    public void UpdateMoneyText(int value)
    {
        moneyText.text = value.ToString();
    }
}
