using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{   
    [Header("Buttons")]
    public Button shopItem;
    public Button shopItem2;
    public Button shopItem3;
    [Header("Towers")]
    public TurretBluePrint standardTurret;
    public TurretBluePrint missileTurret;
    public TurretBluePrint laserTurret;

    public PlayerStats playerStats;

    BuildManager buildManager;
    private void Start()
    {
        buildManager = BuildManager.instance;
    }
    private void Update()
    {
        shopItem.interactable = playerStats.Money >= standardTurret.cost;
        shopItem2.interactable = playerStats.Money >= missileTurret.cost;
        shopItem3.interactable = playerStats.Money >= laserTurret.cost;
    }
    public void SelectStandardTurret()
    {
        Debug.Log("Standard Tower Selected");
        buildManager.SelectTurretToBuild(standardTurret);
    }
    public void SelectMissileTurret()
    {
        Debug.Log("Missile Tower Selected");
        buildManager.SelectTurretToBuild(missileTurret);
    }
    public void SelectLaserTurret()
    {
        Debug.Log("Laser Tower Selected");
        buildManager.SelectTurretToBuild(laserTurret);
    }
}
