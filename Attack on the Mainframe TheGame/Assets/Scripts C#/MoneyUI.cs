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
        int income = (PlayerStats.Money - PlayerStats.Money % 100) / 4; 

        if (BuildManager.instance.GetComponent<WaveSpawner>().BuildMode)
        {
            moneyText.text = "CURRENCY: <color=#FFD500>$" + PlayerStats.Money.ToString() + "</color>"; // + " (+" + income + ")"
        }
        else
        {
            moneyText.text = "CURRENCY: <color=#FFD500>$" + PlayerStats.Money.ToString() + "</color>";
        }
        
    }
}
