using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    public TextMeshProUGUI moneyText;

    public PlayerStats playerStats;

    void Update()
    {
        int income = (playerStats.Money - playerStats.Money % 100) / 5; 

        if (BuildManager.instance.GetComponent<WaveSpawner>().BuildMode)
        {
            moneyText.text = "CURRENCY: <color=#FFD500>$" + playerStats.Money.ToString() + "</color>" + " (+" + income + ")";
        }
        else
        {
            moneyText.text = "CURRENCY: <color=#FFD500>$" + playerStats.Money.ToString() + "</color>";
        }
        
    }
}
