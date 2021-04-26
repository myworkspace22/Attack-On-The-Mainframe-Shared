using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    void Update()
    {
        moneyText.text = "CURRENCY: <color=#FFD500>$" + PlayerStats.Money.ToString() + "</color>" + " (+100)";
    }
}
