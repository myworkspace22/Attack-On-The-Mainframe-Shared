using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TurretBluePrint
{
    public string title;
    public string description;
    public GameObject prefab;
    public int cost;
    public int levelUpCost;

    public string[] upgradeNames;
    public string[] upgradeDescription;
    public GameObject[] upgradedPrefab;
    public int[] upgradeCost;
    public string[] upgradeEffect;


    //public int GetSellAmount()
    //{
    //    return cost / 2;
    //}
}
