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

    public string upgadeDescription1;
    public string upgadeDescription2;
    public GameObject upgradedPrefab;
    public int upgradeCost;

    public int GetSellAmount()
    {
        return cost / 2;
    }
}
