using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public GameObject ui;

    public TextMeshProUGUI title;

    public TextMeshProUGUI description;
    public TextMeshProUGUI cost;
    public TextMeshProUGUI damage;
    public TextMeshProUGUI range;
    public TextMeshProUGUI firerate;

    //public Shop shop;

    public void SelectTower(TurretBluePrint towerBP)
    {
        title.text = towerBP.title;
        description.text = towerBP.description;
        cost.text = "Cost: <color=#FFD500>$" + towerBP.cost + "</color>";
        damage.text = "Damage: " + towerBP.prefab.GetComponent<Turret>().bulletDamage;
        range.text = "Range: " + towerBP.prefab.GetComponent<Turret>().range * 100;
        firerate.text = "Frequency: " + towerBP.prefab.GetComponent<Turret>().fireRate;

        if(PlayerStats.Money < towerBP.cost)
        {
            cost.color = Color.grey;
        }

        ui.SetActive(true);
    }

    public void DeselectTower()
    {
        //if (BuildManager.instance.CanBuild)
        //    return;

        ui.SetActive(false);
    }

    //public void Select(int index)
    //{
    //    if (BuildManager.instance.CanBuild)
    //        return;

    //    switch (index)
    //    {
    //        case 1:
    //            SelectTower(shop.standardTurret);
    //            return;

    //        case 2:
    //            SelectTower(shop.missileTurret);
    //            return;

    //        case 3:
    //            SelectTower(shop.laserTurret);
    //            return;

    //        default:
    //            break;
    //    }

    //}
}
